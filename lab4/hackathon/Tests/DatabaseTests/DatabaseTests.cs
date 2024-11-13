// Tests/DatabaseTests/DatabaseTests.cs

using System;
using System.Threading.Tasks;
using Hackathon.Data;
using Hackathon.Model;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Hackathon.Tests.DatabaseTests;

public class DatabaseTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private HackathonDbContext _dbContext;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var connectionString = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<HackathonDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new HackathonDbContext(options);

        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanSaveAndRetrieveHackathonEvent()
    {
        // Arrange
        var hackathonEvent = new HackathonEvent { Harmonic = 3.5 };
        _dbContext.Hackathons.Add(hackathonEvent);
        await _dbContext.SaveChangesAsync();

        // Act
        var retrievedEvent = await _dbContext.Hackathons.FirstOrDefaultAsync(h => h.Id == hackathonEvent.Id);

        // Assert
        Assert.NotNull(retrievedEvent);
        Assert.Equal(3.5, retrievedEvent.Harmonic);
    }

    [Fact]
    public async Task CanCalculateAverageHarmonic()
    {
        // Arrange
        _dbContext.Hackathons.AddRange(
            new HackathonEvent { Harmonic = 3.0 },
            new HackathonEvent { Harmonic = 4.0 }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var averageHarmonic = await _dbContext.Hackathons.AverageAsync(h => h.Harmonic);

        // Assert
        Assert.Equal(3.5, averageHarmonic);
    }
}
