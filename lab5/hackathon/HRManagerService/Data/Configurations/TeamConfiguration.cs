using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagerService.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.HackathonId).IsRequired();
        builder.Property(t => t.TeamLeadId).IsRequired();
        builder.Property(t => t.JuniorId).IsRequired();

        builder.HasOne(t => t.TeamLead)
            .WithMany()
            .HasForeignKey(t => t.TeamLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Junior)
            .WithMany()
            .HasForeignKey(t => t.JuniorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Hackathon)
            .WithMany(h => h.Teams)
            .HasForeignKey(t => t.HackathonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}