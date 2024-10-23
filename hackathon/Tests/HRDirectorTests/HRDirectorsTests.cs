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
    
}
