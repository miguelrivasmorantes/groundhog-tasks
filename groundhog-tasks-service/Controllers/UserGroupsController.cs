using groundhog_tasks_service.Api.DTOs;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId}/users")]
    public class GroupUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupUsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroupDto>>> GetUsersInGroup(Guid groupId)
        {
            if (!await _context.Groups.AnyAsync(g => g.Id == groupId))
                return NotFound("Group not found.");

            var users = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId)
                .Select(ug => new UserGroupDto
                {
                    Id = ug.Id,
                    UserId = ug.UserId,
                    GroupId = ug.GroupId,
                    RoleId = ug.RoleId
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserGroupDto>> GetUserInGroup(Guid groupId, Guid userId)
        {
            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            if (userGroup == null)
                return NotFound();

            return Ok(new UserGroupDto
            {
                Id = userGroup.Id,
                UserId = userGroup.UserId,
                GroupId = userGroup.GroupId,
                RoleId = userGroup.RoleId
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddUsersToGroup(Guid groupId, [FromBody] List<UserGroupDto> assignments)
        {
            if (!await _context.Groups.AnyAsync(g => g.Id == groupId))
                return NotFound("Group not found.");

            var userIds = assignments.Select(a => a.UserId).ToList();
            var roleIds = assignments.Select(a => a.RoleId).ToList();

            var existingUsers = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(u => u.Id).ToListAsync();
            var existingRoles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Id).ToListAsync();

            foreach (var assign in assignments)
            {
                if (!existingUsers.Contains(assign.UserId) || !existingRoles.Contains(assign.RoleId))
                    return BadRequest($"User or Role not found for UserId: {assign.UserId}, RoleId: {assign.RoleId}");

                if (!await _context.UserGroups.AnyAsync(ug => ug.GroupId == groupId && ug.UserId == assign.UserId))
                {
                    var userGroup = new UserGroup
                    {
                        GroupId = groupId,
                        UserId = assign.UserId,
                        RoleId = assign.RoleId
                    };
                    _context.UserGroups.Add(userGroup);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Users assigned to group successfully.");
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(Guid groupId, Guid userId)
        {
            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            if (userGroup == null)
                return NotFound();

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserRoleInGroup(Guid groupId, Guid userId, [FromBody] UserGroupDto dto)
        {
            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

            if (userGroup == null)
                return NotFound("User not found in group.");

            if (!await _context.Roles.AnyAsync(r => r.Id == dto.RoleId))
                return BadRequest("Role not found.");

            userGroup.RoleId = dto.RoleId;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
