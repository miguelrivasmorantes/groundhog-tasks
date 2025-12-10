using System;

namespace GroundhogTasksService.Data.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public bool Problems { get; set; } = false;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
