namespace groundhog_tasks_service.Api.DTOs
{
    public class UserDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public bool? IsEmailVerified { get; set; }
        public bool? IsActive { get; set; }
    }
}
