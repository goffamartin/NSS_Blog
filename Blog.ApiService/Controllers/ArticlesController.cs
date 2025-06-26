using Blog.ApiService.Services;
using Blog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticlesController(IArticleService service, IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await service.GetAllAsync();
        return Ok(items);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ArticleDto dto)
    {
        var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUser = await userService.GetByExternalIdAsync(externalId);
        if (currentUser is null) return Unauthorized();

        dto.AuthorId = currentUser.Id;

        var result = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ArticleDto dto)
    {
        var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUser = await userService.GetByExternalIdAsync(externalId);
        if (currentUser is null) return Unauthorized();

        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var isAuthor = existing.AuthorId == currentUser.Id;

        if (!isAuthor)
            return Forbid();

        var success = await service.UpdateAsync(id, dto);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUser = await userService.GetByExternalIdAsync(externalId);
        if (currentUser is null) return Unauthorized();

        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var isAuthor = existing.AuthorId == currentUser.Id;
        var isAdmin = User.IsInRole("Admin");

        if (!isAuthor && !isAdmin)
            return Forbid();

        var success = await service.SoftDeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
