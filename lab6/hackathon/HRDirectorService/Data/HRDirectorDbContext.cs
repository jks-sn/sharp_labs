using Entities;
using Microsoft.EntityFrameworkCore;
using HRDirectorService.Data.Configurations;
using Hackathon = HRDirectorService.Entities.Hackathon;
using Participant = HRDirectorService.Entities.Participant;

namespace HRDirectorService.Data;

public sealed class HRDirectorDbContext(DbContextOptions<HRDirectorDbContext> options) : DbContext(options)
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Hackathon> Hackathons { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HackathonConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}