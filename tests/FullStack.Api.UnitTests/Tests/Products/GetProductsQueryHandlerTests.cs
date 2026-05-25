using FluentAssertions;
using FullStack.Api.Features.Products.Queries;
using FullStack.Domain.Contracts;
using FullStack.Domain.Entities;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;
using Moq;

namespace FullStack.Api.UnitTests.Tests.Products;

[TestFixture]
public class GetProductsQueryHandlerTests
{
    private Mock<IProductRepository> _repositoryMock = null!;
    private GetProductsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductsQueryHandler(_repositoryMock.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedProducts_WhenProductsExist()
    {
        var products = new PagedResponse<Product>
        {
            Items = new List<Product>
            {
                new() { Id = 1, Name = "Test", Price = 10m, Description = "Desc", CategoryId = 1, Category = new ProductCategory(1, "Cat1", "cat1"), CreatedAt = DateTimeOffset.UtcNow, IsActive = true }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 20
        };
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _handler.Handle(new GetProductsQuery(new PagedRequest()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
    }

    [Test]
    public async Task Handle_ReturnsEmptyPage_WhenNoProducts()
    {
        var empty = new PagedResponse<Product> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 20 };
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(empty);

        var result = await _handler.Handle(new GetProductsQuery(new PagedRequest()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Test]
    public async Task Handle_MapsProductResponseCorrectly()
    {
        var category = new ProductCategory(5, "Electronics", "electronics");
        var product = new Product { Id = 42, Name = "Widget", Price = 99.99m, Description = "A widget", CategoryId = 5, Category = category, CreatedAt = DateTimeOffset.Parse("2024-01-01T00:00:00Z"), IsActive = true };
        var paged = new PagedResponse<Product> { Items = [product], TotalCount = 1, PageNumber = 1, PageSize = 20 };
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _handler.Handle(new GetProductsQuery(new PagedRequest()), CancellationToken.None);

        var item = result.Value!.Items[0];
        item.Id.Should().Be(42);
        item.Name.Should().Be("Widget");
        item.Price.Should().Be(99.99m);
        item.CategoryId.Should().Be(5);
        item.CategoryName.Should().Be("Electronics");
    }
}
