using System;

namespace GroundhogTasksService.Data.Entities
{
    public enum UserAssignmentStatus
    {
        Overdue,
        Completed,
        Pending,
        DoneIncomplete
    }

    public class UserAssignment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public UserAssignmentStatus Status { get; set; } = UserAssignmentStatus.Pending;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
