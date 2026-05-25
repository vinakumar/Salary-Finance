using FullStack.Domain.Common;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Queries;

/// <summary>
/// Query to get a paged list of products.
/// </summary>
public record GetProductsQuery(PagedRequest Request) : IRequest<Result<PagedResponse<ProductResponse>>>;
