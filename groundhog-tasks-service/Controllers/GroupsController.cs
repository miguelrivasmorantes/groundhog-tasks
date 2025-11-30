using groundhog_tasks_service.Api.DTOs;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
        {
            var groups = await _context.Groups
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                })
                .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(Guid id)
        {
            var group = await _context.Groups
                .Where(g => g.Id == id)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                })
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(GroupDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            dto.Id = group.Id;
            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(Guid id, GroupDto dto)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Name))
                group.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
                group.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{groupId}/assignments")]
        public async Task<IActionResult> GetGroupAssignments(Guid groupId)
        {
            var exists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!exists) return NotFound();

            var assignments = await _context.Assignments
                .Where(a => a.GroupId == groupId)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Description,
                    a.Cycles,
                    a.Periodicity,
                    a.StartDate
                })
                .ToListAsync();

            return Ok(assignments);
        }
    }
}
