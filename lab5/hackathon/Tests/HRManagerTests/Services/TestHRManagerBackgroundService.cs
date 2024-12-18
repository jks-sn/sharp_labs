using HRManagerService.Interface;
using HRManagerService.Interfaces;
using HRManagerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HRManagerTests.Services;

public class TestHRManagerBackgroundService : HRManagerBackgroundService
{
    public TestHRManagerBackgroundService(
        ILogger<HRManagerBackgroundService> logger,
        IServiceProvider serviceProvider)
        : base(
            logger,
            serviceProvider.GetRequiredService<HRManagerService.Services.HRManagerService>(),
            serviceProvider.GetRequiredService<IHRDirectorClient>(),
            serviceProvider.GetRequiredService<IParticipantRepository>(),
            serviceProvider.GetRequiredService<IWishlistRepository>(),
            serviceProvider.GetRequiredService<ITeamBuildingStrategy>(),
            serviceProvider.GetRequiredService<IHackathonRepository>())
    {
    }

    public Task ExecuteAsyncPublic(CancellationToken cancellationToken)
    {
        return base.ExecuteAsync(cancellationToken);
    }
}