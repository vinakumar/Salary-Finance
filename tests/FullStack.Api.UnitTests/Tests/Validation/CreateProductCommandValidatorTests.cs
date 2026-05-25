using FluentAssertions;
using FluentValidation.TestHelper;
using FullStack.Api.Features.Products.Commands;

namespace FullStack.Api.UnitTests.Tests.Validation;

[TestFixture]
public class CreateProductCommandValidatorTests
{
    private CreateProductCommandValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Test]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        var command = new CreateProductCommand { Name = "", Price = 10m, CategoryId = 1 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void Validate_ShouldFail_WhenPriceIsZero()
    {
        var command = new CreateProductCommand { Name = "Valid", Price = 0m, CategoryId = 1 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Test]
    public void Validate_ShouldFail_WhenCategoryIdIsZero()
    {
        var command = new CreateProductCommand { Name = "Valid", Price = 10m, CategoryId = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Test]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var command = new CreateProductCommand { Name = "Valid Product", Price = 29.99m, CategoryId = 1, Description = "A desc" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
