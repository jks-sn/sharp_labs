using Hackathon.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Data.Configurations;

public class HackathonEventConfiguration : IEntityTypeConfiguration<HackathonEvent>
{
    public void Configure(EntityTypeBuilder<HackathonEvent> builder)
    {
        builder.HasMany(h => h.Participants)
            .WithOne(p => p.HackathonEvent)
            .HasForeignKey(p => p.HackathonEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Teams)
            .WithOne(t => t.HackathonEvent)
            .HasForeignKey(t => t.HackathonEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}