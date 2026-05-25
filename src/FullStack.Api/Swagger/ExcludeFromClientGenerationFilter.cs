using FullStack.Api.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FullStack.Api.Swagger;

/// <summary>
/// Swagger document filter that removes operations and schemas tagged with
/// <see cref="ExcludeFromClientGenerationAttribute"/>.
/// </summary>
public class ExcludeFromClientGenerationFilter : IDocumentFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathsToRemove = new List<string>();

        foreach (var path in swaggerDoc.Paths)
        {
            var operationsToRemove = new List<OperationType>();

            foreach (var operation in path.Value.Operations)
            {
                var apiDescription = context.ApiDescriptions
                    .FirstOrDefault(d => d.RelativePath != null &&
                        path.Key.TrimStart('/').Equals(d.RelativePath, StringComparison.OrdinalIgnoreCase));

                if (apiDescription?.ActionDescriptor?.EndpointMetadata
                    ?.Any(m => m is ExcludeFromClientGenerationAttribute) == true)
                {
                    operationsToRemove.Add(operation.Key);
                }
            }

            foreach (var op in operationsToRemove)
            {
                path.Value.Operations.Remove(op);
            }

            if (path.Value.Operations.Count == 0)
            {
                pathsToRemove.Add(path.Key);
            }
        }

        foreach (var path in pathsToRemove)
        {
            swaggerDoc.Paths.Remove(path);
        }
    }
}
