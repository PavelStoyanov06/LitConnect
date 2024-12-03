namespace LitConnect.Data.Configurations.EntityConfigurations;

using LitConnect.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class BookClubConfiguration
{
    public static void ConfigureBookClub(this ModelBuilder builder)
    {
        const string adminUserId = "admin_user_id";

        builder.Entity<BookClub>().HasData(
            new BookClub
            {
                Id = "club_1",
                Name = "Fantasy Lovers",
                Description = "A club for fantasy book enthusiasts",
                OwnerId = adminUserId,
                IsDeleted = false
            },
            new BookClub
            {
                Id = "club_2",
                Name = "Mystery Solvers",
                Description = "Join us to solve literary mysteries",
                OwnerId = adminUserId,
                IsDeleted = false
            }
        );
    }
}