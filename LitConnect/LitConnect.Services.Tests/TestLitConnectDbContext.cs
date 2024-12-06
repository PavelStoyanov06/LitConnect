using LitConnect.Data;
using LitConnect.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LitConnect.Services.Tests;

public class TestLitConnectDbContext : LitConnectDbContext
{
    public TestLitConnectDbContext(DbContextOptions<LitConnectDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships without seeding data
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
    }
}