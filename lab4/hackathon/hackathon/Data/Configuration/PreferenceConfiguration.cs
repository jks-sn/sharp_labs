using Hackathon.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Data.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PreferredName)
            .IsRequired();

        builder.HasOne(p => p.Participant)
            .WithMany(p => p.Preferences)
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}