
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class AppDbContext : DbContext
{
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Hackathon> Hackathons { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка отношений между сущностями

        // Hackathon
        modelBuilder.Entity<Hackathon>()
            .HasMany(h => h.Participants)
            .WithMany(e => e.Hackathons);

        modelBuilder.Entity<Hackathon>()
            .HasMany(h => h.Wishlists)
            .WithOne(w => w.Hackathon)
            .HasForeignKey(w => w.HackathonId);

        modelBuilder.Entity<Hackathon>()
            .HasMany(h => h.Teams)
            .WithOne(t => t.Hackathon)
            .HasForeignKey(t => t.HackathonId);

        // Participant
        modelBuilder.Entity<Participant>()
            .HasMany(e => e.Wishlists)
            .WithOne(w => w.Participant)
            .HasForeignKey(w => w.ParticipantId);

        modelBuilder.Entity<Participant>()
            .HasMany(e => e.Teams)
            .WithMany(t => t.Participants);

        // Team
        modelBuilder.Entity<Team>()
            .HasMany(t => t.Participants)
            .WithMany(e => e.Teams);
    }
}