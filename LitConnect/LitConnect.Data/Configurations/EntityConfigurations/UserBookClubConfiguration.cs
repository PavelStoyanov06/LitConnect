namespace LitConnect.Data.Configurations.EntityConfigurations;

using LitConnect.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class UserBookClubConfiguration
{
    public static void ConfigureUserBookClub(this ModelBuilder builder)
    {
        const string adminUserId = "admin_user_id";

        builder.Entity<UserBookClub>().HasData(
            new UserBookClub
            {
                UserId = adminUserId,
                BookClubId = "club_1",
                JoinedOn = DateTime.UtcNow,
                IsAdmin = true
            },
            new UserBookClub
            {
                UserId = adminUserId,
                BookClubId = "club_2",
                JoinedOn = DateTime.UtcNow,
                IsAdmin = true
            }
        );
    }
}