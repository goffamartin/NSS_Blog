using Blog.ApiService.Services;
using Blog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController(ICommentService service, IUserService userService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int articleId)
        {
            var items = await service.GetByArticleIdAsync(articleId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentDto dto)
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await userService.GetByExternalIdAsync(externalId!);
            if (currentUser is null) return Unauthorized();

            dto.AuthorId = currentUser.Id;
            var result = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { articleId = dto.ArticleId }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await userService.GetByExternalIdAsync(externalId!);
            if (currentUser is null) return Unauthorized();

            var comment = await service.GetByIdAsync(id);
            if (comment is null) return NotFound();

            var isAuthor = comment.AuthorId == currentUser.Id;
            var isAdmin = User.IsInRole("Admin");

            if (!isAuthor && !isAdmin)
                return Forbid();

            var result = await service.SoftDeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentDto dto)
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await userService.GetByExternalIdAsync(externalId!);
            if (currentUser is null) return Unauthorized();

            var comment = await service.GetByIdAsync(id);
            if (comment is null) return NotFound();

            var isAuthor = comment.AuthorId == currentUser.Id;

            if (!isAuthor)
                return Forbid();

            dto.Id = id;
            dto.AuthorId = comment.AuthorId;
            dto.ArticleId = comment.ArticleId;
            var updated = await service.UpdateAsync(dto);
            return Ok(updated);
        }
    }
}
