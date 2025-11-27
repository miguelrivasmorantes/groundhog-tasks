using System;
using System.Collections.Generic;

namespace GroundhogTasksService.Data.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
