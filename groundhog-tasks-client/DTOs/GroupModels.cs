namespace groundhog_tasks_client.DTOs
{
    public class UserGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string RoleName { get; set; } = "";
    }
}