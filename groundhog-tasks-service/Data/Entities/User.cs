using System;
using System.Collections.Generic;

namespace GroundhogTasksService.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<UserAssignment> UserAssignments { get; set; } = new List<UserAssignment>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();
    }
}
