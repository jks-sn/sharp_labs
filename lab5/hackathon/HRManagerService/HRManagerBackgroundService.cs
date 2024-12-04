using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dto;
using Entities;
using Entities.Consts;
using Entities.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HRManagerService
{
    public class HRManagerBackgroundService : BackgroundService
    {
        private readonly ILogger<HRManagerBackgroundService> _logger;
        private readonly HttpClient _httpClient;
        private readonly HRManagerService _hrManagerService;
        private readonly ITeamBuildingStrategy _teamBuildingStrategy = new GaleShapleyStrategy();
        private readonly int _maxRetries = 5;
        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(5);

        public HRManagerBackgroundService(
            ILogger<HRManagerBackgroundService> logger,
            HttpClient httpClient,
            HRManagerService hrManagerService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _hrManagerService = hrManagerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HRManagerBackgroundService запущен.");

            try
            {
                
                // Ожидание завершения получения всех участников
                _logger.LogInformation("Ожидание получения всех participants.");
                await _hrManagerService.WaitAllParticipantsReceivedAsync(stoppingToken);
                _logger.LogInformation("Все участники получены.");

                // Ожидание получения всех wishlists
                _logger.LogInformation("Ожидание получения всех wishlists.");
                await _hrManagerService.WaitAllWishlistsReceivedAsync(stoppingToken);
                _logger.LogInformation("Все wishlists получены.");

                // Отправка участников
                await SendParticipantsAsync(_hrManagerService.Participants, stoppingToken);
                _logger.LogInformation("Участники отправлены.");

                // Отправка wishlists
                await SendWishlistsAsync(_hrManagerService.Wishlists.Values, stoppingToken);
                _logger.LogInformation("Wishlists отправлены.");
                
                // Создание команд
                _logger.LogInformation("Создание команд ");
                var teams = _teamBuildingStrategy.BuildTeams(
                    teamLeads: _hrManagerService.Participants.Where(p => p.Title == ParticipantTitle.TeamLead),
                    juniors: _hrManagerService.Participants.Where(p => p.Title == ParticipantTitle.Junior),
                    teamLeadsWishlists: _hrManagerService.Wishlists.Values.Where(w => w.ParticipantTitle == ParticipantTitle.TeamLead),
                    juniorsWishlists: _hrManagerService.Wishlists.Values.Where(w => w.ParticipantTitle == ParticipantTitle.Junior));

                _logger.LogInformation("Команды созданы.");

                // Отправка команд
                await SendTeamsAsync(teams, stoppingToken);
                _logger.LogInformation("Команды отправлены.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("HRManagerBackgroundService останавливается по запросу отмены.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка в HRManagerBackgroundService.");
            }
        }
        
        private async Task SendParticipantsAsync(IEnumerable<Participant> participants, CancellationToken stoppingToken)
        {
            var participantDtos = participants.Select(p => new ParticipantDto(p.Id, p.Title, p.Name)).ToList();
            var json = JsonSerializer.Serialize(participantDtos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = "api/hr_director/participants";
            _logger.LogInformation("Отправка участников на {Endpoint}", endpoint);

            await SendWithRetriesAsync(endpoint, content, stoppingToken);
        }
        
        private async Task SendWishlistsAsync(IEnumerable<Wishlist> wishlists, CancellationToken stoppingToken)
        {
            var wishlistDtos = wishlists.Select(w => new WishlistDto(w.ParticipantId, w.ParticipantTitle, w.DesiredParticipants)).ToList();
            var json = JsonSerializer.Serialize(wishlistDtos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = "api/hr_director/wishlists";
            _logger.LogInformation("Отправка wishlists на {Endpoint}", endpoint);

            await SendWithRetriesAsync(endpoint, content, stoppingToken);
        }
        
        private async Task SendTeamsAsync(IEnumerable<Team> teams, CancellationToken stoppingToken)
        {
            var teamDtos = teams.Select(t => new TeamDto(
                new ParticipantDto(t.TeamLead.Id, t.TeamLead.Title, t.TeamLead.Name),
                new ParticipantDto(t.Junior.Id, t.Junior.Title, t.Junior.Name))).ToList();
            var json = JsonSerializer.Serialize(teamDtos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = "api/hr_director/teams";
            _logger.LogInformation("Отправка команд на {Endpoint}", endpoint);

            await SendWithRetriesAsync(endpoint, content, stoppingToken);
        }

        private async Task SendWithRetriesAsync(string endpoint, HttpContent content, CancellationToken stoppingToken)
        {
            var delay = _initialDelay;

            for (int retry = 1; retry <= _maxRetries; retry++)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Запрос отменён. Прерывание отправки на {Endpoint}.", endpoint);
                    return;
                }

                try
                {
                    var response = await _httpClient.PostAsync(endpoint, content, stoppingToken);
                    response.EnsureSuccessStatusCode();
                    _logger.LogInformation("Данные успешно отправлены на {Endpoint}.", endpoint);
                    break; // Успешная отправка, выходим из цикла
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке данных на {Endpoint}. Попытка {Attempt}/{MaxRetries}.", endpoint, retry, _maxRetries);

                    if (retry == _maxRetries)
                    {
                        _logger.LogCritical("Достигнуто максимальное количество попыток. Не удалось отправить данные на {Endpoint}.", endpoint);
                        throw; // Повторно выбрасываем исключение после последней попытки
                    }

                    _logger.LogInformation("Ожидание {Delay} перед следующей попыткой.", delay);
                    await Task.Delay(delay, stoppingToken);
                    delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Экспоненциальная задержка
                }
            }
        }
    }
}
