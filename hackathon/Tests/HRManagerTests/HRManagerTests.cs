//Tests/HRManagerTests.cs

using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Strategy;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Hackathon.Tests.HRManagerTests;

public class HRManagerTests
{
    [Fact]
    public void AssignTeams_ShouldReturnCorrectNumberOfTeams()
    {
        // Arrange
        var mockStrategy = new Mock<IAssignmentStrategy>();
        var mockFactory = new Mock<IAssignmentStrategyFactory>();
        mockFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockStrategy.Object);

        var options = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(options);

        var hrManager = new HRManager(mockOptions.Object, mockFactory.Object);

        var juniors = new List<Junior>
        {
            new Junior { Name = "Junior1" },
            new Junior { Name = "Junior2" }
        };

        var teamLeads = new List<TeamLead>
        {
            new TeamLead { Name = "TeamLead1" },
            new TeamLead { Name = "TeamLead2" }
        };

        var expectedTeams = new List<Team>
        {
            new Team(juniors[0], teamLeads[0]),
            new Team(juniors[1], teamLeads[1])
        };

        mockStrategy.Setup(s => s.AssignPairs(juniors, teamLeads)).Returns(expectedTeams);

        // Act
        var teams = hrManager.AssignTeams(juniors, teamLeads);

        // Assert
        Assert.Equal(expectedTeams.Count, teams.Count);
    }

    [Fact]
    public void AssignTeams_ShouldReturnPredefinedDistribution()
    {
        // Arrange
        var mockStrategy = new Mock<IAssignmentStrategy>();
        var mockFactory = new Mock<IAssignmentStrategyFactory>();
        mockFactory.Setup(f => f.GetStrategy("MockStrategy")).Returns(mockStrategy.Object);

        var options = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(options);

        var hrManager = new HRManager(mockOptions.Object, mockFactory.Object);

        var juniors = new List<Junior>
        {
            new Junior { Name = "Junior1" },
            new Junior { Name = "Junior2" }
        };

        var teamLeads = new List<TeamLead>
        {
            new TeamLead { Name = "TeamLead1" },
            new TeamLead { Name = "TeamLead2" }
        };

        var predefinedTeams = new List<Team>
        {
            new Team(juniors[0], teamLeads[1]),
            new Team(juniors[1], teamLeads[0])
        };

        mockStrategy.Setup(s => s.AssignPairs(juniors, teamLeads)).Returns(predefinedTeams);

        // Act
        var teams = hrManager.AssignTeams(juniors, teamLeads);

        // Assert
        Assert.Collection(teams,
            team =>
            {
                Assert.Equal("Junior1", team.Junior.Name);
                Assert.Equal("TeamLead2", team.TeamLead.Name);
            },
            team =>
            {
                Assert.Equal("Junior2", team.Junior.Name);
                Assert.Equal("TeamLead1", team.TeamLead.Name);
            });
    }

    [Fact]
    public void AssignTeams_Strategy_ShouldBeCalledExactlyOnce()
    {
        // Arrange
        var mockStrategy = new Mock<IAssignmentStrategy>();
        var mockFactory = new Mock<IAssignmentStrategyFactory>();
        mockFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockStrategy.Object);

        var options = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(options);

        var hrManager = new HRManager(mockOptions.Object, mockFactory.Object);

        var juniors = new List<Junior>
        {
            new Junior { Name = "Junior1" },
            new Junior { Name = "Junior2" }
        };

        var teamLeads = new List<TeamLead>
        {
            new TeamLead { Name = "TeamLead1" },
            new TeamLead { Name = "TeamLead2" }
        };

        var teams = new List<Team>
        {
            new Team(juniors[0], teamLeads[0]),
            new Team(juniors[1], teamLeads[1])
        };

        mockStrategy.Setup(s => s.AssignPairs(juniors, teamLeads)).Returns(teams);

        // Act
        hrManager.AssignTeams(juniors, teamLeads);
        hrManager.AssignTeams(juniors, teamLeads);

        // Assert
        mockStrategy.Verify(s => s.AssignPairs(juniors, teamLeads), Times.Exactly(2));
    }
}