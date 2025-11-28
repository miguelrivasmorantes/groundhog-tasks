using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using groundhog_tasks_service.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all permissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            var permissions = await _context.Permissions
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Key = p.Key,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();

            return Ok(permissions);
        }

        // GET permission by id
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(Guid id)
        {
            var permission = await _context.Permissions
                .Where(p => p.Id == id)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Key = p.Key,
                    Name = p.Name,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        // GET roles that have this permission
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRolesWithPermission(Guid id)
        {
            var permissionExists = await _context.Permissions.AnyAsync(p => p.Id == id);
            if (!permissionExists)
                return NotFound("Permission not found.");

            var roles = await _context.PermissionRoles
                .Where(pr => pr.PermissionId == id)
                .Include(pr => pr.Role)
                .Select(pr => new RoleDto
                {
                    Id = pr.Role.Id,
                    Name = pr.Role.Name,
                    Description = pr.Role.Description,
                    PermissionIds = pr.Role.PermissionRoles.Select(rp => rp.PermissionId).ToList()
                })
                .ToListAsync();

            return Ok(roles);
        }

        // POST create a new permission
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> CreatePermission(PermissionDto dto)
        {
            if (await _context.Permissions.AnyAsync(p => p.Name == dto.Name))
                return BadRequest("Permission name already exists.");

            if (await _context.Permissions.AnyAsync(p => p.Key == dto.Key))
                return BadRequest("Permission key already exists.");

            var permission = new Permission
            {
                Key = dto.Key,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            dto.Id = permission.Id;
            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, dto);
        }

        // PUT update permission
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(Guid id, PermissionDto dto)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);

            if (permission == null)
                return NotFound();

            // Validar duplicados
            if (await _context.Permissions.AnyAsync(p => p.Id != id && p.Name == dto.Name))
                return BadRequest("Permission name already exists.");

            if (await _context.Permissions.AnyAsync(p => p.Id != id && p.Key == dto.Key))
                return BadRequest("Permission key already exists.");

            permission.Key = dto.Key;
            permission.Name = dto.Name;
            permission.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
