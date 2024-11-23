// Tests/HackathonTests/HackathonTests.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackathon.Data;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Hackathon.Strategy;
using Hackathon.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace Hackathon.Tests.HackathonTests;

public class HackathonTests(TestDataFixture fixture) : IClassFixture<TestDataFixture>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    private HackathonDbContext _dbContext = null!;

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
    public void Run_ShouldReturnHarmonicGreaterThanZero()
    {
        // Arrange
        
        var dataLoader = new TestDataLoader(fixture.Juniors, fixture.TeamLeads);

        var preferenceGenerator = new RandomPreferenceGenerator();

        var strategies = new List<IAssignmentStrategy>
        {
            new GaleShapleyStrategy(),
            new RandomAssignmentStrategy()
        };
        var strategyFactory = new StrategyFactory(strategies);

        var hrManagerOptions = Microsoft.Extensions.Options.Options.Create(new HRManagerOptions { AssignmentStrategy = "GaleShapleyStrategy" });
        var hrManager = new HRManager(hrManagerOptions, strategyFactory);

        var hrDirector = new HRDirector();

        var hackathon = new Services.Hackathon(hrManager, hrDirector, dataLoader, preferenceGenerator, _dbContext);

        // Act
        double harmonic = hackathon.Run();

        // Assert
        Assert.True(harmonic > 0, "Гармоничность должна быть больше нуля.");

        var allParticipants = fixture.Juniors.Cast<Participant>().Concat(fixture.TeamLeads).ToList();
        foreach (var participant in allParticipants)
        {
            Assert.InRange(participant.SatisfactionIndex, 1, int.MaxValue);
        }
    }
}
