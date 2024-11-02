// Tests/DatabaseTests/DatabaseTests.cs

using Microsoft.EntityFrameworkCore;
using Hackathon.Data;
using Hackathon.Model;
using Xunit;

namespace Hackathon.Tests.DatabaseTests;

public class DatabaseTests
{
    private HackathonDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<HackathonDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new HackathonDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public void CanSaveAndRetrieveHackathonEvent()
    {
        using (var context = GetInMemoryDbContext())
        {
            var hackathonEvent = new HackathonEvent { Harmonic = 3.5 };
            context.Hackathons.Add(hackathonEvent);
            context.SaveChanges();

            var retrievedEvent = context.Hackathons.FirstOrDefault(h => h.Id == hackathonEvent.Id);
            Assert.NotNull(retrievedEvent);
            Assert.Equal(3.5, retrievedEvent.Harmonic);
        }
    }

    [Fact]
    public void CanCalculateAverageHarmonic()
    {
        using (var context = GetInMemoryDbContext())
        {
            context.Hackathons.AddRange(
                new HackathonEvent { Harmonic = 3.0 },
                new HackathonEvent { Harmonic = 4.0 }
            );
            context.SaveChanges();

            var averageHarmonic = context.Hackathons.Average(h => h.Harmonic);
            Assert.Equal(3.5, averageHarmonic);
        }
    }
}