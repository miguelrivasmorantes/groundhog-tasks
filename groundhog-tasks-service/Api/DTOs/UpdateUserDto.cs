using System;

namespace groundhog_tasks_service.Api.DTOs
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool? IsEmailVerified { get; set; }
        public bool? IsActive { get; set; }
    }
}
