using System;
using NpgsqlTypes;

namespace GroundhogTasksService.Data.Entities
{
    public enum UserAssignmentStatus
    {
        [PgName("overdue")]
        overdue,

        [PgName("completed")]
        completed,

        [PgName("pending")]
        pending,

        [PgName("done_incomplete")]
        doneIncomplete
    }

    public class UserAssignment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public UserAssignmentStatus Status { get; set; } = UserAssignmentStatus.pending;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
