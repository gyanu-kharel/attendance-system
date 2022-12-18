using Attendance.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Attendance.DataAccess.Persistence.DbContext
{
    public class AttendanceDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options)
        {

        }

        // Register models here
        public virtual DbSet<AttendanceConfig> AttendanceConfigs { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<AttendanceDetail> AttendanceDetails { get; set; }

        // Method override
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            SeedMasterAttendanceConfig(builder);
            SeedUserRoles(builder, out Guid adminRoleId);
            SeedAdmin(builder, adminRoleId);

        }

        // Private methods
        private void SeedMasterAttendanceConfig(ModelBuilder builder)
        {
            AttendanceConfig config = new AttendanceConfig()
            {
                Id = Guid.NewGuid(),
                PunchInTime = new TimeSpan(10, 00, 00),
                PunchOutTime = new TimeSpan(18, 00, 00),
                IsActive = true
            };

            builder.Entity<AttendanceConfig>()
                .HasData(config);
        }

        private void SeedUserRoles(ModelBuilder builder, out Guid adminRoleId)
        {
            var adminRole = new IdentityRole<Guid>()
            {
                Id = Guid.NewGuid(),
                Name = "ADMIN",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = DateTime.UtcNow.Ticks.ToString(),
            };

            adminRoleId = adminRole.Id;

            var employeeRole = new IdentityRole<Guid>()
            {
                Id = Guid.NewGuid(),
                Name = "EMPLOYEE",
                NormalizedName = "EMPLOYEE",
                ConcurrencyStamp = DateTime.UtcNow.Ticks.ToString(),
            };

            builder.Entity<IdentityRole<Guid>>()
                .HasData(adminRole, employeeRole);
        }

        private void SeedAdmin(ModelBuilder builder, Guid adminRoleId)
        {
            var adminUser = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                Email = "admin@attendance.com",
                NormalizedEmail = "ADMIN@ATTENDANCE.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                SecurityStamp = DateTime.UtcNow.Ticks.ToString(),
            };

            PasswordHasher<ApplicationUser> hasher = new();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

            IdentityUserRole<Guid> adminRole = new()
            {
                RoleId = adminRoleId,
                UserId = adminUser.Id
            };

            builder.Entity<ApplicationUser>()
                .HasData(adminUser);
            
            builder.Entity<IdentityUserRole<Guid>>()
                .HasData(adminRole);
        }

    }
}
