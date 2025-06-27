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
    public class LikesController(ILikeService service, IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] LikeDto dto)
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await userService.GetByExternalIdAsync(externalId!);
            if (currentUser is null) return Unauthorized();

            dto.UserId = currentUser.Id;
            var added = await service.ToggleLikeAsync(dto);
            return Ok(added ? "Liked" : "Unliked");
        }

        [AllowAnonymous]
        [HttpGet("{articleId}")]
        public async Task<IActionResult> GetLikeCount(int articleId)
        {
            var count = await service.GetLikeCountAsync(articleId);
            return Ok(count);
        }
    }

}
