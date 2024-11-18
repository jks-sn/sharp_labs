// Tests/Fixtures/TestDatabaseFixture.cs

using System.Threading.Tasks;
using Hackathon.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Hackathon.Tests.DatabaseTests;

public class TestDatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgresContainer { get; private set; }
    public HackathonDbContext DbContext { get; private set; }

    public TestDatabaseFixture()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();

        var connectionString = PostgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<HackathonDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        DbContext = new HackathonDbContext(options);

        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await PostgresContainer.DisposeAsync();
    }
}