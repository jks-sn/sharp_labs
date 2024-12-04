using System.Text;
using System.Text.Json;
using Dto;
using Entities;
using Microsoft.Extensions.Options;

namespace ParticipantService;

public class ParticipantBackgroundService : BackgroundService
{
    private readonly ILogger<ParticipantBackgroundService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ParticipantService _participantService;
    private readonly int _maxRetries = 5;
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(5);

    public ParticipantBackgroundService(
        ILogger<ParticipantBackgroundService> logger,
        IHttpClientFactory httpClientFactory,
        ParticipantService participantService)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(ParticipantBackgroundService));
        _participantService = participantService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ParticipantBackgroundService запускается.");
        try
        {
            var participant = _participantService.GetParticipant();
            var probableTeammates = _participantService.GetProbableTeammates();

            // Отправка данных участника
            await SendParticipantAsync(participant, stoppingToken);

            // Генерация и отправка Wishlist
            var wishlist = participant.MakeWishlist(probableTeammates);
            await SendWishlistAsync(wishlist, stoppingToken);

            _logger.LogInformation("ParticipantBackgroundService успешно отправил данные участника и Wishlist.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Произошла критическая ошибка в ParticipantBackgroundService.");
        }

        await Task.CompletedTask;
    }

    private async Task SendParticipantAsync(Participant participantToSend, CancellationToken stoppingToken)
    {
        var participantDto = new ParticipantDto(participantToSend.Id, participantToSend.Title, participantToSend.Name);
        _logger.LogInformation("Отправка данных участника: {@ParticipantDto}", participantDto);

        var delay = _initialDelay;

        for (var retry = 0; retry < _maxRetries; retry++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Операция SendParticipantAsync была отменена.");
                return;
            }

            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync("api/hr_manager/participant", participantDto, stoppingToken);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation(
                    "Данные участника 'Id = {Id}, Title = {Title}, Name = {Name}' успешно отправлены.",
                    participantDto.Id, participantDto.Title, participantDto.Name);
                break;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка при отправке данных участника. Попытка {Attempt}/{MaxRetries}", retry,
                    _maxRetries);
                if (retry == _maxRetries)
                {
                    _logger.LogCritical(
                        "Достигнуто максимальное количество попыток. Не удалось отправить данные участника.");
                    throw;
                }

                _logger.LogInformation("Ожидание {Delay} перед следующей попыткой.", delay);
                await Task.Delay(delay, stoppingToken);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
            }
        }
    }

    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(wishlistToSend.Id, wishlistToSend.ParticipantTitle,
            wishlistToSend.DesiredParticipants);
        _logger.LogInformation("Отправка Wishlist: {@WishlistDto}", wishlistDto);

        var delay = _initialDelay;

        for (var retry = 1; retry < _maxRetries; retry++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Операция SendWishlistAsync была отменена.");
                return;
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/hr_manager/wishlist", wishlistDto, stoppingToken);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Wishlist участника 'Id = {Id}, Title = {Title}' успешно отправлен.",
                    wishlistDto.ParticipantId, wishlistDto.ParticipantTitle);
                break;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка при отправке Wishlist. Попытка {Attempt}/{MaxRetries}", retry,
                    _maxRetries);
                if (retry == _maxRetries)
                {
                    _logger.LogCritical("Достигнуто максимальное количество попыток. Не удалось отправить Wishlist.");
                    throw;
                }

                _logger.LogInformation("Ожидание {Delay} перед следующей попыткой.", delay);
                await Task.Delay(delay, stoppingToken);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
            }
        }
    }
}