//HRManagerService/Repositories/TeamRepository.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using HRManagerService.Data;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagerService.Repositories;

public class TeamRepository(HRManagerDbContext context) : ITeamRepository
{
    public async Task AddTeamsAsync(IEnumerable<Team> teams)
    {
        // Отладочный вывод входящих данных
        foreach (var team in teams)
        {
            Console.WriteLine($"Adding Team: HackathonId={team.Id}, HackathonId={team.HackathonId}, " +
                              $"TeamLeadId={team.TeamLeadId}, JuniorId={team.JuniorId}");
        }

        // Проверка на дублирование перед добавлением
        var duplicateTeams = teams.GroupBy(t => new { t.HackathonId, t.TeamLeadId, t.JuniorId })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();
        if (duplicateTeams.Any())
        {
            Console.WriteLine("Duplicate Teams Detected:");
            foreach (var team in duplicateTeams)
            {
                Console.WriteLine($"Duplicate Team: HackathonId={team.Id}, HackathonId={team.HackathonId}, " +
                                  $"TeamLeadId={team.TeamLeadId}, JuniorId={team.JuniorId}");
            }
            throw new InvalidOperationException("Attempting to add duplicate teams.");
        }

        
        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();
    }

    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams.ToListAsync();
    }
}