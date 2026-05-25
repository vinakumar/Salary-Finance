using System.Text.Json.Serialization;
using FullStack.Domain.Catalog.Classification.Hierarchy.Taxonomy;
using FullStack.Domain.Models;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;

namespace FullStack.Api.Serialization;

/// <summary>
/// System.Text.Json source generator context for AOT-compatible serialization.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(CreateProductRequest))]
[JsonSerializable(typeof(UpdateProductRequest))]
[JsonSerializable(typeof(PagedRequest))]
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(PagedResponse<ProductResponse>))]
[JsonSerializable(typeof(ApiErrorDetail))]
[JsonSerializable(typeof(List<ApiErrorDetail>))]
[JsonSerializable(typeof(GenericPayload<ProductTaxonomyNode>))]
[JsonSerializable(typeof(ProductTaxonomyNode))]
[JsonSerializable(typeof(List<ProductTaxonomyNode>))]
[JsonSerializable(typeof(IReadOnlyList<ProductTaxonomyNode>))]
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
internal partial class ApiJsonContext : JsonSerializerContext
{
}
