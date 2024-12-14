//HRManagerService/Data/Configurations/WishlistConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRManagerService.Entities;

namespace HRManagerService.Data.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(w => w.DesiredParticipants)
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.HasOne(w => w.Participant)
            .WithMany(p => p.Wishlists)
            .HasForeignKey(w => w.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}