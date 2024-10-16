//Tests/HRDirectorTests/HRDirectorTests.cs

using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Strategy;
using Hackathon.Tests.Fixtures;
using Moq;
using System.Linq;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hackathon.Tests.HRDirectorTests;

public class HRDirectorsTests
{
    [Fact]
    public void ComputeHarmonic_ShouldReturnSameValue_ForIdenticalSatisfactionIndices()
    {
        // Arrange
        var hrDirector = new HRDirector(null);
        var participants = new List<Participant>
        {
            new Junior { SatisfactionIndex = 10 },
            new TeamLead { SatisfactionIndex = 10 },
            new Junior { SatisfactionIndex = 10 },
        };

        // Act
        double harmonicMean = hrDirector.ComputeHarmonic(participants);

        // Assert
        Assert.Equal(10, harmonicMean);
    }

}