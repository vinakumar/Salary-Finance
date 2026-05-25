using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Queries;

/// <summary>
/// Handler for <see cref="GetProductsQuery"/>.
/// </summary>
public class GetProductsQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductsQuery, Result<PagedResponse<ProductResponse>>>
{
    /// <inheritdoc/>
    public async Task<Result<PagedResponse<ProductResponse>>> Handle(
        GetProductsQuery query, CancellationToken ct)
    {
        var pagedProducts = await repository.GetPagedAsync(query.Request, ct);

        var response = new PagedResponse<ProductResponse>
        {
            Items = pagedProducts.Items.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                CategoryId = p.Category.Id,
                CategoryName = p.Category.Name,
                CreatedAt = p.CreatedAt,
                IsActive = p.IsActive
            }).ToList(),
            TotalCount = pagedProducts.TotalCount,
            PageNumber = pagedProducts.PageNumber,
            PageSize = pagedProducts.PageSize
        };

        return Result<PagedResponse<ProductResponse>>.Success(response);
    }
}
