// Tests/HRManagerTests/Integration/HRManagerDatabaseTests.cs
using System.Threading.Tasks;
using HRManagerService.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace HRManagerTests.Integration;

public class HRManagerDatabaseTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("hrmanagerdbtest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private HRManagerDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        var cs = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<HRManagerDbContext>()
            .UseNpgsql(cs)
            .Options;

        _dbContext = new HRManagerDbContext(options);

        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanInsertParticipant()
    {
        var participant = new Entities.Participant
        {
            Id = 100,
            Title = Entities.Consts.ParticipantTitle.Junior,
            Name = "TestJunior"
        };

        _dbContext.Participants.Add(participant);
        await _dbContext.SaveChangesAsync();

        var retrieved = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == 100 && p.Title == Entities.Consts.ParticipantTitle.Junior);
        Assert.NotNull(retrieved);
        Assert.Equal("TestJunior", retrieved!.Name);
    }
}