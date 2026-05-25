using FluentAssertions;
using FullStack.Api.Features.Products.Commands;
using FullStack.Domain.Common;
using FullStack.Domain.Contracts;
using FullStack.Domain.Entities;
using Moq;

namespace FullStack.Api.UnitTests.Tests.Products;

[TestFixture]
public class DeleteProductCommandHandlerTests
{
    private Mock<IProductRepository> _repositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private DeleteProductCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteProductCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_ReturnsSuccess_WhenProductExists()
    {
        var product = new Product { Id = 1, Name = "Test", Price = 10m, CategoryId = 1, Category = new ProductCategory(1, "Cat", "cat"), IsActive = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteProductCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ReturnsNotFound_WhenProductDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var result = await _handler.Handle(new DeleteProductCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
    }
}
