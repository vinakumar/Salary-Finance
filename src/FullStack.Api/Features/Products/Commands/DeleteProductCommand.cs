using FullStack.Domain.Common;
using MediatR;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Command to delete a product by its identifier.
/// </summary>
/// <param name="Id">The product identifier to delete.</param>
public record DeleteProductCommand(int Id) : IRequest<Result<bool>>;
