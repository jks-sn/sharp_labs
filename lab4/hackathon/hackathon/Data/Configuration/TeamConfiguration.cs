using Hackathon.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasOne(t => t.Junior)
            .WithMany()
            .HasForeignKey(t => t.JuniorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TeamLead)
            .WithMany()
            .HasForeignKey(t => t.TeamLeadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}