// HRDirectorTests.cs\

using Xunit;
using Hackathon.Model;

namespace HackathonSimulation.Tests
{
    public class HRDirectorTests
    {
        [Fact]
        public void HarmonicMean_Of_Same_Numbers_Returns_That_Number()
        {
            // Arrange
            var hrDirector = new HRDirector();
            var participants = new List<Participant>
            {
                new Junior { SatisfactionIndex = 5 },
                new Junior { SatisfactionIndex = 5 },
                new TeamLead { SatisfactionIndex = 5 },
                new TeamLead { SatisfactionIndex = 5 }
            };

            // Act
            var harmonicity = hrDirector.ComputeHarmonicity(participants);

            // Assert
            Assert.Equal(5, harmonicity);
        }

        [Fact]
        public void HarmonicMean_Of_2_And_6_Should_Be_3()
        {
            // Arrange
            var hrDirector = new HRDirector();
            var participants = new List<Participant>
            {
                new Junior { SatisfactionIndex = 2 },
                new TeamLead { SatisfactionIndex = 6 }
            };

            // Act
            var harmonicity = hrDirector.ComputeHarmonicity(participants);

            // Assert
            Assert.Equal(3, harmonicity);
        }
        [Fact]
        public void ComputeHarmonicity_Should_Return_Predefined_Value()
        {
            // Arrange
            var hrDirector = new HRDirector();

            var junior = new Junior
            {
                Name = "Junior1",
                AssignedPartner = "TeamLead1",
                PreferenceList = new List<string> { "TeamLead1", "TeamLead2" }
            };
            junior.CalculateSatisfactionIndex(); // SatisfactionIndex = 20

            var teamLead = new TeamLead
            {
                Name = "TeamLead1",
                AssignedPartner = "Junior1",
                PreferenceList = new List<string> { "Junior1", "Junior2" }
            };
            teamLead.CalculateSatisfactionIndex(); // SatisfactionIndex = 20

            var participants = new List<Participant> { junior, teamLead };

            // Act
            var harmonicity = hrDirector.ComputeHarmonicity(participants);

            // Assert
            Assert.Equal(20, harmonicity);
        }

    }
}
