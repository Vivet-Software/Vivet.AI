using System;
using Vivet.AI.Services.Models.Blobs;

namespace Vivet.AI.Services.Requests.Vision;

/// <summary>
/// Represents a video images extraction request.
/// </summary>
public class VideoImageExtractionRequest : BaseImagesExtractionRequest<VideoBlob>
{
    /// <summary>
    /// The interval in which frames in the video will be snapshotted and outputted as images.
    /// </summary>
    public virtual TimeSpan FrameInterval { get; set; } = TimeSpan.FromSeconds(1);
}