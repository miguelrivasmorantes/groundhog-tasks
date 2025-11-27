namespace GroundhogTasksService.Data.Entities
{
	public enum UserAssignmentStatus { Overdue, Completed, Pending, DoneIncomplete }

	public class UserAssignment
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int AssignmentId { get; set; }
		public UserAssignmentStatus Status { get; set; }
	}
}