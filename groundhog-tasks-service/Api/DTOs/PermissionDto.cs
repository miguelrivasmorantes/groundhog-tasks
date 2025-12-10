using System;

namespace groundhog_tasks_service.Api.DTOs
{
    public class PermissionDto
    {
        public Guid? Id { get; set; }
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
