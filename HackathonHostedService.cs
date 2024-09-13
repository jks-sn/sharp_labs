// HackathonHostedService.cs

using Hackathon.Model;
using Microsoft.Extensions.Hosting;

namespace HackathonSimulation
{
    public class HackathonHostedService(DataLoader dataLoader, HRDirector hrDirector, HRManager hrManager) : BackgroundService
    {
        private readonly DataLoader _dataLoader = dataLoader;
        private readonly HRDirector _hrDirector = hrDirector;
        private readonly HRManager _hrManager = hrManager;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => RunHackathons(), stoppingToken);
        }

        private void RunHackathons()
        {
            var juniors = _dataLoader.LoadJuniors("./data/Juniors20.csv");
            var teamLeads = _dataLoader.LoadTeamLeads("./data/Teamleads20.csv");

            double totalHarmonicity = 0;
            int hackathonCount = 1000;

            for (int i = 0; i < hackathonCount; i++)
            {
                var juniorsClone = juniors.Select(j => new Junior { Name = j.Name }).ToList();
                var teamLeadsClone = teamLeads.Select(tl => new TeamLead { Name = tl.Name }).ToList();

               var hackathon = new Hackathon.Model.Hackathon(juniorsClone, teamLeadsClone, _hrManager, _hrDirector);
                double harmonicity = hackathon.RunHackathon();
                totalHarmonicity += harmonicity;

                Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonicity:F2}");
            }

            double averageHarmonicity = totalHarmonicity / hackathonCount;
            Console.WriteLine($"\nСредняя гармоничность по {hackathonCount} хакатонам: {averageHarmonicity:F2}");
        }
    }
}
