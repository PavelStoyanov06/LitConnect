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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Will add configurations later
    }
}