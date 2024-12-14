using LitConnect.Data.Models;
using LitConnect.Data;
using LitConnect.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

[TestFixture]
public class BookClubServiceTests : IDisposable
{
    private DbContextOptions<LitConnectDbContext> dbOptions = null!;
    private LitConnectDbContext dbContext = null!;
    private BookClubService bookClubService = null!;
    private bool isDisposed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        dbOptions = new DbContextOptionsBuilder<LitConnectDbContext>()
            .UseInMemoryDatabase($"LitConnectBookClubTestDb_{Guid.NewGuid()}")
            .Options;
    }

    [SetUp]
    public async Task Setup()
    {
        dbContext = new LitConnectDbContext(dbOptions);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        bookClubService = new BookClubService(dbContext);

        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.BookClubs.RemoveRange(dbContext.BookClubs);
        dbContext.UsersBookClubs.RemoveRange(dbContext.UsersBookClubs);
        await dbContext.SaveChangesAsync();

        await SeedDataAsync();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedBookClubs()
    {
        var result = await bookClubService.GetAllAsync("user1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WithValidId_ShouldReturnCorrectDetails()
    {
        var result = await bookClubService.GetDetailsAsync("club_1", "user1");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Fantasy Lovers"));
            Assert.That(result.IsUserMember, Is.True);
        });
    }

    [Test]
    public async Task JoinBookClubAsync_ShouldAddUserToClub()
    {
        var bookClubId = "club_2";
        var userId = "user2";

        await bookClubService.JoinBookClubAsync(bookClubId, userId);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId &&
                                      ubc.UserId == userId &&
                                      !ubc.IsDeleted);
        Assert.That(membership, Is.Not.Null);
    }

    [Test]
    public async Task LeaveBookClubAsync_ShouldSoftDeleteMembership()
    {
        var bookClubId = "club_1";
        var userId = "user1";

        await bookClubService.LeaveBookClubAsync(bookClubId, userId);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId &&
                                      ubc.UserId == userId);
        Assert.Multiple(() =>
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership!.IsDeleted, Is.True);
        });
    }

    [Test]
    public async Task SetAdminStatusAsync_ShouldUpdateAdminStatus()
    {
        var bookClubId = "club_1";
        var userId = "user1";
        var currentUserId = "admin_user";

        await bookClubService.SetAdminStatusAsync(bookClubId, userId, currentUserId, true);

        var membership = await dbContext.UsersBookClubs
            .FirstOrDefaultAsync(ubc => ubc.BookClubId == bookClubId &&
                                      ubc.UserId == userId);
        Assert.Multiple(() =>
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership!.IsAdmin, Is.True);
        });
    }

    private async Task SeedDataAsync()
    {
        // Add users first
        var adminUser = new ApplicationUser
        {
            Id = "admin_user",
            UserName = "admin@test.com",
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User"
        };

        var user1 = new ApplicationUser
        {
            Id = "user1",
            UserName = "user1@test.com",
            Email = "user1@test.com",
            FirstName = "Test",
            LastName = "User1"
        };

        var user2 = new ApplicationUser
        {
            Id = "user2",
            UserName = "user2@test.com",
            Email = "user2@test.com",
            FirstName = "Test",
            LastName = "User2"
        };

        await dbContext.Users.AddRangeAsync(new[] { adminUser, user1, user2 });
        await dbContext.SaveChangesAsync();

        // Add book clubs
        var club1 = new BookClub
        {
            Id = "club_1",
            Name = "Fantasy Lovers",
            Description = "A club for fantasy book enthusiasts",
            OwnerId = adminUser.Id,
            IsDeleted = false
        };

        var club2 = new BookClub
        {
            Id = "club_2",
            Name = "Mystery Solvers",
            Description = "Join us to solve literary mysteries",
            OwnerId = adminUser.Id,
            IsDeleted = false
        };

        await dbContext.BookClubs.AddRangeAsync(new[] { club1, club2 });
        await dbContext.SaveChangesAsync();

        // Add memberships
        var membership1 = new UserBookClub
        {
            UserId = adminUser.Id,
            BookClubId = club1.Id,
            JoinedOn = DateTime.UtcNow,
            IsAdmin = true,
            IsDeleted = false
        };

        var membership2 = new UserBookClub
        {
            UserId = user1.Id,
            BookClubId = club1.Id,
            JoinedOn = DateTime.UtcNow,
            IsAdmin = false,
            IsDeleted = false
        };

        await dbContext.UsersBookClubs.AddRangeAsync(new[] { membership1, membership2 });
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                dbContext?.Dispose();
            }
            isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}