using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Models.Response;
using MediatR;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Handler for <see cref="UpdateProductCommand"/>.
/// </summary>
public class UpdateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    /// <inheritdoc/>
    public async Task<Result<ProductResponse>> Handle(
        UpdateProductCommand command, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(command.Id, ct);

        if (product is null)
        {
            return Result<ProductResponse>.Failure(
                $"Product with id '{command.Id}' was not found.",
                ResultErrorType.NotFound);
        }

        if (command.Name is not null && command.Name != product.Name)
        {
            if (await repository.ExistsByNameAsync(command.Name, ct))
            {
                return Result<ProductResponse>.Failure(
                    $"A product with name '{command.Name}' already exists.",
                    ResultErrorType.Conflict);
            }
        }

        var updated = product with
        {
            Name = command.Name ?? product.Name,
            Price = command.Price ?? product.Price,
            Description = command.Description ?? product.Description
        };

        await repository.UpdateAsync(updated, ct);
        await unitOfWork.CommitAsync(ct);

        var response = new ProductResponse
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price,
            Description = updated.Description,
            CategoryId = updated.Category.Id,
            CategoryName = updated.Category.Name,
            CreatedAt = updated.CreatedAt,
            IsActive = updated.IsActive
        };

        return Result<ProductResponse>.Success(response);
    }
}
