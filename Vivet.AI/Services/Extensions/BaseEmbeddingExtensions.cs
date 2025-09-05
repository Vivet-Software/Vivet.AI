using System;
using Vivet.AI.Config;
using Vivet.AI.Config.Enums;
using Vivet.AI.Data.Models;

namespace Vivet.AI.Services.Extensions;

internal static class BaseEmbeddingExtensions
{
    internal static double GetRecencyScore<T>(this T record, BaseScoringOptions scoringOptions)
        where T : BaseEmbedding
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        if (scoringOptions == null)
            throw new ArgumentNullException(nameof(scoringOptions));

        var resultDateTime = DateTimeOffset.FromUnixTimeSeconds(record.UnixTimestamp).UtcDateTime;
        var ageInDays = (DateTimeOffset.UtcNow - resultDateTime).TotalDays;

        return scoringOptions.RecencyDecayStrategy switch
        {
            RecencyDecayStrategy.Linear => Math.Max(0, scoringOptions.RecencyBoostMax - ageInDays * scoringOptions.RecencyBoostMax / scoringOptions.RecencyDecayDays),
            RecencyDecayStrategy.Exponential => scoringOptions.RecencyBoostMax * Math.Exp(-ageInDays / scoringOptions.RecencyDecayDays),
            RecencyDecayStrategy.Sigmoid => scoringOptions.RecencyBoostMax / (1 + Math.Exp((ageInDays - scoringOptions.RecencyDecayDays) / scoringOptions.RecencySigmoidSteepness)),
            _ => 0
        };
    }
}