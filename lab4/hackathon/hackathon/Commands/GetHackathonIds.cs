// Commands/GetHackathonIds.cs

using MediatR;
using System.Collections.Generic;

namespace Hackathon.Commands
{
    public class GetHackathonIds : IRequest<List<int>>
    {
    }
}