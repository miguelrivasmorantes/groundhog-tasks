using System.ComponentModel.DataAnnotations;

namespace groundhog_tasks_service.Api.DTOs
{
    public class ReportDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        public bool Problems { get; set; } = false;

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}