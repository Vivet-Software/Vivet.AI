using System.Threading.Tasks;
using System.Threading;
using Vivet.AI.Services.Requests.Metadata;
using Vivet.AI.Services.Responses.Metadata;

namespace Vivet.AI.Services.Interfaces;

/// <summary>
/// Provides functionality for extracting structured metadata from binary blob content,
/// such as images, audio, video, or documents, using a chat completion model configured for metadta.
/// The service uses prompt templates to request metadata extraction from the AI model, optionally returning strongly-typed metadata results.
/// </summary>
public interface IMetadataService
{
    /// <summary>
    /// Extracts basic metadata (Summary, Description) from the provided blob.
    /// </summary>
    /// <param name="request">The metadata extraction request containing blob content and settings.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="MetadataResponse"/> containing the extracted metadata, token usage, and any error message.</returns>
    Task<MetadataResponse> GetAsync(GetMetadataRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts basic metadata (Summary, Description) from the provided blob,
    /// as well as additional metadata specified by the propeties of <typeparamref name="T"/>.
    /// Ensure that the properties are nullable, in case no metadata can be retrieved.
    /// </summary>
    /// <typeparam name="T">The type of the additional metadata object to deserialize into. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="request">The metadata extraction request containing blob content and settings.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="MetadataResponse{T}"/> containing the strongly-typed metadata, token usage, and any error message.</returns>
    Task<MetadataResponse<T>> GetAsync<T>(GetMetadataRequest request, CancellationToken cancellationToken = default)
        where T : class, new();
}