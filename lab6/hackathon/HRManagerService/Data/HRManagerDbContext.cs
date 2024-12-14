//HRManagerService/Data/HRManagerDbContext

using HRManagerService.Data.Configurations;
using HRManagerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Data;
public sealed class HRManagerDbContext(DbContextOptions<HRManagerDbContext> options) : DbContext(options)
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}