// Tests/PreferenceGeneratorTests/PreferenceGeneratorTests.cs

using Hackathon.Data;
using Hackathon.Preferences;
using Hackathon.Tests.DatabaseTests;
using Hackathon.Tests.Fixtures;
using Xunit;

namespace Hackathon.Tests.PreferenceGeneratorTests
{
    [Collection("Database collection")]
    public class PreferenceGeneratorTests(TestDataFixture fixture, TestDatabaseFixture dbFixture)
        : IClassFixture<TestDataFixture>
    {
        private readonly HackathonDbContext _dbContext = dbFixture.DbContext;

        [Fact]
        public void GeneratePreferences_ShouldAssignPreferencesToAllParticipants()
        {
            // Arrange
            var preferenceGenerator = new RandomPreferenceGenerator();

            var juniors = fixture.Juniors;
            var teamLeads = fixture.TeamLeads;

            // Act
            preferenceGenerator.GeneratePreferences(juniors, teamLeads);

            // Assert
            foreach (var junior in juniors)
            {
                Assert.NotNull(junior.Preferences);
                Assert.Equal(teamLeads.Count, junior.Preferences.Count);
            }

            foreach (var teamLead in teamLeads)
            {
                Assert.NotNull(teamLead.Preferences);
                Assert.Equal(juniors.Count, teamLead.Preferences.Count);
            }
        }
    }
}