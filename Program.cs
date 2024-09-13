
using Hackathon.Model;
using Hackathon.Strategy;

namespace Hackathon
{
    class Program
    {
        static void Main()
        {
            var juniors = DataLoader.LoadJuniors("./data/Juniors20.csv");
            var teamLeads = DataLoader.LoadTeamLeads("./data/Teamleads20.csv");
            
            HRDirector hrDirector = new HRDirector();
            IAssignmentStrategy strategy = new GaleShapleyStrategy();
            HRManager hrManager = new HRManager(strategy);

            double totalHarmonicity = 0;
            int hackathonCount = 1000;

            for (int i = 0; i < hackathonCount; i++)
            {
                var juniorsClone = juniors.Select(j => new Junior { Name = j.Name }).ToList();
                var teamLeadsClone = teamLeads.Select(tl => new TeamLead { Name = tl.Name }).ToList();

                Model.Hackathon hackathon = new Model.Hackathon(juniorsClone, teamLeadsClone, hrManager);
                double harmonicity = hackathon.RunHackathon();
                totalHarmonicity += harmonicity;

                Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonicity:F2}");
            }

            double averageHarmonicity = totalHarmonicity / hackathonCount;
            Console.WriteLine($"\nСредняя гармоничность по {hackathonCount} хакатонам: {averageHarmonicity:F2}");
        }
    }
}


//DataLoader
//Junior Teamled classes
//Wishlist
//