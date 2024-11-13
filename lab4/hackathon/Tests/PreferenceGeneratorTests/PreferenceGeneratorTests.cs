// Tests/PreferenceGeneratorTests/PreferenceGeneratorTests.cs

using Hackathon.Data;
using Hackathon.Preferences;
using Hackathon.Tests.DatabaseTests;
using Hackathon.Tests.Fixtures;
using Xunit;

namespace Hackathon.Tests.PreferenceGeneratorTests
{
    [Collection("Database collection")]
    public class PreferenceGeneratorTests : IClassFixture<TestDataFixture>
    {
        private readonly TestDataFixture _fixture;
        private readonly HackathonDbContext _dbContext;

        public PreferenceGeneratorTests(TestDataFixture fixture, TestDatabaseFixture dbFixture)
        {
            _fixture = fixture;
            _dbContext = dbFixture.DbContext;
        }

        [Fact]
        public void GeneratePreferences_ShouldAssignPreferencesToAllParticipants()
        {
            // Arrange
            var preferenceGenerator = new RandomPreferenceGenerator();

            var juniors = _fixture.Juniors;
            var teamLeads = _fixture.TeamLeads;

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