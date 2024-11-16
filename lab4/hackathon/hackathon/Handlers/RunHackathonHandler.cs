// Handlers/RunHackathonHandler.cs

using System.Threading;
using System.Threading.Tasks;
using Hackathon.Commands;
using Hackathon.Interface;
using MediatR;

namespace Hackathon.Handlers;

public class RunHackathonHandler(IHackathon hackathonService) : IRequestHandler<RunHackathon, double>
{
    public Task<double> Handle(RunHackathon request, CancellationToken cancellationToken)
    {
        var harmonic = hackathonService.Run();
        return Task.FromResult(harmonic);
    }
}