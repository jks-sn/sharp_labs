// Model/Hackathon.cs

namespace Hackathon.Model;
public class Hackathon(HRManager hrManager, HRDirector hrDirector)
{
    private readonly List<Junior> _juniors = hrDirector.LoadJuniors();
    private readonly List<TeamLead> _teamLeads = hrDirector.LoadTeamLeads();
    public double Run()
    {
        GenerateRandomPreferences();

        var teams = hrManager.AssignTeams(_juniors, _teamLeads);

        foreach (var team in teams)
        {
            team.Junior.CalculateSatisfactionIndex();
            team.TeamLead.CalculateSatisfactionIndex();
        }

        List<Participant> allParticipants = [.. _juniors, .. _teamLeads];

        return hrDirector.ComputeHarmonic(allParticipants);
    }

    private void GenerateRandomPreferences()
    {
        var random = new Random();

        foreach (var junior in _juniors)
        {
            junior.WishList = _teamLeads.Select(tl => tl.Name).OrderBy(x => random.Next()).ToList();
        }

        foreach (var teamLead in _teamLeads)
        {
            teamLead.WishList = _juniors.Select(j => j.Name).OrderBy(x => random.Next()).ToList();
        }
    }
}
