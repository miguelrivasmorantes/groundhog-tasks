using Microsoft.EntityFrameworkCore;
using GroundhogTasksService.Data.Entities;

namespace GroundhogTasksService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<UserAssignment> UserAssignments { get; set; } = null!;
        public DbSet<UserGroup> UserGroups { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<PermissionRole> PermissionRoles { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
    }
}
