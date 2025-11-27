using System;
using System.Collections.Generic;

namespace GroundhogTasksService.Data.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Cycles { get; set; } = 1;
        public TimeSpan? Periodicity { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; } = null!;

        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserAssignment> UserAssignments { get; set; } = new List<UserAssignment>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
