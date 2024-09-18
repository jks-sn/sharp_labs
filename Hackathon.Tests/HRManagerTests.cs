// HRManagerTests.cs
using Xunit;
using Moq;
using Hackathon.Model;
using Hackathon.Strategy;

namespace HackathonSimulation.Tests
{
    public class HRManagerTests
    {
        [Fact]
        public void AssignTeams_Should_Return_Correct_Number_Of_Teams()
        {
            // Arrange
            var juniors = Enumerable.Range(1, 20).Select(i => new Junior { Name = $"Junior{i}" }).ToList();
            var teamLeads = Enumerable.Range(1, 20).Select(i => new TeamLead { Name = $"TeamLead{i}" }).ToList();

            // Создаём фиктивную стратегию
            var strategy = new MockAssignmentStrategy();
            var hrManager = new HRManager(strategy);

            // Act
            var teams = hrManager.AssignTeams(juniors, teamLeads);

            // Assert
            Assert.Equal(20, teams.Count);
        }

        [Fact]
        public void AssignTeams_Should_Return_Predefined_Distribution()
        {
            // Arrange
            var junior = new Junior { Name = "Junior1" };
            var teamLead = new TeamLead { Name = "TeamLead1" };

            var juniors = new List<Junior> { junior };
            var teamLeads = new List<TeamLead> { teamLead };

            // Фиктивная стратегия, которая всегда возвращает заданную пару
            var strategy = new PredefinedAssignmentStrategy(junior, teamLead);
            var hrManager = new HRManager(strategy);

            // Act
            var teams = hrManager.AssignTeams(juniors, teamLeads);

            // Assert
            Assert.Single(teams);
            Assert.Equal(junior, teams[0].Junior);
            Assert.Equal(teamLead, teams[0].TeamLead);
        }
        [Fact]
        public void AssignTeams_Should_Call_Strategy_Exactly_Once()
        {
            // Arrange
            var juniors = new List<Junior> { new Junior { Name = "Junior1" } };
            var teamLeads = new List<TeamLead> { new TeamLead { Name = "TeamLead1" } };

            var strategyMock = new Mock<IAssignmentStrategy>();
            strategyMock.Setup(s => s.AssignPairs(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()))
                .Returns([new Team(juniors[0], teamLeads[0])]);

            var hrManager = new HRManager(strategyMock.Object);

            // Act
            var teams = hrManager.AssignTeams(juniors, teamLeads);

            // Assert
            strategyMock.Verify(s => s.AssignPairs(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()), Times.Once);
        }
    }

    public class PredefinedAssignmentStrategy(Junior junior, TeamLead teamLead) : IAssignmentStrategy
    {
        private readonly Junior _junior = junior;
        private readonly TeamLead _teamLead = teamLead;

        public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
        {
            return new List<Team> { new Team(_junior, _teamLead) };
        }
    }


    // Фиктивная стратегия для тестирования
    public class MockAssignmentStrategy : IAssignmentStrategy
    {
        public List<Team> AssignPairs(List<Junior> juniors, List<TeamLead> teamLeads)
        {
            // Возвращаем пары джунов и тимлидов по порядку
            return juniors.Zip(teamLeads, (j, tl) => new Team(j, tl)).ToList();
        }
    }

}
