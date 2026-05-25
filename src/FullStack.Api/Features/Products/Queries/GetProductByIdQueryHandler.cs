using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Queries;

/// <summary>
/// Handler for <see cref="GetProductByIdQuery"/>.
/// </summary>
public class GetProductByIdQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    /// <inheritdoc/>
    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(query.Id, ct);

        if (product is null)
        {
            return Result<ProductResponse>.Failure(
                $"Product with id '{query.Id}' was not found.",
                ResultErrorType.NotFound);
        }

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            CreatedAt = product.CreatedAt,
            IsActive = product.IsActive
        };

        return Result<ProductResponse>.Success(response);
    }
}
