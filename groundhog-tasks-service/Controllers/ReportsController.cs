using groundhog_tasks_service.Api.DTOs;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace groundhog_tasks_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReports()
        {
            var reports = await _context.Reports
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    AssignmentId = r.AssignmentId,
                    Problems = r.Problems,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDto>> GetReport(Guid id)
        {
            var report = await _context.Reports
                .Where(r => r.Id == id)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    AssignmentId = r.AssignmentId,
                    Problems = r.Problems,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (report == null)
                return NotFound("Report not found.");

            return Ok(report);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReportsByUser(Guid userId)
        {
            var reports = await _context.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    AssignmentId = r.AssignmentId,
                    Problems = r.Problems,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            if (!reports.Any())
                return NotFound("No reports found for this user.");

            return Ok(reports);
        }

        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReportsByAssignment(Guid assignmentId)
        {
            var reports = await _context.Reports
                .Where(r => r.AssignmentId == assignmentId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    AssignmentId = r.AssignmentId,
                    Problems = r.Problems,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            if (!reports.Any())
                return NotFound("No reports found for this assignment.");

            return Ok(reports);
        }

        [HttpPost]
        public async Task<ActionResult<ReportDto>> CreateReport([FromBody] ReportDto dto)
        {
            var assignmentExists = await _context.Assignments
                .AnyAsync(a => a.Id == dto.AssignmentId);

            if (!assignmentExists)
            {
                return BadRequest("The assignment was not found.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return BadRequest("The user was not found.");
            }

            var isUserAssigned = await _context.UserAssignments
                .AnyAsync(ua => ua.UserId == dto.UserId && ua.AssignmentId == dto.AssignmentId);

            if (!isUserAssigned)
            {
                return Forbid("The user is not assigned to this task and cannot create a report about it.");
            }

            var report = new Report
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId,
                Problems = dto.Problems,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            dto.Id = report.Id;
            dto.CreatedAt = report.CreatedAt;

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] ReportDto dto)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
                return NotFound("Report not found.");

            report.Problems = dto.Problems;
            report.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reports.Any(e => e.Id == id))
                {
                    return NotFound("Report not found after update check.");
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound("Report not found.");

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}