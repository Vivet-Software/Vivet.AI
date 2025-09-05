using System;
using System.Threading.Tasks;
using Vivet.AI.Services.Models.Blobs.Consts;

namespace Vivet.AI.Services.Models.Blobs.Data;

/// <summary>
/// Represents the base class for blob data, providing methods to get Base64 and Data URI representations.
/// </summary>
public abstract class BaseBlobData
{
    internal abstract Task<string> GetBase64();

    internal string GetDataUri(string base64, string mimeType)
    {
        if (base64 == null)
            throw new ArgumentNullException(nameof(base64));

        if (mimeType == null)
            throw new ArgumentNullException(nameof(mimeType));

        return string.Format(BlobDataTemplates.BLOB_DATA_TEMPLATE, mimeType, base64);
    }
}