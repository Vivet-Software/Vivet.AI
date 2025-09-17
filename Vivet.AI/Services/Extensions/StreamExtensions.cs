using System;
using System.IO;

namespace Vivet.AI.Services.Extensions;

internal static class StreamExtensions
{
    internal static string ToBase64(this Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var memoryStream = new MemoryStream();
        stream
            .CopyTo(memoryStream);
        
        var bytes = memoryStream.ToArray();

        return Convert.ToBase64String(bytes);
    }
}