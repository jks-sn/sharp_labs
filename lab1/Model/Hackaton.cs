// Hackathon.cs

using Hackathon.Model;

namespace Hackathon.Model
{
    public class Hackathon(List<Junior> juniors, List<TeamLead> teamLeads, HRManager hrManager, HRDirector hrDirector)
    {
        private readonly List<Junior> _juniors = juniors;
        private readonly List<TeamLead> _teamLeads = teamLeads;
        private readonly HRManager _hrManager = hrManager;
        private readonly HRDirector _hrDirector = hrDirector;

        public double RunHackathon()
        {
            GenerateRandomPreferences();

            var teams = _hrManager.AssignTeams(_juniors, _teamLeads);

            foreach (var team in teams)
            {

                team.Junior.CalculateSatisfactionIndex();
                team.TeamLead.CalculateSatisfactionIndex();
            }

            List<Participant> allParticipants = [.. _juniors, .. _teamLeads];

            return _hrDirector.ComputeHarmonicity(allParticipants);
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
