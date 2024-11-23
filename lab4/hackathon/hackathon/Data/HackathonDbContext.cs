// Data/HackathonDbContext.cs

using Microsoft.EntityFrameworkCore;
using Hackathon.Model;
using Hackathon.Data.Configurations;

namespace Hackathon.Data;

public class HackathonDbContext(DbContextOptions<HackathonDbContext> options) : DbContext(options)
{
    public DbSet<HackathonEvent> Hackathons { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Junior> Juniors { get; set; }
    public DbSet<TeamLead> TeamLeads { get; set; }
    public DbSet<Team> Teams { get; set; }
    
    public DbSet<Preference> Preferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new HackathonEventConfiguration());
        modelBuilder.ApplyConfiguration(new PreferenceConfiguration());
    }
}
