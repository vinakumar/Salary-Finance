using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using MediatR;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Handler for <see cref="DeleteProductCommand"/>.
/// </summary>
public class DeleteProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    /// <inheritdoc/>
    public async Task<Result<bool>> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(command.Id, ct);

        if (product is null)
        {
            return Result<bool>.Failure(
                $"Product with id '{command.Id}' was not found.",
                ResultErrorType.NotFound);
        }

        await repository.DeleteAsync(command.Id, ct);
        await unitOfWork.CommitAsync(ct);

        return Result<bool>.Success(true);
    }
}
