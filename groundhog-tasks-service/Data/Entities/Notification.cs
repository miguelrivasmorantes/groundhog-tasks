using System;
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? AssignmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsSent { get; set; } = true;
    public string Type { get; set; } = string.Empty;
}