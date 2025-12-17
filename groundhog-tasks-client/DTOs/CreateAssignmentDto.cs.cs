namespace groundhog_tasks_client.DTOs
{
    public class CreateAssignmentDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now;
        public int Cycles { get; set; } = 1;
        public string Periodicity { get; set; } = "00:00:00";

        // IDs necesarios para la relación
        public Guid CreatorUserId { get; set; }
        public Guid GroupId { get; set; }

        // La lista de usuarios asignados
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }

    // DTO auxiliar para listar los miembros del grupo en los checkboxes
    public class GroupMemberDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}