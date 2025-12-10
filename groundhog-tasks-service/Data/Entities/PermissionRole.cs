using System;

namespace GroundhogTasksService.Data.Entities
{
    public class PermissionRole
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
