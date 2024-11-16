// Tests/HRManagerTests/HRManagerTests.cs

using System.Collections.Generic;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Services;
using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Tests.Builder;
using Hackathon.Tests.Fixtures;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hackathon.Tests.HRManagerTests;
public class HRManagerTests(TestDataFixture fixture) : IClassFixture<TestDataFixture>
{
    [Fact]
    public void AssignTeams_ShouldAssignTeamsCorrectlyUsingGaleShapleyStrategy()
    {
        // Arrange
        var strategies = new List<IAssignmentStrategy>
        {
            new GaleShapleyStrategy(),
            new RandomAssignmentStrategy()
        };
        var strategyFactory = new StrategyFactory(strategies);
        
        var hrManagerOptions = Microsoft.Extensions.Options.Options.Create(new HRManagerOptions { AssignmentStrategy = "GaleShapleyStrategy" });
        
        var hrManager = new HRManager(hrManagerOptions, strategyFactory);

        // Act
        var teams = hrManager.AssignTeams(fixture.Juniors, fixture.TeamLeads);

        // Assert
        Assert.Equal(3, teams.Count);
        Assert.Contains(teams, t => t.Junior.Name == "Junior1" && t.TeamLead.Name == "TeamLead1");
        Assert.Contains(teams, t => t.Junior.Name == "Junior2" && t.TeamLead.Name == "TeamLead2");
        Assert.Contains(teams, t => t.Junior.Name == "Junior3" && t.TeamLead.Name == "TeamLead3");

    }
}
