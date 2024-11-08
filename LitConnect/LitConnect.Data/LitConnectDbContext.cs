namespace LitConnect.Data;

using LitConnect.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class LitConnectDbContext : IdentityDbContext<ApplicationUser>
{
    public LitConnectDbContext(DbContextOptions<LitConnectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookClub> BookClubs { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<UserBookClub> UsersBookClubs { get; set; }
    public DbSet<ReadingList> ReadingLists { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BooksGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserBookClub>()
            .HasOne(ub => ub.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserBookClub>()
            .HasOne(ub => ub.BookClub)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookGenre>()
            .HasOne(bg => bg.Book)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookGenre>()
            .HasOne(bg => bg.Genre)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}