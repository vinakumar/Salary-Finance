using FullStack.Domain.Common;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Command to create a new product.
/// </summary>
public record CreateProductCommand : IRequest<Result<ProductResponse>>
{
    /// <summary>The product name.</summary>
    public required string Name { get; init; }

    /// <summary>The product price.</summary>
    public required decimal Price { get; init; }

    /// <summary>The category identifier.</summary>
    public required int CategoryId { get; init; }

    /// <summary>The product description.</summary>
    public string Description { get; init; } = string.Empty;
}
