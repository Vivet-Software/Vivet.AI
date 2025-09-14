using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vivet.AI.Config;
using Vivet.AI.Config.Enums;
using Vivet.AI.Data.Models;
using Vivet.AI.Extensions.Consts;

namespace Vivet.AI.Extensions;

// BUG: Vector Plugin
// REQUEST:
//        Query = request.Question,
//        Criteria =
//        {
//            UserId = request.UserId,
//            ScopeId = request.ScopeId,
//            AgentId = request.AgentId,
//            DateRange = new DateRange
//            {
//                FromAt = fromAt
//            }
//        },
//        CurrentThreadId = request.CurrentThreadId,
//        Limit = limit
//var limit = this.options.Memory.UseQueryDeduplication
//    ? this.options.Memory.ContextQueryLimit * 2
//    : this.options.Memory.ContextQueryLimit;

// RESPONSE:
// - FullContext
// - CounterpartContext
// - CreatedAt
// - Blob (GetDataUri)

// REQUEST:
//Query = request.Question,
//Criteria =
//{
//    TenantId = request.TenantId,
//    SubTenantId = request.SubTenantId,
//    ScopeId = request.ScopeId,
//    UserId = request.UserId
//},
//Limit = limit
//var limit = this.options.Knowledge.UseQueryDeduplication
//    ? this.options.Knowledge.ContextQueryLimit * 2
//    : this.options.Knowledge.ContextQueryLimit;

// RESPONSE:
// - Source
// - FullContext
// - BlobMetadata
// - CreatedAt

internal static class VectorStoreTextSearchExtensions
{
    private const string BASEVECTOR_STORE_SEARCH_FUNCTION_NAME = "Search{0}";

    internal static KernelFunction CreateSearchFunction<T>(this VectorStoreTextSearch<T> textSearch, ChatOptions chatOptions)
        where T : BaseEmbedding
    {
        var functionName = string.Format(VectorStoreTextSearchExtensions.BASEVECTOR_STORE_SEARCH_FUNCTION_NAME, typeof(T).Name);

        //var parameters = typeof(T)
        //    .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
        //    .Select(x =>
        //    {
        //        var parameterAttribute = x
        //            .GetCustomAttribute<TextSearchParameterAttribute>();

        //        if (parameterAttribute == null)
        //        {
        //            return null;
        //        }

        //        return new KernelParameterMetadata(x.Name)
        //        {
        //            Description = parameterAttribute.Description,
        //            DefaultValue = parameterAttribute.DefaultValue,
        //            IsRequired = parameterAttribute.IsRequired
        //        };
        //    })
        //    .Where(x => x != null)
        //    .ToList();
        var parameters = new List<KernelParameterMetadata>();

        var limit = typeof(T) switch
        {
            var x when x == typeof(Memory) => chatOptions.Memory.ContextQueryLimit,
            var x when x == typeof(Knowledge) => chatOptions.Knowledge.ContextQueryLimit,
            _ => 5
        };

        parameters
            .AddRange(
            [
                new KernelParameterMetadata("skip") { IsRequired = true, DefaultValue = 0 },
                new KernelParameterMetadata("limit") { IsRequired = true, DefaultValue = limit }
            ]);

        var options = new KernelFunctionFromMethodOptions
        {
            FunctionName = functionName,
            Description = null,
            Parameters = parameters,
            ReturnParameter = new KernelReturnParameterMetadata
            {
                ParameterType = typeof(KernelSearchResults<string>)
            }
        };

        return textSearch
            .CreateGetTextSearchResults(options);
    }
}

/// <summary>
/// Result mapper which converts a <see cref="BaseEmbedding"/> to a <see cref="TextSearchResult"/>.
/// </summary>
public sealed class EmbeddingTextSearchResultMapper : ITextSearchResultMapper
{
    /// <inheritdoc />
    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is BaseEmbedding embedding)
        {
            return new TextSearchResult(embedding.Content)
            {
                Name = embedding.Id.ToString()
            };
        }

        throw new ArgumentException("Invalid result type.");
    }
}

/// <summary>
/// String mapper which converts a <see cref="BaseEmbedding"/> to a string.
/// </summary>
public sealed class EmbeddingTextSearchStringMapper : ITextSearchStringMapper
{
    /// <inheritdoc />
    public string MapFromResultToString(object result)
    {
        if (result is BaseEmbedding embedding)
        {
            return embedding.Content;
        }

        throw new ArgumentException("Invalid result type.");
    }
}

internal static class KernelBuilderExtensions
{
    internal static IKernelBuilder AddChatPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, ChatOptions chatOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        if (chatOptions == null)
            throw new ArgumentNullException(nameof(chatOptions));

        builder
            .AddWebSearchPlugin(chatOptions.Plugins.BuiltInPlugins.WebSearch)
            .AddVectorStorePlugin<Knowledge>(serviceProvider)
            .AddVectorStorePlugin<Memory>(serviceProvider);

