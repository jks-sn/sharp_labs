using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRDirectorService.Data.Configurations;

public class HackathonConfiguration : IEntityTypeConfiguration<Hackathon>
{
    public void Configure(EntityTypeBuilder<Hackathon> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.MeanSatisfactionIndex).HasDefaultValue(0.0);
    }
}