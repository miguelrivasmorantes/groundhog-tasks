public class AssignmentDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Cycles { get; set; } = 1;
    public string? Periodicity { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public Guid? CreatorUserId { get; set; }
    public Guid? GroupId { get; set; }
    public List<Guid> UserIds { get; set; } = new List<Guid>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
