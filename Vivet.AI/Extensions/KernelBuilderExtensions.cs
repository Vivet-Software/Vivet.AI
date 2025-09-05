using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Plugins.TextSearch.Mappers;

namespace Vivet.AI.Extensions;

// TODO: Plugins / Functions
// - Some Plugins / functions should also be added to Metadata / (Summarization). E.g. a new Movie, book or song might not be known to the LLM when getting metadata
// - Knowledge, Memory plugin / function (Do we need to implement ISemanticTextMemory for memory interface with plugins / functions, or can we use our own MemoryVectorStore
// - Google, Bing, etc online search - https://learn.microsoft.com/en-us/semantic-kernel/concepts/text-search/out-of-the-box-textsearch/google-textsearch?pivots=programming-language-csharp

// FOR USING ISemanticTextMemory
//var services = new ServiceCollection();
//var a = services.BuildServiceProvider().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
//var semanticTextMemory = new MemoryBuilder()
//    .WithTextEmbeddingGeneration(services.BuildServiceProvider().GetRequiredService<ITextEmbeddingGenerationService>()) // .WithTextEmbeddingGeneration(services.BuildServiceProvider().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>())
//    .WithMemoryStore<IMemoryStore>(x => new MilvusMemoryStore(new MilvusClient(new Uri(""))))
//    .Build();

//string aa = await semanticTextMemory
//    .SaveInformationAsync("collection", "text", "id");
//semanticTextMemory.SearchAsync()

internal static class KernelBuilderExtensions
{
    internal static string VectorStoreSearchPluginNameTemplate = "Search{0}Plugin";
    internal static string VectorStoreSearchFunctionNameTemplate = "Search{0}";

    internal static IKernelBuilder AddVectorStoreSearches(this IKernelBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        builder
            .AddVectorStoreSearch<Knowledge>(serviceProvider)
            .AddVectorStoreSearch<Memory>(serviceProvider);

        return builder;
    }


    private static IKernelBuilder AddVectorStoreSearch<TEmbedding>(this IKernelBuilder builder, IServiceProvider serviceProvider)
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

        var pluginName = string.Format(KernelBuilderExtensions.VectorStoreSearchPluginNameTemplate, typeof(TEmbedding).Name);

        var searchPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, null, [kernelFunction]);

        builder.Plugins
            .Add(searchPlugin);

        return builder;
    }
}