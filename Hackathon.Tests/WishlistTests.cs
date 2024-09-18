// WishlistTests.cs
using Xunit;
using Hackathon.Model;

namespace Hackathon.Tests
{
    public class WishlistTests
    {
        [Fact]
        public void Wishlist_Should_Have_Correct_Size()
        {
            // Arrange
            var juniors = new List<Junior> { new Junior { Name = "Junior1" } };
            var teamLeads = Enumerable.Range(1, 20).Select(i => new TeamLead { Name = $"TeamLead{i}" }).ToList();

            // Act
            var junior = juniors.First();
            junior.WishList = teamLeads.Select(tl => tl.Name).ToList();

            // Assert
            Assert.Equal(20, junior.WishList.Count);
        }

        [Fact]
        public void Wishlist_Should_Contain_Specific_TeamLead()
        {
            // Arrange
            var juniors = new List<Junior> { new Junior { Name = "Junior1" } };
            var specificTeamLead = new TeamLead { Name = "TeamLead_Specific" };
            var teamLeads = new List<TeamLead>
            {
                specificTeamLead,
                new TeamLead { Name = "TeamLead2" },
                // Добавьте остальные тимлидов
            };

            // Act
            var junior = juniors.First();
            junior.WishList = teamLeads.Select(tl => tl.Name).ToList();

            // Assert
            Assert.Contains(specificTeamLead.Name, junior.WishList);
        }
    }
}
