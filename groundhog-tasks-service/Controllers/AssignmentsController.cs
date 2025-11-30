using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using groundhog_tasks_service.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignments()
        {
            var assignments = await _context.Assignments
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Cycles = a.Cycles,
                    Periodicity = a.Periodicity.HasValue
                        ? a.Periodicity.Value.ToString(@"hh\:mm\:ss")
                        : null,
                    StartDate = a.StartDate,
                    CreatorUserId = a.CreatorUserId,
                    GroupId = a.GroupId
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id)
        {
            var assignment = await _context.Assignments
                .Where(a => a.Id == id)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Cycles = a.Cycles,
                    Periodicity = a.Periodicity.HasValue
                        ? a.Periodicity.Value.ToString(@"hh\:mm\:ss")
                        : null,
                    StartDate = a.StartDate,
                    CreatorUserId = a.CreatorUserId,
                    GroupId = a.GroupId
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentDto>> CreateAssignment([FromBody] AssignmentDto dto)
        {
            if (dto.Cycles < 1)
                return BadRequest("Cycles must be at least 1.");

            if (!dto.CreatorUserId.HasValue)
                return BadRequest("CreatorUserId is required.");

            if (!dto.GroupId.HasValue)
                return BadRequest("GroupId is required.");

            var creator = await _context.Users
                .Include(u => u.UserGroups)
                .FirstOrDefaultAsync(u => u.Id == dto.CreatorUserId.Value);
            if (creator == null)
                return BadRequest("Creator user does not exist.");

            var group = await _context.Groups
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == dto.GroupId.Value);
            if (group == null)
                return BadRequest("Group does not exist.");

            if (!group.UserGroups.Any(ug => ug.UserId == dto.CreatorUserId.Value))
                return BadRequest("Creator user does not belong to the group.");

            if (dto.UserIds.Any())
            {
                var usersInRequest = await _context.Users
                    .Where(u => dto.UserIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();
                if (usersInRequest.Count != dto.UserIds.Count)
                    return BadRequest("One or more assigned users do not exist.");

                var usersInGroup = await _context.UserGroups
                    .Where(ug => ug.GroupId == dto.GroupId.Value && dto.UserIds.Contains(ug.UserId))
                    .Select(ug => ug.UserId)
                    .ToListAsync();
                if (usersInGroup.Count != dto.UserIds.Count)
                    return BadRequest("All assigned users must belong to the group.");
            }

            TimeSpan? periodicity = null;
            if (!string.IsNullOrEmpty(dto.Periodicity))
            {
                if (!TimeSpan.TryParse(dto.Periodicity, out var ts))
                    return BadRequest("Invalid periodicity format. Use 'hh:mm:ss'.");
                periodicity = ts;
            }

            var assignment = new Assignment
            {
                Name = dto.Name,
                Description = dto.Description,
                Cycles = dto.Cycles,
                Periodicity = periodicity,
                StartDate = dto.StartDate,
                CreatorUserId = dto.CreatorUserId.Value,
                GroupId = dto.GroupId.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            if (dto.UserIds.Any())
            {
                var userAssignments = dto.UserIds.Select(uid => new UserAssignment
                {
                    UserId = uid,
                    AssignmentId = assignment.Id,
                    Status = UserAssignmentStatus.pending,
                    AssignedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                _context.UserAssignments.AddRange(userAssignments);
                await _context.SaveChangesAsync();
            }

            dto.Id = assignment.Id;
            dto.CreatedAt = assignment.CreatedAt;
            dto.UpdatedAt = assignment.UpdatedAt;

            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] AssignmentDto dto)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Group)
                .Include(a => a.Group.UserGroups)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
                return NotFound("Assignment not found.");

            if (dto.Cycles < 1)
                return BadRequest("Cycles must be at least 1.");

            if (dto.CreatorUserId.HasValue && dto.CreatorUserId.Value != assignment.CreatorUserId)
                return BadRequest("CreatorUserId cannot be edited.");

            if (dto.GroupId.HasValue && dto.GroupId.Value != assignment.GroupId)
            {
                var group = await _context.Groups
                    .Include(g => g.UserGroups)
                    .FirstOrDefaultAsync(g => g.Id == dto.GroupId.Value);
                if (group == null)
                    return BadRequest("Group does not exist.");

                if (!group.UserGroups.Any(ug => ug.UserId == assignment.CreatorUserId))
                    return BadRequest("Creator user does not belong to this new group.");

                assignment.GroupId = dto.GroupId.Value;
            }

            TimeSpan? periodicity = null;
            if (!string.IsNullOrEmpty(dto.Periodicity))
            {
                if (!TimeSpan.TryParse(dto.Periodicity, out var ts))
                    return BadRequest("Invalid periodicity format. Use 'hh:mm:ss'.");
                periodicity = ts;
            }

            assignment.Name = dto.Name ?? assignment.Name;
            assignment.Description = dto.Description ?? assignment.Description;
            assignment.Cycles = dto.Cycles;
            assignment.Periodicity = periodicity;
            assignment.StartDate = dto.StartDate != default ? dto.StartDate : assignment.StartDate;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            if (dto.UserIds.Any())
            {
                var existingAssignments = await _context.UserAssignments
                    .Where(ua => ua.AssignmentId == id)
                    .ToListAsync();

                var toRemove = existingAssignments
                    .Where(ua => !dto.UserIds.Contains(ua.UserId))
                    .ToList();
                _context.UserAssignments.RemoveRange(toRemove);

                var toAdd = dto.UserIds
                    .Where(uid => !existingAssignments.Any(e => e.UserId == uid))
                    .Select(uid => new UserAssignment
                    {
                        UserId = uid,
                        AssignmentId = id,
                        Status = UserAssignmentStatus.pending,
                        AssignedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    })
                    .ToList();

                _context.UserAssignments.AddRange(toAdd);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpPost("{assignmentId}/assign-users")]
        public async Task<IActionResult> AssignUsers(Guid assignmentId, [FromBody] UserAssignmentDto dto)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null)
                return NotFound("Assignment not found.");

            var users = await _context.Users
                .Where(u => dto.UserIds.Contains(u.Id))
                .ToListAsync();
            if (users.Count != dto.UserIds.Count)
                return BadRequest("One or more users do not exist.");

            var groupUsers = await _context.UserGroups
                .Where(ug => ug.GroupId == assignment.GroupId)
                .Select(ug => ug.UserId)
                .ToListAsync();
            if (dto.UserIds.Any(id => !groupUsers.Contains(id)))
                return BadRequest("Some users do not belong to the assignment's group.");

            var existingAssignments = await _context.UserAssignments
                .Where(ua => ua.AssignmentId == assignmentId &&
                             dto.UserIds.Contains(ua.UserId))
                .ToListAsync();

            UserAssignmentStatus statusEnum;
            if (!Enum.TryParse<UserAssignmentStatus>(dto.Status.ToString(), true, out statusEnum))
            {
                return BadRequest("Invalid status value.");
            }

            var newUserAssignments = dto.UserIds
                .Where(id => !existingAssignments.Any(e => e.UserId == id))
                .Select(id => new UserAssignment
                {
                    UserId = id,
                    AssignmentId = assignmentId,
                    Status = statusEnum,
                    AssignedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                })
                .ToList();

            _context.UserAssignments.AddRange(newUserAssignments);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Assigned = newUserAssignments.Count,
                AlreadyAssigned = existingAssignments.Count
            });
        }

        [HttpPost("{assignmentId}/unassign-users")]
        public async Task<IActionResult> UnassignUsers(Guid assignmentId, [FromBody] UserAssignmentDto dto)
        {
            var userAssignments = await _context.UserAssignments
                .Where(ua => ua.AssignmentId == assignmentId &&
                             dto.UserIds.Contains(ua.UserId))
                .ToListAsync();

            if (!userAssignments.Any())
                return NotFound("No matching user assignments found.");

            _context.UserAssignments.RemoveRange(userAssignments);
            await _context.SaveChangesAsync();

            return Ok(new { Unassigned = userAssignments.Count });
        }

        [HttpGet("{assignmentId}/users")]
        public async Task<ActionResult<IEnumerable<object>>> GetAssignedUsers(Guid assignmentId)
        {
            if (!await _context.Assignments.AnyAsync(a => a.Id == assignmentId))
                return NotFound("Assignment not found.");

            var result = await _context.UserAssignments
                .Where(ua => ua.AssignmentId == assignmentId)
                .Include(ua => ua.User)
                .Select(ua => new
                {
                    ua.UserId,
                    ua.Status,
                    ua.AssignedAt,
                    ua.UpdatedAt
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpPatch("{assignmentId}/users/{userId}/status")]
        public async Task<IActionResult> UpdateUserAssignmentStatus(Guid assignmentId, Guid userId, [FromBody] UserAssignmentDto dto)
        {
            var userAssignment = await _context.UserAssignments
                .FirstOrDefaultAsync(ua => ua.AssignmentId == assignmentId && ua.UserId == userId);

            if (userAssignment == null)
                return NotFound("User assignment not found.");

            userAssignment.Status = dto.Status;
            userAssignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
