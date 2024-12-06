using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HRManagerService.Data;
public class HRManagerDbContext(DbContextOptions<HRManagerDbContext> options) : DbContext(options)
{
    public DbSet<Entities.Participant> Participants { get; set; }
    public DbSet<Entities.Wishlist> Wishlists { get; set; }
    public DbSet<Entities.Hackathon> Hackathons { get; set; }
    public DbSet<Entities.Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Применяем все конфигурации из сборки (папка Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}