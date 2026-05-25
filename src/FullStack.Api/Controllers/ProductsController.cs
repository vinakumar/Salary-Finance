using Asp.Versioning;
using FullStack.Api.Features.Products.Commands;
using FullStack.Api.Features.Products.Queries;
using FullStack.Domain.Common;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Controllers;

/// <summary>
/// API controller for managing products.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a paged list of products.
    /// </summary>
    /// <param name="request">Paging and sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProducts([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductsQuery(request), ct);
        return ToActionResult(result);
    }

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product details.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProduct(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Description = request.Description
        };

        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetProduct), new { id = result.Value!.Id }, result.Value);
        }

        return ToActionResult(result);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description
        };

        var result = await mediator.Send(command, ct);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), ct);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToActionResult(result);
    }

    private IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var problemDetails = new ProblemDetails
        {
            Title = result.ErrorType.ToString(),
            Detail = result.Error,
            Status = result.ErrorType switch
            {
                ResultErrorType.NotFound => StatusCodes.Status404NotFound,
                ResultErrorType.Conflict => StatusCodes.Status409Conflict,
                ResultErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                ResultErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            }
        };

        return StatusCode(problemDetails.Status!.Value, problemDetails);
    }
}
