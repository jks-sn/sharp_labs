//HRDirectorService/Data/HRDirectorDbContextFactory.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace HRDirectorService.Data;

public class HRDirectorDbContextFactory : IDesignTimeDbContextFactory<HRDirectorDbContext>
{
    public HRDirectorDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        var connectionString = configuration.GetConnectionString("HRDirectorConnection");
        
        var optionsBuilder = new DbContextOptionsBuilder<HRDirectorDbContext>();
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        optionsBuilder.UseNpgsql(connectionString);
        
        return new HRDirectorDbContext(optionsBuilder.Options);
    }
}