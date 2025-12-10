using GroundhogTasksService.Data.Entities;
namespace groundhog_tasks_service.Api.DTOs

{
    public class UserAssignmentDto
    {
        public List<Guid> UserIds { get; set; } = new();
        public UserAssignmentStatus Status { get; set; } = UserAssignmentStatus.pending;
    }
}
