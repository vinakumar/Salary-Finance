using FullStack.Api.Infrastructure;
using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Entities;
using FullStack.Domain.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Features.Products.Commands;

/// <summary>
/// Handler for <see cref="CreateProductCommand"/>.
/// </summary>
public class CreateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    AppDbContext dbContext)
    : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    /// <inheritdoc/>
    public async Task<Result<ProductResponse>> Handle(
        CreateProductCommand command, CancellationToken ct)
    {
        if (await repository.ExistsByNameAsync(command.Name, ct))
        {
            return Result<ProductResponse>.Failure(
                $"A product with name '{command.Name}' already exists.",
                ResultErrorType.Conflict);
        }

        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == command.CategoryId, ct);
        if (category is null)
        {
            return Result<ProductResponse>.Failure(
                $"Category with id '{command.CategoryId}' was not found.",
                ResultErrorType.NotFound);
        }

        var product = new Product
        {
            Name = command.Name,
            Price = command.Price,
            Description = command.Description,
            CategoryId = command.CategoryId,
            Category = category,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        var created = await repository.AddAsync(product, ct);
        await unitOfWork.CommitAsync(ct);

        var response = new ProductResponse
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price,
            Description = created.Description,
            CategoryId = created.Category.Id,
            CategoryName = created.Category.Name,
            CreatedAt = created.CreatedAt,
            IsActive = created.IsActive
        };

        return Result<ProductResponse>.Success(response);
    }
}
