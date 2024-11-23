using Hackathon.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasDiscriminator<string>("ParticipantType")
            .HasValue<Junior>("Junior")
            .HasValue<TeamLead>("TeamLead");

        builder.HasMany(p => p.Preferences)
            .WithOne(p => p.Participant)
            .HasForeignKey(p => p.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.AssignedPartner)
            .IsRequired(false);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
}