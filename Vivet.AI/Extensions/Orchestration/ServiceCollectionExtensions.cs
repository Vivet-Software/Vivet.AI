using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ImageToText;
using Vivet.AI.Config;
using Vivet.AI.SemanticKernel.Services;

namespace Vivet.AI.Extensions.Orchestration;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNullTranscriptionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Transcription == null)
        {
            return services;
        }

        services
            .AddScoped<IAudioToTextService, NullAudioToTextService>();

        services
            .AddTranscriptionServices(options);

        return services;
    }

    internal static IServiceCollection AddNullVisionServices(this IServiceCollection services, AiOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Vision == null)
        {
            return services;
        }

        services
            .AddScoped<IImageToTextService, NullImageToTextService>();

        services
            .AddTranscriptionServices(options);

        return services;
    }
}