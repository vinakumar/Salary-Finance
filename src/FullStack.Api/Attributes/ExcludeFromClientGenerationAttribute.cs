namespace FullStack.Api.Attributes;

/// <summary>
/// Marks an endpoint or model for exclusion from client generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ExcludeFromClientGenerationAttribute : Attribute
{
}
