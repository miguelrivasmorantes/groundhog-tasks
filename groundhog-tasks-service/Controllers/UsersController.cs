using groundhog_tasks_service.Api.DTOs;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsEmailVerified = u.IsEmailVerified
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsEmailVerified = u.IsEmailVerified
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password ?? "",
                IsActive = dto.IsActive ?? true,
                IsEmailVerified = dto.IsEmailVerified ?? false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            dto.Id = user.Id;
            dto.Password = null;

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = dto.Password;
            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;
            if (dto.IsEmailVerified.HasValue)
                user.IsEmailVerified = dto.IsEmailVerified.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{userId}/assignments")]
        public async Task<IActionResult> GetUserAssignments(Guid userId)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!exists) return NotFound();

            var assignments = await _context.UserAssignments
                .Where(ua => ua.UserId == userId)
                .Select(ua => new
                {
                    ua.Assignment.Id,
                    ua.Assignment.Name,
                    ua.Assignment.Description,
                    ua.Status,
                    ua.AssignedAt,
                    ua.UpdatedAt
                })
                .ToListAsync();

            return Ok(assignments);
        }
    }
}
