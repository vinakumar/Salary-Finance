using FullStack.Domain.Common;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Command to update an existing product.
/// </summary>
public record UpdateProductCommand : IRequest<Result<ProductResponse>>
{
    /// <summary>The product identifier.</summary>
    public int Id { get; init; }

    /// <summary>The updated product name.</summary>
    public string? Name { get; init; }

    /// <summary>The updated product price.</summary>
    public decimal? Price { get; init; }

    /// <summary>The updated product description.</summary>
    public string? Description { get; init; }
}
