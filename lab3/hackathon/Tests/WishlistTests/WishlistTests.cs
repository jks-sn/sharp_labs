// Tests/WishlistTests/WishlistTests.cs

using System.Collections.Generic;
using System.Linq;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Hackathon.Strategy;
using Hackathon.Tests.Fixtures;
using Hackathon.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Hackathon.Tests.WishlistTests;
public class WishlistTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;

    public WishlistTests(TestDataFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void JuniorWishlist_ShouldHaveCorrectSize()
    {
        // Arrange
        var mockDataLoader = new Mock<IDataLoader>();
        mockDataLoader.Setup(dl => dl.LoadJuniors()).Returns(_fixture.Juniors);
        mockDataLoader.Setup(dl => dl.LoadTeamLeads()).Returns(_fixture.TeamLeads);

        var hrDirector = new HRDirector();

        var mockPreferenceGenerator = new Mock<IPreferenceGenerator>();
        mockPreferenceGenerator.Setup(pg => pg.GeneratePreferences(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()))
            .Callback<List<Junior>, List<TeamLead>>((juniors, teamLeads) =>
            {
                foreach (var junior in juniors)
                {
                    junior.WishList = teamLeads.Select(tl => tl.Name).ToList();
                }

                foreach (var teamLead in teamLeads)
                {
                    teamLead.WishList = juniors.Select(j => j.Name).ToList();
                }
            });

        var mockStrategyFactory = new Mock<IAssignmentStrategyFactory>();
        var mockAssignmentStrategy = new Mock<IAssignmentStrategy>();

        mockAssignmentStrategy.Setup(s => s.AssignPairs(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()))
            .Returns(new List<Team>());

        mockStrategyFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockAssignmentStrategy.Object);

        var hrManagerOptions = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(hrManagerOptions);

        var hrManager = new HRManager(mockOptions.Object, mockStrategyFactory.Object);

        var hackathon = new Services.Hackathon(hrManager, hrDirector, mockDataLoader.Object, mockPreferenceGenerator.Object);
        
        // Act
        hackathon.Run();

        // Assert
        foreach (var junior in _fixture.Juniors)
        {
            Assert.Equal(_fixture.TeamLeads.Count, junior.WishList.Count);
        }

        foreach (var teamLead in _fixture.TeamLeads)
        {
            Assert.Equal(_fixture.Juniors.Count, teamLead.WishList.Count);
        }
    }

    [Fact]
    public void Wishlist_ShouldContainSpecificEmployee()
    {
        // Arrange
        var specificJunior = _fixture.Juniors.First();
        var specificTeamLead = _fixture.TeamLeads.First();

        var mockDataLoader = new Mock<IDataLoader>();
        mockDataLoader.Setup(dl => dl.LoadJuniors()).Returns(_fixture.Juniors);
        mockDataLoader.Setup(dl => dl.LoadTeamLeads()).Returns(_fixture.TeamLeads);

        var hrDirector = new HRDirector();

        var mockPreferenceGenerator = new Mock<IPreferenceGenerator>();
        mockPreferenceGenerator.Setup(pg => pg.GeneratePreferences(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()))
            .Callback<List<Junior>, List<TeamLead>>((juniors, teamLeads) =>
            {
                foreach (var junior in juniors)
                {
                    junior.WishList = teamLeads.Select(tl => tl.Name).ToList();
                }

                foreach (var teamLead in teamLeads)
                {
                    teamLead.WishList = juniors.Select(j => j.Name).ToList();
                }
            });

        var mockStrategyFactory = new Mock<IAssignmentStrategyFactory>();
        var mockAssignmentStrategy = new Mock<IAssignmentStrategy>();

        mockAssignmentStrategy.Setup(s => s.AssignPairs(It.IsAny<List<Junior>>(), It.IsAny<List<TeamLead>>()))
            .Returns(new List<Team>());

        mockStrategyFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockAssignmentStrategy.Object);

        var hrManagerOptions = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(hrManagerOptions);

        var hrManager = new HRManager(mockOptions.Object, mockStrategyFactory.Object);

        var hackathon = new Services.Hackathon(hrManager, hrDirector, mockDataLoader.Object, mockPreferenceGenerator.Object);
        
        foreach (var junior in _fixture.Juniors)
        {
            junior.SatisfactionIndex = 3; 
        }

        foreach (var teamLead in _fixture.TeamLeads)
        {
            teamLead.SatisfactionIndex = 4;
        }
        // Act
        hackathon.Run();

        // Assert
        foreach (var junior in _fixture.Juniors)
        {
            Assert.Contains(specificTeamLead.Name, junior.WishList);
        }

        foreach (var teamLead in _fixture.TeamLeads)
        {
            Assert.Contains(specificJunior.Name, teamLead.WishList);
        }
    }
}
