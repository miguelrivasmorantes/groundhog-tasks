namespace GroundhogTasksService.Data.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}