namespace groundhog_tasks_service.Api.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
