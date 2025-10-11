using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using Vivet.AI.Services.Consts;
using Vivet.AI.Services.Exceptions;
using Vivet.AI.Services.Extensions;
using Vivet.AI.Services.Models;

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

    /// <summary>
    /// Get the response function calls from the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/>.</param>
    /// <returns>The functions calls from the kernel.</returns>
    protected internal static FunctionCall[] GetResponseFunctionCalls(Kernel kernel)
    {
        if (kernel == null)
            throw new ArgumentNullException(nameof(kernel));

        var value = (IEnumerable<AutoFunctionInvocationContext>)kernel.Data[KernelData.FUNCTION_CALLS];

        var functionCalls = value
            .Select(x => x.GetFunctionCall())
            .OrderBy(x => x.CreatedAt)
            .ToArray();

        return functionCalls;
    }
}