        var types = chatOptions.Plugins.CustomPlugins
            .Select(x => Type.GetType(x, true)); 

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddMetadataPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, MetadataOptions metadataOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (metadataOptions == null)
            throw new ArgumentNullException(nameof(metadataOptions));

        var types = metadataOptions.Plugins.CustomPlugins
            .Select(x => Type.GetType(x, true));

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }
    internal static IKernelBuilder AddSummarizationPluginsFromConfiguration(this IKernelBuilder builder, IServiceProvider serviceProvider, SummarizationOptions summarizationOptions)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (summarizationOptions == null) 
            throw new ArgumentNullException(nameof(summarizationOptions));

        var types = summarizationOptions.Plugins.CustomPlugins
            .Select(x => Type.GetType(x, true));

        foreach (var type in types)
        {
            builder
                .AddFromType(type, serviceProvider);
        }

        return builder;
    }


    private static IKernelBuilder AddWebSearchPlugin(this IKernelBuilder builder, WebSearchPluginOptions options = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return null;
        }

        var textSearchOptions = new TextSearchOptions
        {
            Top = options.Limit
        };

        List<KernelFunction> webSearchFunctions;

        switch (options.Provider)
        {
            case WebSearchProvider.Bing:
                var bingTextSearch = new BingTextSearch(options.ApiKey);

                var bingSearchFunc = bingTextSearch
                    .CreateSearch(searchOptions: textSearchOptions);

                var bingGetResultsFunc = bingTextSearch
                    .CreateGetSearchResults(searchOptions: textSearchOptions);

                var bingGetTextResultsFunc = bingTextSearch
                    .CreateGetTextSearchResults(searchOptions: textSearchOptions);

                webSearchFunctions =
                [
                    bingSearchFunc,
                    bingGetResultsFunc,
                    bingGetTextResultsFunc
                ];

                break;

            case WebSearchProvider.Google:
                var initializer = new BaseClientService.Initializer
                {
                    ApiKey = options.ApiKey
                };
                var googleTextSearch = new GoogleTextSearch(initializer, options.Id);

                var googleSearchFunc = googleTextSearch
                    .CreateSearch(searchOptions: textSearchOptions);

                var googleGetResultsFunc = googleTextSearch
                    .CreateGetSearchResults(searchOptions: textSearchOptions);

                var googleGetTextResultsFunc = googleTextSearch
                    .CreateGetTextSearchResults(searchOptions: textSearchOptions);

                webSearchFunctions =
                [
                    googleSearchFunc,
                    googleGetResultsFunc,
                    googleGetTextResultsFunc
                ];

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.Provider));
        }

        builder.Plugins
            .AddFromFunctions(BuiltInPluginNames.WEB_SEARCH_PLUGIN, webSearchFunctions);

        return builder;
    }
    private static IKernelBuilder AddVectorStorePlugin<TEmbedding>(this IKernelBuilder builder, IServiceProvider serviceProvider)
        where TEmbedding : BaseEmbedding
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var serviceId = typeof(TEmbedding).Name;

        var textSearchStringMapper = new EmbeddingTextSearchStringMapper();
        var textSearchResultMapper = new EmbeddingTextSearchResultMapper();
        var textSearchOptions = new VectorStoreTextSearchOptions();

        builder
            .AddVectorStoreTextSearch<TEmbedding>(textSearchStringMapper, textSearchResultMapper, textSearchOptions);

        var textSearch = serviceProvider
            .GetRequiredKeyedService<VectorStoreTextSearch<TEmbedding>>(serviceId);

        var chatOptions = serviceProvider
            .GetRequiredService<ChatOptions>();

        var kernelFunction = textSearch
            .CreateSearchFunction(chatOptions);

        var pluginName = string.Format(BuiltInPluginNames.VECTOR_STORE_SEARCH_PLUGIN, typeof(TEmbedding).Name);
        var description = "DESCRIPTION";

        var searchPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, description, [kernelFunction]);

        builder.Plugins
            .Add(searchPlugin);

        return builder;
    }
    private static void AddFromType(this IKernelBuilder kernelBuilder, Type type, IServiceProvider serviceProvider)
    {
        if (kernelBuilder == null) 
            throw new ArgumentNullException(nameof(kernelBuilder));

        if (type == null) 
            throw new ArgumentNullException(nameof(type));

        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        if (!typeof(object).IsAssignableFrom(type))
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} is invalid.");
        }

        var constructorInfo = type
            .GetConstructors()
            .OrderByDescending(x => x.GetParameters().Length)
            .FirstOrDefault();

        if (constructorInfo == null)
        {
            throw new InvalidOperationException($"Plugin type {type.FullName} has no public constructor.");
        }

        var parameters = constructorInfo
            .GetParameters()
            .Select(x => serviceProvider.GetService(x.ParameterType) ?? throw new InvalidOperationException($"Cannot resolve constructor parameter {x.ParameterType.FullName} for plugin {type.FullName}"))
            .ToArray();

        var instance = constructorInfo
            .Invoke(parameters);

        kernelBuilder.Plugins
            .AddFromObject(instance, type.Name);
    }
}