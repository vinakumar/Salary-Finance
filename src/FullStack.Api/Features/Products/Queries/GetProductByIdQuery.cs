using FullStack.Domain.Common;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Queries;

/// <summary>
/// Query to get a single product by its identifier.
/// </summary>
/// <param name="Id">The product identifier.</param>
public record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse>>;
