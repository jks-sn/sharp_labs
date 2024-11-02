// Data/HackathonDbContext.cs

using Microsoft.EntityFrameworkCore;
using Hackathon.Model;

namespace Hackathon.Data;

public class HackathonDbContext : DbContext
{
    public DbSet<HackathonEvent> Hackathons { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Junior> Juniors { get; set; }
    public DbSet<TeamLead> TeamLeads { get; set; }
    public DbSet<Team> Teams { get; set; }

    public HackathonDbContext(DbContextOptions<HackathonDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>()
            .HasDiscriminator<string>("ParticipantType")
            .HasValue<Junior>("Junior")
            .HasValue<TeamLead>("TeamLead");
        
        modelBuilder.Entity<Participant>()
            .HasMany(p => p.Preferences)
            .WithOne(p => p.Participant)
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Team>()
            .HasOne(t => t.Junior)
            .WithMany()
            .HasForeignKey(t => t.JuniorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.TeamLead)
            .WithMany()
            .HasForeignKey(t => t.TeamLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HackathonEvent>()
            .HasMany(h => h.Participants)
            .WithOne(p => p.HackathonEvent)
            .HasForeignKey(p => p.HackathonEventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HackathonEvent>()
            .HasMany(h => h.Teams)
            .WithOne(t => t.HackathonEvent)
            .HasForeignKey(t => t.HackathonEventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Participant>()
            .Property(p => p.AssignedPartner)
            .IsRequired(false);
        
    }
}
