// Handlers/PrintHackathonInfoHandler.cs

using System.Threading;
using System.Threading.Tasks;
using Hackathon.Commands;
using Hackathon.Services;
using MediatR;

namespace Hackathon.Handlers;

public class PrintHackathonInfoHandler(HackathonPrinter hackathonPrinter)
    : IRequestHandler<PrintHackathonInfo>
{
    public Task Handle(PrintHackathonInfo request, CancellationToken cancellationToken)
    {
        hackathonPrinter.PrintHackathonById(request.HackathonId);
        return Task.FromResult(Unit.Task);
    }
}