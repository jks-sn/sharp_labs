// Handlers/PrintAverageHarmonicHandler.cs

using System.Threading;
using System.Threading.Tasks;
using Hackathon.Commands;
using Hackathon.Services;
using MediatR;

namespace Hackathon.Handlers;

public class PrintAverageHarmonicHandler(HackathonPrinter hackathonPrinter)
    : IRequestHandler<PrintAverageHarmonic>
{
    public Task Handle(PrintAverageHarmonic request, CancellationToken cancellationToken)
    {
        hackathonPrinter.PrintAverageHarmonic();
        return Task.FromResult(Unit.Task);
    }
}