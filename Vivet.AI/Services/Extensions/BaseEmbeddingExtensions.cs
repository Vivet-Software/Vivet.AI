using System;
using Vivet.AI.Config;
using Vivet.AI.Data.Models;
using Vivet.AI.Models.Enums;
using Vivet.AI.Services.Requests.Embedding.Common.Models.ConfigOverrides;

namespace Vivet.AI.Services.Extensions;

internal static class BaseEmbeddingExtensions
{
    internal static double GetRecencyScore<T>(this T record, BaseScoringOptions scoringOptions, BaseScoringConfigOverrides overrides)
        where T : BaseEmbedding
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        if (scoringOptions == null)
            throw new ArgumentNullException(nameof(scoringOptions));

        var resultDateTime = DateTimeOffset.FromUnixTimeSeconds(record.UnixTimestamp).UtcDateTime;
        var ageInDays = (DateTimeOffset.UtcNow - resultDateTime).TotalDays;

        var recencyDecayStrategy = overrides.RecencyDecayStrategy ?? scoringOptions.RecencyDecayStrategy;
        var recencyBoostMax = overrides.RecencyBoostMax ?? scoringOptions.RecencyBoostMax;
        var recencyDecayDays = overrides.RecencyDecayDays ?? scoringOptions.RecencyDecayDays;
        var recencySigmoidSteepness = overrides.RecencySigmoidSteepness ?? scoringOptions.RecencySigmoidSteepness;

        return recencyDecayStrategy switch
        {
            RecencyDecayStrategy.Linear => Math.Max(0, recencyBoostMax - ageInDays * recencyBoostMax / recencyDecayDays),
            RecencyDecayStrategy.Exponential => recencyBoostMax * Math.Exp(-ageInDays / recencyDecayDays),
            RecencyDecayStrategy.Sigmoid => recencyBoostMax / (1 + Math.Exp((ageInDays - recencyDecayDays) / recencySigmoidSteepness)),
            _ => 0
        };
    }
}