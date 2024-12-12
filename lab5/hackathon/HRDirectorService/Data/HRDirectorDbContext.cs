//HRDirectorService/Data/HRDirectorDbContext.cs

using Entities;
using HRDirectorService.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HRDirectorService.Data;

public class HRDirectorDbContext(DbContextOptions<HRDirectorDbContext> options) : DbContext(options)
{
    public DbSet<Hackathon> Hackathons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HackathonConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}