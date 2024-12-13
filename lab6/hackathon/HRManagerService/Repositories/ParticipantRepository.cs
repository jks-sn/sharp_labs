//HRManagerService/Repositories/ParticipantRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
using HRManagerService.Entities;
using HRManagerService.Entities.Consts;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Repositories;

public class ParticipantRepository(HRManagerDbContext context) : IParticipantRepository
{
    public async Task AddParticipantAsync(Participant participant)
    {
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await context.Participants.CountAsync();
    }
    
    public async Task<int> GetParticipantCountForHackathonAsync(int hackathonId)
    {
        return await context.Participants
            .Where(p => p.HackathonId == hackathonId)
            .CountAsync();
    }
    
    public async Task<List<Participant>> GetAllAsync()
    {
        return await context.Participants.ToListAsync();
    }

    public async Task<List<Participant>> GetParticipantsForHackathonAsync(int hackathonId)
    {
        return await context.Participants
            .Where(p => p.HackathonId == hackathonId)
            .ToListAsync();
    }
    
    public async Task<Participant> GetByIdAsync(int id, ParticipantTitle title)
    {
        return await context.Participants.FindAsync(id, title);
    }
    
}
