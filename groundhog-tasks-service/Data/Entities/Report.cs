namespace GroundhogTasksService.Data.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AssignmentId { get; set; }
        public string Problems { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}