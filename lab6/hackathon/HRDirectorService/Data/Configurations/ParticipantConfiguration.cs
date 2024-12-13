//HRDirectorService/Data/Configurations/ParticipantConfiguration.cs

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRDirectorService.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Entities.Participant>
{
    public void Configure(EntityTypeBuilder<Entities.Participant> builder)
    {
        builder.ToTable("Participants");
        
        builder.HasKey(p => new { p.Id, p.Title });;
        
        builder.Property(p => p.Title)
            .HasConversion<string>();
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Participant)
            .HasForeignKey(w => new { w.ParticipantId, w.ParticipantTitle })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Hackathon)
            .WithMany(h => h.Participants)
            .HasForeignKey(p => p.HackathonId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}