//HRManagerService/Data/HRManagerDbContext

using Entities;
using HRManagerService.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Data;
public sealed class HRManagerDbContext(DbContextOptions<HRManagerDbContext> options) : DbContext(options)
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        modelBuilder.ApplyConfiguration(new HackathonConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}