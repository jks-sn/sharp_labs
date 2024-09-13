// Hackathon.cs
using Hackathon.Model;

namespace Hackathon.Model
{
    public class Hackathon(List<Junior> juniors, List<TeamLead> teamLeads, HRManager hrManager)
    {
        private readonly List<Junior> _juniors = juniors;
        private readonly List<TeamLead> _teamLeads = teamLeads;
        private readonly HRManager _hrManager = hrManager;

        public double RunHackathon()
        {
            GenerateRandomPreferences();

            var pairs = _hrManager.AssignTeams(_juniors, _teamLeads);

            foreach (var pair in pairs)
            {
                pair.Item1.AssignedPartner = pair.Item2.Name;
                pair.Item2.AssignedPartner = pair.Item1.Name;
                //Console.WriteLine($"Джун: {pair.Item1.Name} - Тимлид: {pair.Item2.Name}");
                pair.Item1.CalculateSatisfactionIndex();
                pair.Item2.CalculateSatisfactionIndex();
            }

            List<Participant> allParticipants = new List<Participant>();
            allParticipants.AddRange(_juniors);
            allParticipants.AddRange(_teamLeads);

            return HRDirector.ComputeHarmonicity(allParticipants);
        }

        private void GenerateRandomPreferences()
        {
            Random rnd = new Random();

            foreach (var junior in _juniors)
            {
                junior.WishList = _teamLeads.Select(tl => tl.Name).OrderBy(x => rnd.Next()).ToList();
            }

            foreach (var teamLead in _teamLeads)
            {
                teamLead.WishList = _juniors.Select(j => j.Name).OrderBy(x => rnd.Next()).ToList();
            }
        }
    }
}
