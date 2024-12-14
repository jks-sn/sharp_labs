//HRManagerService/Data/Configurations/ParticipantConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRManagerService.Entities;

namespace HRManagerService.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Title)
            .HasConversion<string>();
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(p => new {p.ParticipantId, p.Title, p.HackathonId}).IsUnique();
        
        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Participant)
            .HasForeignKey(w => new { w.ParticipantId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}