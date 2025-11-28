namespace groundhog_tasks_service.Api.DTOs
{
    public class UserGroupDto
    {
        public Guid? Id { get; set; } 
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid RoleId { get; set; }
    }
}
