using Blog.Shared.Dtos;
using Blog.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _service;

        public LikesController(ILikeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] LikeDto dto)
        {
            var added = await _service.ToggleLikeAsync(dto);
            return added ? Ok("Liked") : Ok("Unliked");
        }

        [HttpGet("{articleId}")]
        public async Task<IActionResult> GetLikeCount(int articleId)
        {
            var count = await _service.GetLikeCountAsync(articleId);
            return Ok(count);
        }
    }
}
