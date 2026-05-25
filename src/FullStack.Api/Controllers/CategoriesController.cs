using FullStack.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Controllers;

/// <summary>
/// API controller for product categories.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Gets all product categories.
    /// </summary>
    /// <returns>A list of categories.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await dbContext.Categories
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Slug })
            .ToListAsync(ct);

        return Ok(categories);
    }
}
