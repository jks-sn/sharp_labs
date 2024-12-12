//HRDirectorService/Repositories/HackathonRepository.cs

using System.Threading.Tasks;
using Entities;
using HRDirectorService.Data;
using HRDirectorService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRDirectorService.Repositories;

public class HackathonRepository(HRDirectorDbContext context) : IHackathonRepository
{
    public async Task AddHackathonAsync(Hackathon hackathon)
    {
        context.Hackathons.Add(hackathon);
        await context.SaveChangesAsync();
    }
}