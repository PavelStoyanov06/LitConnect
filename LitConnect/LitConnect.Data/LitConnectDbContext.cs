namespace LitConnect.Data;

using LitConnect.Data.Configurations.EntityConfigurations;
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
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserBookClub>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.BookClubs)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserBookClub>()
            .HasOne(ub => ub.BookClub)
            .WithMany(bc => bc.Users)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Discussion>()
            .HasOne(d => d.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Discussion>()
            .HasOne(d => d.BookClub)
            .WithMany(bc => bc.Discussions)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookGenre>()
            .HasOne(bg => bg.Book)
            .WithMany(b => b.Genres)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookGenre>()
            .HasOne(bg => bg.Genre)
            .WithMany(g => g.Books)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(c => c.Discussion)
            .WithMany(d => d.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookClub>()
            .HasOne<Book>()
            .WithMany()
            .HasForeignKey(bc => bc.CurrentBookId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed data configurations
        builder.ConfigureIdentity();
        builder.ConfigureGenre();
        builder.ConfigureBook();
        builder.ConfigureBookClub();
        builder.ConfigureUserBookClub();
    }
}