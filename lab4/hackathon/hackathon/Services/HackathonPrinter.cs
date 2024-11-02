// Services/HackathonPrinter.cs

using Hackathon.Data;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Services;

public class HackathonPrinter
{
    private readonly HackathonDbContext _dbContext;

    public HackathonPrinter(HackathonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void PrintHackathonById(int hackathonId)
    {
        var hackathon = _dbContext.Hackathons
            .Include(h => h.Participants)
            .Include(h => h.Teams)
            .ThenInclude(t => t.Junior)
            .Include(h => h.Teams)
            .ThenInclude(t => t.TeamLead)
            .FirstOrDefault(h => h.Id == hackathonId);

        if (hackathon == null)
        {
            Console.WriteLine($"Хакатон с ID {hackathonId} не найден.");
            return;
        }

        Console.WriteLine($"Хакатон {hackathon.Id}: Гармоничность = {hackathon.Harmonic:F2}");

        Console.WriteLine("Участники:");
        foreach (var participant in hackathon.Participants)
        {
            Console.WriteLine($"- {participant.Name}");
        }

        Console.WriteLine("Команды:");
        foreach (var team in hackathon.Teams)
        {
            Console.WriteLine($"- Джун: {team.Junior.Name}, Тимлид: {team.TeamLead.Name}");
        }
    }

    public void PrintAverageHarmonic()
    {
        var averageHarmonic = _dbContext.Hackathons.Average(h => h.Harmonic);
        Console.WriteLine($"Средняя гармоничность по всем хакатонам: {averageHarmonic:F2}");
    }
}