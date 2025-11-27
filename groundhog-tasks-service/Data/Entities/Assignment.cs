namespace GroundhogTasksService.Data.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Cycles { get; set; }
        public TimeSpan Periodicity { get; set; }
        public DateTime StartDate { get; set; }
        public int CreatorId { get; set; }
        public int GroupId { get; set; }
    }
}