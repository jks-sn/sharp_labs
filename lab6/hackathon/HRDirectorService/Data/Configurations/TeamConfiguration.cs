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
        
        builder.Property(t => t.TeamLeadTitle)
            .HasConversion<string>();

        builder.Property(t => t.JuniorTitle)
            .HasConversion<string>();

        builder.HasOne(t => t.Hackathon)
            .WithMany(h => h.Teams)
            .HasForeignKey(t => t.HackathonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(t => t.TeamLead)
            .WithMany()
            .HasForeignKey(t => new { t.TeamLeadId, t.TeamLeadTitle })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Junior)
            .WithMany()
            .HasForeignKey(t => new { t.JuniorId, t.JuniorTitle })
            .OnDelete(DeleteBehavior.Restrict);
    }
}