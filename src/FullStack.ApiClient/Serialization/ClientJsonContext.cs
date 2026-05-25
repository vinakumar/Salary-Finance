using System.Text.Json.Serialization;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;

namespace FullStack.ApiClient.Serialization;

/// <summary>
/// System.Text.Json source generator context for the API client.
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
internal partial class ClientJsonContext : JsonSerializerContext
{
}
