namespace LitConnect.Data.Configurations.EntityConfigurations;

using LitConnect.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class IdentityConfiguration
{
    public static void ConfigureIdentity(this ModelBuilder builder)
    {
        // Seed Roles
        var adminRoleId = "admin_role_id";
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        // Seed Admin User
        var adminUserId = "admin_user_id";
        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin@litconnect.com",
            NormalizedUserName = "ADMIN@LITCONNECT.COM",
            Email = "admin@litconnect.com",
            NormalizedEmail = "ADMIN@LITCONNECT.COM",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            SecurityStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");
        builder.Entity<ApplicationUser>().HasData(adminUser);

        // Link Admin User to Admin Role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            }
        );
    }
}