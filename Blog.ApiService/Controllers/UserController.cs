using Blog.ApiService.Services;
using Blog.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.ApiService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService service) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAllAsync());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await service.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentUser = await service.GetByExternalIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (currentUser == null || (currentUser.Id != user.Id && !User.IsInRole("Admin")))
                return Forbid();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            var result = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await service.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentUser = await service.GetByExternalIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (currentUser == null || (currentUser.Id != user.Id && !User.IsInRole("Admin")))
                return Forbid();

            var success = await service.SoftDeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await service.GetByExternalIdAsync(externalId);

            if (user == null)
            {
                user = new UserDto
                {
                    Username = User.Identity!.Name!,
                    Email = User.FindFirstValue(ClaimTypes.Email)!,
                    IdentityProviderExternalId = externalId,
                    Created = DateTime.UtcNow
                };

                user = await service.CreateAsync(user);
            }

            return Ok(new { user.Id, user.Username, user.Email });
        }
    }
}
