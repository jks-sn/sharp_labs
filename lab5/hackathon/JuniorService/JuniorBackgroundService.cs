using System.Text;
using System.Text.Json;
using Dto;
using Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Services;

namespace JuniorService;

public class JuniorBackgroundService : BackgroundService
{
    private readonly ILogger<JuniorBackgroundService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceSettings _serviceSettings;

    public JuniorBackgroundService(
        ILogger<JuniorBackgroundService> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<ServiceSettings> serviceSettings, 
        DataLoader dataLoader)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _serviceSettings = serviceSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Загрузка вероятных напарников

        _logger.LogInformation("Junior '{Id} {Name}' is sending data...", _serviceSettings.Participant.Id, _serviceSettings.Participant.Name);

        var httpClient = _httpClientFactory.CreateClient("HrManagerClient");

        await SendParticipantAsync(httpClient, _serviceSettings.Participant, stoppingToken);
        _logger.LogInformation("Junior '{Id} {Name}' has sent their data.", _serviceSettings.Participant.Id, _serviceSettings.Participant.Name);

        var wishlist = _serviceSettings.Participant.MakeWishlist(_serviceSettings.ProbableTeammates);
        await SendWishlistAsync(httpClient, wishlist, stoppingToken);
        _logger.LogInformation("Junior '{Id} {Name}' has sent their wishlist.", _serviceSettings.Participant.Id, _serviceSettings.Participant.Name);
    }

    private async Task SendParticipantAsync(HttpClient httpClient, Participant participantToSend, CancellationToken stoppingToken)
    {
        var participantDto = new ParticipantDto(participantToSend.Id, participantToSend.Title, participantToSend.Name);
        var content = new StringContent(JsonSerializer.Serialize(participantDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/hr_manager/participant", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task SendWishlistAsync(HttpClient httpClient, Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(wishlistToSend.ParticipantId, wishlistToSend.ParticipantTitle, wishlistToSend.DesiredParticipants);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/hr_manager/wishlist", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
}
