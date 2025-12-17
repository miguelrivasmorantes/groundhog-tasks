using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using groundhog_tasks_service.Api.DTOs;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "The user does not exist." });
            }

            if (user.PasswordHash != loginDto.Password)
            {
                return Unauthorized(new { message = "Wrong password." });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Your account is deactivated." });
            }

            var sessionExpiration = DateTime.UtcNow.AddMinutes(15);

            return Ok(new
            {
                message = "Login exitoso",
                isAuthenticated = true,
                sessionExpiresAt = sessionExpiration,
                userData = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                }
            });
        }
    }
}