// ParticipantService/ParticipantBackgroundService.cs

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
        Services.ParticipantService participantService,
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
        _logger.LogInformation("ParticipantBackgroundService запускается.");
        try
        {
            // Отправка данных участника
            await SendParticipantAsync(_participant, stoppingToken);

            // Генерация и отправка Wishlist
            var probableTeammates = _participantService.GetProbableTeammates();
            var wishlist = _participant.MakeWishlist(probableTeammates);

            await SendWishlistAsync(wishlist, stoppingToken);

            _logger.LogInformation("ParticipantBackgroundService успешно отправил данные участника и Wishlist.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Произошла критическая ошибка в ParticipantBackgroundService.");
        }
    }

    private async Task SendParticipantAsync(Participant participantToSend, CancellationToken stoppingToken)
    {
        var participantInputModel = new ParticipantInputModel
        {
            Id = participantToSend.Id,
            Title = participantToSend.Title.ToString(),
            Name = participantToSend.Name
        };

        _logger.LogInformation("Отправка данных участника: {@Participant}", participantInputModel);

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
                var response = await _hrManagerApi.AddParticipantAsync(participantInputModel);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Данные участника успешно отправлены.");
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

            _logger.LogInformation("Ожидание {Delay} перед следующей попыткой.", delay);
            await Task.Delay(delay, stoppingToken);
            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
        }
    }

    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistInputModel = new WishlistInputModel
        {
            ParticipantId = wishlistToSend.ParticipantId,
            ParticipantTitle = wishlistToSend.ParticipantTitle.ToString(),
            DesiredParticipants = wishlistToSend.DesiredParticipants
        };

        _logger.LogInformation("Отправка Wishlist: {@Wishlist}", wishlistInputModel);

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
                var response = await _hrManagerApi.AddWishlistAsync(wishlistInputModel);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Wishlist успешно отправлен.");
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

            _logger.LogInformation("Ожидание {Delay} перед следующей попыткой.", delay);
            await Task.Delay(delay, stoppingToken);
            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
        }
    }
}