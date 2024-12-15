//HRManagerService/Data/HRManagerDbContextFactory.cs

using HRManagerService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

public class HRManagerDbContextFactory : IDesignTimeDbContextFactory<HRManagerDbContext>
{
    public HRManagerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        var connectionString = configuration.GetConnectionString("HRManagerConnection");
        
        var optionsBuilder = new DbContextOptionsBuilder<HRManagerDbContext>();
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        optionsBuilder.UseNpgsql(connectionString);
        
        return new HRManagerDbContext(optionsBuilder.Options);
    }
}