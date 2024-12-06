using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagerService.Data.Configurations;

public class HackathonConfiguration : IEntityTypeConfiguration<Hackathon>
{
    public void Configure(EntityTypeBuilder<Hackathon> builder)
    {
        builder.HasKey(h => h.Id);

        // MeanSatisfactionIndex по умолчанию 0.0, можно nullable сделать
        builder.Property(h => h.MeanSatisfactionIndex)
            .HasDefaultValue(0.0);

        // Связи уже настроены в других конфигурациях через WithMany, но можно явно:
        // builder.HasMany(h => h.Participants).WithOne(p => p.Hackathon).HasForeignKey(p => p.HackathonId);
        // builder.HasMany(h => h.Wishlists).WithOne(w => w.Hackathon).HasForeignKey(w => w.HackathonId);
        // builder.HasMany(h => h.Teams).WithOne(t => t.Hackathon).HasForeignKey(t => t.HackathonId);
    }
}