//HRDirectorService/Data/Configurations/WishlistConfiguration.cs

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRDirectorService.Data.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(t => t.Id)
            .UseIdentityColumn();
        
        builder.Property(w => w.ParticipantTitle)
            .HasConversion<string>();
        
        builder.Property(w => w.DesiredParticipants)
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.HasOne(w => w.Participant)
            .WithMany(p => p.Wishlists)
            .HasForeignKey(w => new { w.ParticipantId, w.ParticipantTitle })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Hackathon)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(w => w.HackathonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}