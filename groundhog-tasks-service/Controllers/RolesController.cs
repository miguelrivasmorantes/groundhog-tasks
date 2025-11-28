using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using groundhog_tasks_service.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Include(r => r.PermissionRoles)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    PermissionIds = r.PermissionRoles.Select(pr => pr.PermissionId).ToList()
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.PermissionRoles)
                .Where(r => r.Id == id)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    PermissionIds = r.PermissionRoles.Select(pr => pr.PermissionId).ToList()
                })
                .FirstOrDefaultAsync();

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto dto)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == dto.Name))
                return BadRequest("Role name already exists.");

            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            if (dto.PermissionIds != null && dto.PermissionIds.Count > 0)
            {
                var existingPermissions = await _context.Permissions
                    .Where(p => dto.PermissionIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                foreach (var pid in existingPermissions)
                {
                    _context.PermissionRoles.Add(new PermissionRole
                    {
                        RoleId = role.Id,
                        PermissionId = pid
                    });
                }
                await _context.SaveChangesAsync();
            }

            dto.Id = role.Id;
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, RoleDto dto)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound();

            if (await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id))
                return BadRequest("Role name already exists.");

            role.Name = dto.Name;
            role.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdateRolePermissions(Guid id, [FromBody] List<Guid> permissionIds)
        {
            var role = await _context.Roles
                .Include(r => r.PermissionRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound("Role not found.");

            var existingPermissions = await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            if (existingPermissions.Count != permissionIds.Count)
                return BadRequest("One or more permissions do not exist.");

            var toRemove = role.PermissionRoles
                .Where(pr => !existingPermissions.Contains(pr.PermissionId))
                .ToList();
            _context.PermissionRoles.RemoveRange(toRemove);

            var currentPermissionIds = role.PermissionRoles.Select(pr => pr.PermissionId).ToList();
            var toAdd = existingPermissions
                .Where(pid => !currentPermissionIds.Contains(pid))
                .Select(pid => new PermissionRole { RoleId = role.Id, PermissionId = pid });
            _context.PermissionRoles.AddRange(toAdd);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/permissions")]
        public async Task<IActionResult> RemovePermissionsFromRole(Guid id, [FromBody] List<Guid> permissionIds)
        {
            var role = await _context.Roles
                .Include(r => r.PermissionRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound("Role not found.");

            var toRemove = role.PermissionRoles
                .Where(pr => permissionIds.Contains(pr.PermissionId))
                .ToList();

            if (!toRemove.Any())
                return NotFound("None of the specified permissions are assigned to this role.");

            _context.PermissionRoles.RemoveRange(toRemove);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetRolePermissions(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.PermissionRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound("Role not found.");

            var permissionIds = role.PermissionRoles.Select(pr => pr.PermissionId).ToList();
            return Ok(permissionIds);
        }

        [HttpGet("{id}/permissions/{permissionId}")]
        public async Task<ActionResult<bool>> RoleHasPermission(Guid id, Guid permissionId)
        {
            if (!await _context.Roles.AnyAsync(r => r.Id == id))
                return NotFound("Role not found.");

            var exists = await _context.PermissionRoles
                .AnyAsync(pr => pr.RoleId == id && pr.PermissionId == permissionId);

            return Ok(exists);
        }
    }
}
