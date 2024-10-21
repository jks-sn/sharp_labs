// Services/Hackathon.cs

using Hackathon.Model;
using Hackathon.Preferences;


namespace Hackathon.Services;

public class Hackathon
{
    private readonly HRManager _hrManager;
    private readonly HRDirector _hrDirector;
    private readonly IPreferenceGenerator _preferenceGenerator;
    private readonly List<Junior> _juniors;
    private readonly List<TeamLead> _teamLeads;
    
    public Hackathon(
        HRManager hrManager,
        HRDirector hrDirector,
        DataLoader dataLoader,
        IPreferenceGenerator preferenceGenerator)
    {
        _hrManager = hrManager;
        _hrDirector = hrDirector;
        _preferenceGenerator = preferenceGenerator;
        _juniors = dataLoader.LoadJuniors();
        _teamLeads = dataLoader.LoadTeamLeads();
    }
    
    public double Run()
    {
        _preferenceGenerator.GeneratePreferences(_juniors,_teamLeads);

        var teams = _hrManager.AssignTeams(_juniors, _teamLeads);

        foreach (var team in teams)
        {
            team.Junior.CalculateSatisfactionIndex();
            team.TeamLead.CalculateSatisfactionIndex();
        }

        List<Participant> allParticipants = _juniors.Cast<Participant>().Concat(_teamLeads).ToList();
        
        double harmonic = _hrDirector.EvaluateHackathon(allParticipants);
        
        _hrDirector.AnalyzeResults(harmonic);
        _hrDirector.ProvideGuidance(_hrManager);

        return harmonic;
    }
}
