using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Responses.Embeddings.Knowledge.Models;
using Vivet.AI.Services.Responses.Embeddings.Memory.Models;

namespace Vivet.AI.Services.Helpers;

internal static class ContextDeduplicator
{
    public static SearchMemoryResult[] DeduplicateMemoryResults(IEnumerable<SearchMemoryResult> results, double similarityThreshold = 0.90)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results));

        var deduplicatedResults = new List<SearchMemoryResult>();

        // Deduplicate results.
        foreach (var result in results.OrderByDescending(x => x.Result.CreatedAt))
        {
            var existingIndex = deduplicatedResults
                .FindIndex(x =>
                    (x.Result.Blob != null && result.Result.Blob != null && x.Result.Blob?.Hash == result.Result.Blob.Hash) ||
                    (x.Result.IsQuestion == result.Result.IsQuestion && x.Result.FullContext.AreSimilar(result.Result.FullContext, similarityThreshold)));

            if (existingIndex == -1)
            {
                deduplicatedResults
                    .Add(result);
            }
            else
            {
                var similarResult = deduplicatedResults[existingIndex];

                var newestResult = result.Result.CreatedAt > similarResult.Result.CreatedAt 
                    ? result 
                    : similarResult;

                var olderResult = newestResult == result 
                    ? similarResult 
                    : result;

                newestResult.Result.CounterpartContext = newestResult.Result.CounterpartContext
                    .Concat(olderResult.Result.CounterpartContext)
                    .Distinct()
                    .ToArray();

                if (newestResult != similarResult)
                {
                    deduplicatedResults[existingIndex] = newestResult;
                }
            }
        }

        // Collapse Q/A pairs.
        var collapsedResults = new List<SearchMemoryResult>(deduplicatedResults);
        foreach (var question in deduplicatedResults.Where(x => x.Result.IsQuestion))
        {
            foreach (var answer in deduplicatedResults.Where(x => x.Result.IsAnswer))
            {
                var isQuestionSimilar = answer.Result.CounterpartContext is { Length: 1 } && question.Result.FullContext
                    .AreSimilar(answer.Result.CounterpartContext[0], similarityThreshold);

                var isAnswerSimilar = question.Result.CounterpartContext is { Length: 1 } && answer.Result.FullContext
                    .AreSimilar(question.Result.CounterpartContext[0], similarityThreshold);

                if (isQuestionSimilar && isAnswerSimilar)
                {
                    var mergedAnswers = question.Result.CounterpartContext
                        .ToList();
                    
                    if (!mergedAnswers.Contains(answer.Result.FullContext))
                    {
                        mergedAnswers
                            .Add(answer.Result.FullContext);
                    }

                    question.Result.CounterpartContext = mergedAnswers
                        .ToArray();

                    var matchingAnswerCount = deduplicatedResults
                        .Count(x => x.Result.IsAnswer && x.Result.FullContext.Equals(answer.Result.FullContext));

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

    internal static SearchKnowledgeResult[] DeduplicateKnowledgeResults(SearchKnowledgeResult[] knowledgeResults, double matchThreshold = 0.90)
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

                var isSameBlob = current.Result.Blob != null && compare.Result.Blob != null && compare.Result.Blob.Hash == current.Result.Blob.Hash;
                var similarity = isSameBlob 
                    ? 1.00D 
                    : (double)Fuzz.Ratio(current.Result.FullContext, compare.Result.FullContext) / 100;

                if (similarity >= matchThreshold)
                {
                    var newer = current.Result.CreatedAt >= compare.Result.CreatedAt
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