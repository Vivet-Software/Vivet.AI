using System;

namespace Vivet.AI.Services.Exceptions;

/// <summary>
/// Metadata Exception.
/// </summary>
public class AiException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public AiException(string message)
        : base(message)
    {
    }
}