// Data/Entities/User.cs
namespace GroundhogTasksService.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}

// Data/Entities/Group.cs
namespace GroundhogTasksService.Data.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
