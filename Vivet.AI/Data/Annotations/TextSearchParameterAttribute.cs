using System;

namespace Vivet.AI.Data.Annotations;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class TextSearchParameterAttribute : Attribute
{
    internal string Name { get; set; }

    internal string Description { get; set; }

    internal bool IsRequired { get; set; } = false;

    internal object DefaultValue { get; set; }
}