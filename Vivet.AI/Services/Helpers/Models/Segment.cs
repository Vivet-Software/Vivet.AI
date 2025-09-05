using Vivet.AI.Services.Helpers.Enums;

namespace Vivet.AI.Services.Helpers.Models;

internal class Segment
{
    public virtual SegmentType Type { get; set; }

    public virtual string Content { get; set; }
}