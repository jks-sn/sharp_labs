// Tests/HRManagerTests/Services/HRManagerBackgroundServiceTests.cs

using System.Threading;
using System.Threading.Tasks;
using HRManagerService.Clients;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using HRManagerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HRManagerTests.Services;

public class HRManagerBackgroundServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldSendHackathonData_WhenAllParticipantsAndWishlistsReady()
    {
        // Arrange
        var services = new ServiceCollection();

        var hrDirectorClientMock = new Mock<IHRDirectorClient>();
        hrDirectorClientMock
            .Setup(c => c.SendHackathonDataAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        services.AddSingleton(hrDirectorClientMock.Object);

        var participantRepoMock = new Mock<IParticipantRepository>();
        var wishlistRepoMock = new Mock<IWishlistRepository>();
        var hackathonRepoMock = new Mock<IHackathonRepository>();
        var teamBuildingStrategyMock = new Mock<ITeamBuildingStrategy>();
        var teamRepoMock = new Mock<ITeamRepository>();
        services.AddSingleton(participantRepoMock.Object);
        services.AddSingleton(wishlistRepoMock.Object);
        services.AddSingleton(hackathonRepoMock.Object);
        services.AddSingleton(teamBuildingStrategyMock.Object);
        services.AddSingleton(teamRepoMock.Object);

        // Настройка репозиториев для возврата достаточного количества участников и вишлистов
        var participants = new List<Entities.Participant>
        {
            new Entities.Participant { Id = 1, Title = Entities.Consts.ParticipantTitle.TeamLead },
            new Entities.Participant { Id = 2, Title = Entities.Consts.ParticipantTitle.Junior }
        };
        participantRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(participants);
        participantRepoMock.Setup(r => r.CountAsync()).ReturnsAsync(participants.Count);


        var wishlists = new List<Entities.Wishlist>
        {
            new Entities.Wishlist { Id = 1, Participant = participants[0] },
            new Entities.Wishlist { Id = 2, Participant = participants[1] }
        };
        wishlistRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(wishlists);
        wishlistRepoMock.Setup(r => r.CountAsync()).ReturnsAsync(wishlists.Count);


        var teams = new List<Entities.Team>
        {
            new Entities.Team { Id = 1, TeamLead = participants[0], Junior = participants[1] }
        };
        teamBuildingStrategyMock.Setup(t => t.BuildTeams(
                It.IsAny<IEnumerable<Entities.Participant>>(),
                It.IsAny<IEnumerable<Entities.Participant>>(),
                It.IsAny<IEnumerable<Entities.Wishlist>>(),
                It.IsAny<IEnumerable<Entities.Wishlist>>()))
            .Returns(teams);

        var createdHackathon = new Entities.Hackathon
        {
            Id = 1,
            Participants = participants,
            Wishlists = wishlists,
            Teams = teams,
            MeanSatisfactionIndex = 0.0
        };
        hackathonRepoMock.Setup(r => r.CreateHackathonAsync(It.IsAny<Entities.Hackathon>()))
            .ReturnsAsync((Entities.Hackathon h) =>
            {
                h.Id = createdHackathon.Id;
                return h;
            });

        var controllerOptions = new HRManagerService.Options.ControllerOptions { ParticipantsNumber = 2 };
        var hrManagerService = new HRManagerService.Services.HRManagerService(
            participantRepoMock.Object,
            wishlistRepoMock.Object,
            Microsoft.Extensions.Options.Options.Create(controllerOptions),
            Mock.Of<ILogger<HRManagerService.Services.HRManagerService>>());
        services.AddSingleton(hrManagerService);

        var loggerMock = new Mock<ILogger<HRManagerBackgroundService>>();
        services.AddSingleton(loggerMock.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var service = new TestHRManagerBackgroundService(loggerMock.Object, serviceProvider);

        // Act
        await service.ExecuteAsyncPublic(CancellationToken.None);

        // Assert
        hrDirectorClientMock.Verify(c => c.SendHackathonDataAsync(createdHackathon.Id), Times.Once,
            "Ожидается один вызов SendHackathonDataAsync, так как участники и вишлисты готовы.");
    }
}