using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
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

    public async Task<List<Participant>> GetAllAsync()
    {
        return await context.Participants.ToListAsync();
    }

    public async Task<Participant> GetByIdAsync(int id)
    {
        return await context.Participants.FindAsync(id);
    }

    public async Task<int> CountAsync()
    {
        return await context.Participants.CountAsync();
    }
}
