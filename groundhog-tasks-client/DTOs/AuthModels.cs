namespace groundhog_tasks_client.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime SessionExpiresAt { get; set; }
        public UserData UserData { get; set; }
    }

    public class UserData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}