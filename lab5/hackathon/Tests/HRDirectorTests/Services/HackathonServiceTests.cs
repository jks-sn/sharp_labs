// Tests/HRDirectorTests/Services/HackathonServiceTests.cs
using System;
using System.Collections.Generic;
using Dto;
using Entities;
using HRDirectorService.Interfaces;
using HRDirectorService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HRDirectorTests.Services;

public class HackathonServiceTests
{
    [Fact]
    public async void ProcessHackathonAsync_ShouldCalculateMeanSatisfaction()
    {
        // Arrange
        var hackathonRepoMock = new Mock<IHackathonRepository>();
        var logger = new Mock<ILogger<HackathonService>>();
        var service = new HackathonService(hackathonRepoMock.Object, logger.Object);

        var participants = new List<ParticipantDto>
        {
            new ParticipantDto(1, "Junior", "J1"),
            new ParticipantDto(2, "TeamLead", "T1")
        };

        var wishlists = new List<WishlistDto>
        {
            new WishlistDto(2, "TeamLead", new List<int>{1})
        };

        var teams = new List<TeamDto>
        {
            new TeamDto(
                new ParticipantDto(2, "TeamLead", "T1"),
                new ParticipantDto(1, "Junior", "J1")
            )
        };

        var hackathonDto = new HackathonDto(1, 0.0, participants, wishlists, teams);

        // Act
        await service.ProcessHackathonAsync(hackathonDto);

        // Assert
        hackathonRepoMock.Verify(r => r.AddHackathonAsync(It.Is<Hackathon>(h => Math.Abs(h.MeanSatisfactionIndex - 5.0) < 0.001)), Times.Once);
    }

    [Fact]
    public async void ProcessHackathonAsync_ShouldHandleNoTeamsGracefully()
    {
        // Arrange
        var hackathonRepoMock = new Mock<IHackathonRepository>();
        var logger = new Mock<ILogger<HackathonService>>();
        var service = new HackathonService(hackathonRepoMock.Object, logger.Object);

        var hackathonDto = new HackathonDto(1, 0.0, new List<ParticipantDto>(), new List<WishlistDto>(), new List<TeamDto>());

        // Act
        await service.ProcessHackathonAsync(hackathonDto);

        // Assert
        hackathonRepoMock.Verify(r => r.AddHackathonAsync(It.Is<Hackathon>(h => h.MeanSatisfactionIndex == 0.0)), Times.Once);
    }
}
