//HRDirectorService/Data/Configurations/HackathonConfiguration.cs

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRDirectorService.Data.Configurations;

public class HackathonConfiguration : IEntityTypeConfiguration<Entities.Hackathon>
{
    public void Configure(EntityTypeBuilder<Entities.Hackathon> builder)
    {
        
        builder.HasKey(h => h.Id);

        builder.Property(t => t.Id)
            .UseIdentityColumn();
        
        builder.Property(h => h.MeanSatisfactionIndex)
            .HasDefaultValue(0.0);

        builder.HasMany(h => h.Participants)
            .WithOne(p => p.Hackathon)
            .HasForeignKey(p => p.HackathonId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(h => h.Wishlists)
            .WithOne(w => w.Hackathon)
            .HasForeignKey(w => w.HackathonId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(h => h.Teams)
            .WithOne(t => t.Hackathon)
            .HasForeignKey(t => t.HackathonId)
            .OnDelete(DeleteBehavior.Cascade);  
    }
}