// ParticipantService/ParticipantBackgroundService.cs

using Dto;
using Entities;
using Microsoft.Extensions.Options;
using ParticipantService.Clients;
using ParticipantService.Options;
using Refit;

namespace ParticipantService.Services;

public class ParticipantBackgroundService : BackgroundService
{
    private readonly IHrManagerApi _hrManagerApi;
    private readonly ILogger<ParticipantBackgroundService> _logger;
    private readonly Participant _participant;
    private readonly Services.ParticipantService _participantService;
    private readonly RetryOptions _retryOptions;

    public ParticipantBackgroundService(
        IHrManagerApi hrManagerApi,
        IOptions<RetryOptions> retryOptions,
        ParticipantService participantService,
        ILogger<ParticipantBackgroundService> logger)
    {
        _hrManagerApi = hrManagerApi;
        _logger = logger;
        _participantService = participantService;
        _retryOptions = retryOptions.Value;

        // Получаем данные участника из ParticipantService
        _participant = _participantService.GetParticipant();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("ParticipantBackgroundService запускается.");
        try
        {
            // Отправка данных участника
            await SendParticipantAsync(_participant, stoppingToken);

            // Генерация и отправка Wishlist
            var probableTeammates = _participantService.GetProbableTeammates();
            var wishlist = _participant.MakeWishlist(probableTeammates);

            await SendWishlistAsync(wishlist, stoppingToken);

            _logger.LogWarning("ParticipantBackgroundService успешно отправил данные участника и Wishlist.");

            return;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Произошла критическая ошибка в ParticipantBackgroundService.");
        }

        await Task.CompletedTask;
    }

    private async Task SendParticipantAsync(Participant participantToSend, CancellationToken stoppingToken)
    {
        var participantDto = new ParticipantDto(
            participantToSend.Id,
            participantToSend.Title.ToString(),
            participantToSend.Name
        );

        _logger.LogWarning("Отправка данных участника: {@Participant}", participantDto);

        var delay = TimeSpan.FromSeconds(_retryOptions.InitialDelaySeconds);

        for (var retry = 1; retry < _retryOptions.MaxRetries; retry++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Операция SendParticipantAsync была отменена.");
                return;
            }

            try
            {
                var response = await _hrManagerApi.AddParticipantAsync(participantDto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Данные участника успешно отправлены.");
                    return;
                }
                else
                {
                    _logger.LogWarning("Не удалось отправить данные участника. Статус код: {StatusCode}",
                        response.StatusCode);
                }
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Ошибка при отправке данных участника. Попытка {Attempt}/{MaxRetries}", retry,
                    _retryOptions.MaxRetries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при отправке данных участника. Попытка {Attempt}/{MaxRetries}",
                    retry, _retryOptions.MaxRetries);
            }

            if (retry == _retryOptions.MaxRetries)
            {
                _logger.LogCritical(
                    "Достигнуто максимальное количество попыток. Не удалось отправить данные участника.");
                throw new Exception("Не удалось отправить данные участника после максимального количества попыток.");
            }

            _logger.LogWarning("Ожидание {Delay} перед следующей попыткой.", delay);
            await Task.Delay(delay, stoppingToken);
            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
        }
    }

    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(
            wishlistToSend.ParticipantId,
            wishlistToSend.ParticipantTitle.ToString(),
            wishlistToSend.DesiredParticipants);

        _logger.LogWarning("Отправка Wishlist: {@Wishlist}", wishlistDto);

        var delay = TimeSpan.FromSeconds(_retryOptions.InitialDelaySeconds);

        for (var retry = 1; retry <= _retryOptions.MaxRetries; retry++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Операция SendWishlistAsync была отменена.");
                return;
            }

            try
            {
                var response = await _hrManagerApi.AddWishlistAsync(wishlistDto);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Wishlist успешно отправлен.");
                    return;
                }
                else
                {
                    _logger.LogWarning("Не удалось отправить Wishlist. Статус код: {StatusCode}", response.StatusCode);
                }
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Ошибка при отправке Wishlist. Попытка {Attempt}/{MaxRetries}", retry,
                    _retryOptions.MaxRetries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при отправке Wishlist. Попытка {Attempt}/{MaxRetries}", retry,
                    _retryOptions.MaxRetries);
            }

            if (retry == _retryOptions.MaxRetries)
            {
                _logger.LogCritical("Достигнуто максимальное количество попыток. Не удалось отправить Wishlist.");
                throw new Exception("Не удалось отправить Wishlist после максимального количества попыток.");
            }

            _logger.LogWarning("Ожидание {Delay} перед следующей попыткой.", delay);
            await Task.Delay(delay, stoppingToken);
            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
        }
    }
}