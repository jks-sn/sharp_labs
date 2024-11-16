// Handlers/GetHackathonIdsHandler.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Commands;
using Hackathon.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Handlers;

public class GetHackathonIdsHandler(HackathonDbContext dbContext) : IRequestHandler<GetHackathonIds, List<int>>
{
    public async Task<List<int>> Handle(GetHackathonIds request, CancellationToken cancellationToken)
    {
        var ids = await dbContext.Hackathons.Select(h => h.Id).ToListAsync(cancellationToken);
        return ids;
    }
}