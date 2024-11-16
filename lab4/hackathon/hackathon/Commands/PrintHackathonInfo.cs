// Commands/PrintHackathonInfo.cs

using MediatR;

namespace Hackathon.Commands;

public class PrintHackathonInfo(int hackathonId) : IRequest
{
    public int HackathonId { get; } = hackathonId;
}