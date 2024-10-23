// File: Tests/HRDirectorTests/HRDirectorTests.cs

using System.Collections.Generic;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Services;
using Hackathon.Services;
using Hackathon.Interface;
using Hackathon.Tests.Fixtures;
using Hackathon.Utilities;
using Moq;
using Xunit;

namespace Hackathon.Tests.HRDirectorTests;

public class HRDirectorTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;

    public HRDirectorTests(TestDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void EvaluateHackathon_ShouldReturnCorrectHarmonicMean()
    {
        // Arrange
        foreach (var junior in _fixture.Juniors)
        {
            junior.SatisfactionIndex = 3; 
        }
        foreach (var teamLead in _fixture.TeamLeads)
        {
            teamLead.SatisfactionIndex = 4; 
        }
        
        var hrDirector = new HRDirector();

        var allParticipants = new List<Participant>();
        allParticipants.AddRange(_fixture.Juniors);
        allParticipants.AddRange(_fixture.TeamLeads);

        // Act
        decimal harmonicMean = hrDirector.EvaluateHackathon(allParticipants);

        // Assert
        // Гармоническое среднее для [3,3,3,4,4] = 5 / (1/3 + 1/3 + 1/3 + 1/4 + 1/4) = 5 / (1 + 0.5) = 5 / 1.5 ≈ 3.333
        Assert.True(Math.Abs(harmonicMean - 3.33m) < 0.1m, $"Ожидалось 3.33, получено {harmonicMean}");
    }
    [Fact]
    public void EvaluateHackathon_ShouldHandleDifferentSatisfactionIndices()
    {
        // Arrange
        var participants = new List<Participant>
        {
            new Junior { Name = "Junior1", SatisfactionIndex = 5 },
            new Junior { Name = "Junior2", SatisfactionIndex = 10 },
            new TeamLead { Name = "TeamLead1", SatisfactionIndex = 15 }
        };

        var hrDirector = new HRDirector();

        // Гармоническое среднее: 3 / (1/5 + 1/10 + 1/15) = 3 / (0.2 + 0.1 + 0.0667) ≈ 3 / 0.3667 ≈ 8.1818
        decimal expectedHarmonicMean = 8.1818m;

        // Act
        decimal harmonicMean = hrDirector.EvaluateHackathon(participants);

        // Assert
        Assert.True(Math.Abs(harmonicMean - expectedHarmonicMean) < 0.01m, $"Ожидалось {expectedHarmonicMean}, получено {harmonicMean}");
    }

    [Fact]
    public void EvaluateHackathon_ShouldThrowException_WhenParticipantsAreEmpty()
    {
        // Arrange
        var participants = new List<Participant>();

        var hrDirector = new HRDirector();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => hrDirector.EvaluateHackathon(participants));
    }

    [Fact]
    public void EvaluateHackathon_ShouldThrowException_WhenAnySatisfactionIndexIsZero()
    {
        // Arrange
        var participants = new List<Participant>
        {
            new Junior { Name = "Junior1", SatisfactionIndex = 3 },
            new Junior { Name = "Junior2", SatisfactionIndex = 0 },
            new TeamLead { Name = "TeamLead1", SatisfactionIndex = 4 }
        };

        var hrDirector = new HRDirector();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => hrDirector.EvaluateHackathon(participants));
    }

    [Fact]
    public void EvaluateHackathon_ShouldReturnZero_WhenDenominatorIsInfinity()
    {
        // Arrange
        var participants = new List<Participant>
        {
            new Junior { Name = "Junior1", SatisfactionIndex = 1 },
            new Junior { Name = "Junior2", SatisfactionIndex = 1 },
            new TeamLead { Name = "TeamLead1", SatisfactionIndex = 1 }
        };

        var hrDirector = new HRDirector();

        // Гармоническое среднее: 3 / (1 + 1 + 1) = 1
        decimal expectedHarmonicMean = 1m;

        // Act
        decimal harmonicMean = hrDirector.EvaluateHackathon(participants);

        // Assert
        Assert.Equal(expectedHarmonicMean, harmonicMean);
    }
}
