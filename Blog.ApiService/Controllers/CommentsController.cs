using Blog.ApiService.Dtos;
using Blog.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentsController(ICommentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int articleId)
        {
            var items = await _service.GetByArticleIdAsync(articleId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { articleId = dto.ArticleId }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
