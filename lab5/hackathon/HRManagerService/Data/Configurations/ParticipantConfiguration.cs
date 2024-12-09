//HRManagerService/Data/Configurations/ParticipantConfiguration.cs

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities.Consts;

namespace HRManagerService.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasKey(p => new { p.Id, p.Title });;
        
        builder.Property(p => p.Title)
            .HasConversion<string>();
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Participant)
            .HasForeignKey(w => new { w.ParticipantId, w.ParticipantTitle })
            .OnDelete(DeleteBehavior.Cascade); // Логично, удаляем участника - удаляем его вишлисты (или можно Restrict)

        builder.HasOne(p => p.Hackathon)
            .WithMany(h => h.Participants)
            .HasForeignKey(p => p.HackathonId)
            .OnDelete(DeleteBehavior.SetNull); // Если хакатон удалён, участники остаются, но без ссылки
    }
}