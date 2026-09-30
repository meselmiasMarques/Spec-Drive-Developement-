using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Api.Infrastructure;
using WeMoney.Application.Categories;

namespace WeMoney.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/categories")]
public class CategoriesController(CategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<CategoryResponse>> List(CancellationToken ct) =>
        await categoryService.ListAsync(User.GetUserId(), ct);

    [HttpGet("{id:guid}")]
    public async Task<CategoryResponse> Get(Guid id, CancellationToken ct) =>
        await categoryService.GetAsync(id, User.GetUserId(), ct);

    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CategoryRequest request, CancellationToken ct)
    {
        var category = await categoryService.CreateAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<CategoryResponse> Update(Guid id, CategoryRequest request, CancellationToken ct) =>
        await categoryService.UpdateAsync(id, User.GetUserId(), request, ct);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await categoryService.DeleteAsync(id, User.GetUserId(), ct);
        return NoContent();
    }
}
