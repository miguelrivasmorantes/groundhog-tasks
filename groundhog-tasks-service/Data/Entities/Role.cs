using System;
using System.Collections.Generic;

namespace GroundhogTasksService.Data.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }
}
