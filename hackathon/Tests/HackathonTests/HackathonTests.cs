// File: Tests/HackathonTests/HackathonTests.cs

using System.Collections.Generic;
using System.Linq;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Tests.Builder;
using Hackathon.Tests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hackathon.Tests.HackathonTests;

public class HackathonTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;

    public HackathonTests(TestDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Run_ShouldReturnHarmonicGreaterThanZero()
    {
        // Arrange

        var dataLoader = new TestDataLoader(_fixture.Juniors, _fixture.TeamLeads);

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
        
        var hackathon = new Services.Hackathon(hrManager, hrDirector, dataLoader, preferenceGenerator);

        // Act
        double harmonic = hackathon.Run();

        // Assert
        Assert.True(harmonic > 0, "Гармоничность должна быть больше нуля.");
        
        var allParticipants = _fixture.Juniors.Cast<Participant>().Concat(_fixture.TeamLeads).ToList();
        foreach (var participant in allParticipants)
        {
            Assert.InRange(participant.SatisfactionIndex, 1, int.MaxValue);
        }
    }
}

public class TestDataLoader : IDataLoader
{
    private readonly List<Junior> _juniors;
    private readonly List<TeamLead> _teamLeads;

    public TestDataLoader(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        _juniors = juniors;
        _teamLeads = teamLeads;
    }

    public List<Junior> LoadJuniors()
    {
        return _juniors;
    }

    public List<TeamLead> LoadTeamLeads()
    {
        return _teamLeads;
    }
}
