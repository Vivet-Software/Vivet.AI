using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;

namespace Vivet.AI.Services.Collectors.Models;

internal class ResponseCallbackCollector
{
    private readonly List<ResponseCallback> results = [];

    internal virtual IEnumerable<ResponseCallback> GetAll()
    {
        return this.results;
    }

    internal void AddResponseCallback(ChatMessageContent chatMessageContent, TimeSpan elapsedTime)
    {
        if (chatMessageContent == null)
            throw new ArgumentNullException(nameof(chatMessageContent));

        this.results
            .Add(new ResponseCallback
            {
                ChatMessageContent = chatMessageContent,
                ElapsedTime = elapsedTime
            });
    }
}