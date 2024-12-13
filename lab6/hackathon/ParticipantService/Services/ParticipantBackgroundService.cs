// ParticipantService/ParticipantBackgroundService.cs

using Dto;
using Messages;
using MassTransit;
using Microsoft.Extensions.Options;
using ParticipantService.Clients;
using ParticipantService.Options;
using Refit;

namespace ParticipantService.Services;

public class ParticipantBackgroundService(
    ILogger<ParticipantBackgroundService> logger,
    ParticipantService participantService,
    IBus bus)
    : BackgroundService, IConsumer<IHackathonStarted>
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
    
    public async Task Consume(ConsumeContext<IHackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        logger.LogWarning("ParticipantBackgroundService запускается.");
        
        var participant = participantService.GetParticipant();
        
        await bus.Publish<IParticipantInfo>(new
        {
            Id = participant.Id,
            Title = participant.Title.ToString(),
            Name = participant.Name,
            HackathonId = hackathonId
        });
        
        var wishlist = participant.MakeWishlist(participantService.GetProbableTeammates());
        await bus.Publish<IWishlistInfo>(new
        {
            ParticipantId = wishlist.ParticipantId,
            ParticipantTitle = wishlist.ParticipantTitle.ToString(),
            HackathonId = hackathonId,
            DesiredParticipants = wishlist.DesiredParticipants.ToArray()
        });

        logger.LogInformation("Participant {Id} data and wishlist for hackathon {HackathonId} published.", participant.Id, hackathonId);
        
    }
}