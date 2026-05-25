using FluentAssertions;
using FullStack.Api.Features.Products.Commands;
using FullStack.Api.Infrastructure;
using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FullStack.Api.UnitTests.Tests.Products;

[TestFixture]
public class CreateProductCommandHandlerTests
{
    private Mock<IProductRepository> _repositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private AppDbContext _dbContext = null!;
    private CreateProductCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        // Ensure category 1 is seeded (AppDbContext seeds it via OnModelCreating)
        _handler = new CreateProductCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsSuccess_WhenProductIsCreated()
    {
        var command = new CreateProductCommand { Name = "New Product", Price = 25.00m, CategoryId = 1 };
        _repositoryMock.Setup(r => r.ExistsByNameAsync("New Product", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p with { Id = 99 });
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Product");
        result.Value.Price.Should().Be(25.00m);
    }

    [Test]
    public async Task Handle_ReturnsConflict_WhenNameAlreadyExists()
    {
        var command = new CreateProductCommand { Name = "Existing", Price = 10m, CategoryId = 1 };
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Existing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ResultErrorType.Conflict);
    }

    [Test]
    public async Task Handle_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        var command = new CreateProductCommand { Name = "Test", Price = 10m, CategoryId = 9999 };
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
    }
}
