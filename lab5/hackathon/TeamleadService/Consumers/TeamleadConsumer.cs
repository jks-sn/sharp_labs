// TeamleadService/Consumers/TeamleadConsumer.cs
using MassTransit;
using Message;
using Microsoft.Extensions.Logging;

namespace TeamleadService.Consumers
{
    public class TeamleadConsumer : IConsumer<IHackathonStarted>
    {
        private readonly ILogger<TeamleadConsumer> _logger;
        private readonly TeamleadService _teamleadService;

        public TeamleadConsumer(ILogger<TeamleadConsumer> logger, TeamleadService teamleadService)
        {
            _logger = logger;
            _teamleadService = teamleadService;
        }

        public Task Consume(ConsumeContext<IHackathonStarted> context)
        {
            _logger.LogInformation("Hackathon started message received: {Message}", context.Message.Message);
            _teamleadService.HackathonStartedTcs.SetResult(true);
            return Task.CompletedTask;
        }
    }
}