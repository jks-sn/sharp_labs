//HRDirectorService/Data/Configurations/TeamConfiguration.cs

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRDirectorService.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(t => t.Hackathon)
            .WithMany(h => h.Teams)
            .HasForeignKey(t => t.HackathonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(t => t.TeamLead)
            .WithMany()
            .HasForeignKey(t => t.TeamLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Junior)
            .WithMany()
            .HasForeignKey(j => j.JuniorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}