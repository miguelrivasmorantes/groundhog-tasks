using System;
using System.Collections.Generic;

namespace GroundhogTasksService.Data.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }
}
