namespace groundhog_tasks_client.DTOs
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }
        public string Name{ get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public string GroupName { get; set; } = "";
    }
}