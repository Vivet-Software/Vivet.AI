using Vivet.AI.Services.Exceptions;

namespace Vivet.AI.Services;

/// <summary>
/// Base Service.
/// </summary>
public abstract class BaseService
{
    /// <summary>
    /// Returns an thrown exception with the specified <paramref name="errorMessage"/>
    /// or null if <paramref name="errorMessage"/> is null.
    /// </summary>
    /// <param name="errorMessage">The error message. null if no error.</param>
    /// <returns>The <see cref="AiException"/> or null.</returns>
    protected internal static AiException GetResponseExceptionOrDefault(string errorMessage = null)
    {
        if (errorMessage == null)
        {
            return null;
        }

        try
        {
            throw new AiException(errorMessage);
        }
        catch (AiException ex)
        {
            return ex;
        }
    }
}