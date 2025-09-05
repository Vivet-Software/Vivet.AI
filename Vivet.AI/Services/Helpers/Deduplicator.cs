using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

namespace Vivet.AI.Services.Helpers;

internal static class Deduplicator
{
    public static MemoryResult[] DeduplicateMemoryResults(IEnumerable<MemoryResult> results, double similarityThreshold = 0.90)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results));

        var deduplicatedResults = new List<MemoryResult>();

        // Deduplicate results.
        foreach (var result in results.OrderByDescending(x => x.CreatedAt))
        {
            var existingIndex = deduplicatedResults
                .FindIndex(x =>
                    (x.Blob != null && result.Blob != null && x.Blob?.Hash == result.Blob.Hash) ||
                    (x.IsQuestion == result.IsQuestion && x.FullContext.AreSimilar(result.FullContext, similarityThreshold)));

            if (existingIndex == -1)
            {
                deduplicatedResults
                    .Add(result);
            }
            else
            {
                var similarResult = deduplicatedResults[existingIndex];

                var newestResult = result.CreatedAt > similarResult.CreatedAt 
                    ? result 
                    : similarResult;

                var olderResult = newestResult == result 
                    ? similarResult 
                    : result;

                newestResult.CounterpartContext = newestResult.CounterpartContext
                    .Concat(olderResult.CounterpartContext)
                    .Distinct()
                    .ToArray();

                if (newestResult != similarResult)
                {
                    deduplicatedResults[existingIndex] = newestResult;
                }
            }
        }

        // Collapse Q/A pairs.
        var collapsedResults = new List<MemoryResult>(deduplicatedResults);
        foreach (var question in deduplicatedResults.Where(x => x.IsQuestion))
        {
            foreach (var answer in deduplicatedResults.Where(x => x.IsAnswer))
            {
                var isQuestionSimilar = answer.CounterpartContext is { Length: 1 } && question.FullContext
                    .AreSimilar(answer.CounterpartContext[0], similarityThreshold);

                var isAnswerSimilar = question.CounterpartContext is { Length: 1 } && answer.FullContext
                    .AreSimilar(question.CounterpartContext[0], similarityThreshold);

                if (isQuestionSimilar && isAnswerSimilar)
                {
                    var mergedAnswers = question.CounterpartContext
                        .ToList();
                    
                    if (!mergedAnswers.Contains(answer.FullContext))
                    {
                        mergedAnswers
                            .Add(answer.FullContext);
                    }

                    question.CounterpartContext = mergedAnswers
                        .ToArray();

                    var matchingAnswerCount = deduplicatedResults
                        .Count(x => x.IsAnswer && x.FullContext.Equals(answer.FullContext));

                    if (matchingAnswerCount == 1)
                    {
                        collapsedResults
                            .Remove(answer);
                    }
                }
            }
        }

        return collapsedResults
            .ToArray();
    }

    internal static KnowledgeResult[] DeduplicateKnowledgeResults(KnowledgeResult[] knowledgeResults, double matchThreshold = 0.90)
    {
        if (knowledgeResults == null)
            throw new ArgumentNullException(nameof(knowledgeResults));

        var usedIndexes = new HashSet<int>();

        for (var i = 0; i < knowledgeResults.Length; i++)
        {
            if (usedIndexes.Contains(i))
            {
                continue;
            }

            var current = knowledgeResults[i];

            for (var j = i + 1; j < knowledgeResults.Length; j++)
            {
                if (usedIndexes.Contains(j))
                {
                    continue;
                }

                var compare = knowledgeResults[j];

                var isSameBlob = current.Blob != null && compare.Blob != null && compare.Blob.Hash == current.Blob.Hash;
                var similarity = isSameBlob 
                    ? 1.00D 
                    : (double)Fuzz.Ratio(current.FullContext, compare.FullContext) / 100;

                if (similarity >= matchThreshold)
                {
                    var newer = current.CreatedAt >= compare.CreatedAt
                        ? current
                        : compare;

                    var olderIndex = newer == current
                        ? j
                        : i;

                    usedIndexes
                        .Add(olderIndex);
                }
            }
        }

        return knowledgeResults
            .Where((_, index) => !usedIndexes.Contains(index))
            .ToArray();
    }
}