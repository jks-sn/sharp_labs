using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities.Consts;

namespace HRManagerService.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Храним enum ParticipantTitle как строку  
        builder.Property(p => p.Title)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Participant)
            .HasForeignKey(w => w.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade); // Логично, удаляем участника - удаляем его вишлисты (или можно Restrict)

        builder.HasOne(p => p.Hackathon)
            .WithMany(h => h.Participants)
            .HasForeignKey(p => p.HackathonId)
            .OnDelete(DeleteBehavior.SetNull); // Если хакатон удалён, участники остаются, но без ссылки
    }
}