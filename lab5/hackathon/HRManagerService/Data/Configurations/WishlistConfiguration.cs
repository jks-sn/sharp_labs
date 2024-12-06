using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities.Consts;

namespace HRManagerService.Data.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.ParticipantId).IsRequired();

        builder.Property(w => w.ParticipantTitle)
            .IsRequired()
            .HasConversion<string>();

        // DesiredParticipants - список int, можно хранить как JSON или разделённую строку
        // Для простоты оставим по умолчанию, EF может не мапить напрямую List<int> без ValueConverter или OwnsMany
        // Предположим, мы используем EF Core 6+ и хочешь сохранить в JSON (Postgres): нужно Npgsql.Jsonb. 
        // Или просто не забудьте ValueConverter. Для примера - пусть это будет json столбец:
        builder.Property(w => w.DesiredParticipants)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne(w => w.Hackathon)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(w => w.HackathonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}