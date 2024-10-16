//Tests/WishlistTests.cs

using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Strategy;
using Hackathon.Tests.Fixtures;
using Moq;
using System.Linq;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hackathon.Tests.WishlistTests;

 public class WishlistTests(TestDataFixture fixture) : IClassFixture<TestDataFixture>
 {
    private readonly TestDataFixture _fixture = fixture;

    [Fact]
    public void JuniorWishlist_ShouldHaveCorrectSize()
    {
        // Arrange
        var mockDataLoader = new Mock<DataLoader>(new OptionsWrapper<DataLoaderOptions>(new DataLoaderOptions()));
        
        mockDataLoader.Setup(dl => dl.LoadJuniors()).Returns(_fixture.Juniors);
        mockDataLoader.Setup(dl => dl.LoadTeamLeads()).Returns(_fixture.TeamLeads);
        
        var hrDirector = new HRDirector(mockDataLoader.Object);
        
        var mockStrategyFactory = new Mock<IAssignmentStrategyFactory>();
        var mockAssignmentStrategy = new Mock<IAssignmentStrategy>();
        mockStrategyFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockAssignmentStrategy.Object);
        
        var hrManagerOptions = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(hrManagerOptions);

        var hrManager = new HRManager(mockOptions.Object, mockStrategyFactory.Object);
        
        var hackathon = new Hackathon.Model.Hackathon(hrManager, hrDirector);

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

        var mockDataLoader = new Mock<DataLoader>(new OptionsWrapper<DataLoaderOptions>(new DataLoaderOptions()));
        mockDataLoader.Setup(dl => dl.LoadJuniors()).Returns(_fixture.Juniors);
        mockDataLoader.Setup(dl => dl.LoadTeamLeads()).Returns(_fixture.TeamLeads);

        var hrDirector = new HRDirector(mockDataLoader.Object);

        var mockStrategyFactory = new Mock<IAssignmentStrategyFactory>();
        var mockAssignmentStrategy = new Mock<IAssignmentStrategy>();
        mockStrategyFactory.Setup(f => f.GetStrategy(It.IsAny<string>())).Returns(mockAssignmentStrategy.Object);

        var hrManagerOptions = new HRManagerOptions { AssignmentStrategy = "MockStrategy" };
        var mockOptions = new Mock<IOptions<HRManagerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(hrManagerOptions);

        var hrManager = new HRManager(mockOptions.Object, mockStrategyFactory.Object);

        var hackathon = new Hackathon.Model.Hackathon(hrManager, hrDirector);
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