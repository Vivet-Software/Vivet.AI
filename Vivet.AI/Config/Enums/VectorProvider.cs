namespace Vivet.AI.Config.Enums;

/// <summary>
/// Vector Provider.
/// </summary>
public enum VectorProvider
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Qdrant.
    /// </summary>
    Qdrant = 1,

    /// <summary>
    /// Azure Ai Search.
    /// </summary>
    AzureAiSearch = 2,

    /// <summary>
    /// Pinecone.
    /// </summary>
    Pinecone = 3,

    /// <summary>
    /// Postgres (pgVector).
    /// </summary>
    Postgres = 4,

    /// <summary>
    /// Weaviate.
    /// </summary>
    Weaviate = 5
}