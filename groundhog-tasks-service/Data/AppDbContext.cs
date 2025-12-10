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
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserGroup>().ToTable("user_group");

            modelBuilder.Entity<PermissionRole>().ToTable("permission_role");

            modelBuilder.HasPostgresEnum<UserAssignmentStatus>("user_assignment_status");

            modelBuilder.Entity<UserAssignment>(entity =>
            {
                entity.ToTable("user_assignment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Status)
                    .HasColumnType("user_assignment_status")
                    .HasDefaultValue(UserAssignmentStatus.pending);
            });


            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.IsEnum) continue;

                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(ToSnakeCase(property.Name));

                foreach (var key in entity.GetKeys())
                    key.SetName(ToSnakeCase(key.GetName()!));

                foreach (var index in entity.GetIndexes())
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));

                foreach (var fk in entity.GetForeignKeys())
                    fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));
            }

        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    if (i > 0) builder.Append('_');
                    builder.Append(char.ToLowerInvariant(input[i]));
                }
                else
                {
                    builder.Append(input[i]);
                }
            }
            return builder.ToString();
        }
    }
}
