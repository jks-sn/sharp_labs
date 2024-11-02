// Services/Hackathon.cs

using Hackathon.Data;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Preferences;
using Microsoft.EntityFrameworkCore;


namespace Hackathon.Services;

public class Hackathon : IHackathon
{
    private readonly IHRManager _hrManager;
    private readonly IHRDirector _hrDirector;
    private readonly IPreferenceGenerator _preferenceGenerator;
    private readonly List<Junior> _juniors;
    private readonly List<TeamLead> _teamLeads;
    private readonly HackathonDbContext _dbContext;
    
    public Hackathon(
        IHRManager hrManager,
        IHRDirector hrDirector,
        IDataLoader dataLoader,
        IPreferenceGenerator preferenceGenerator,
        HackathonDbContext dbContext)
    {
        _hrManager = hrManager;
        _hrDirector = hrDirector;
        _preferenceGenerator = preferenceGenerator;
        _dbContext = dbContext;
        
        _juniors = dataLoader.LoadJuniors();
        _teamLeads = dataLoader.LoadTeamLeads();
    }
    
    public double Run()
    {
        var hackathonEvent = new HackathonEvent();
        
        _preferenceGenerator.GeneratePreferences(_juniors,_teamLeads);

        foreach (var junior in _juniors)
        {
            junior.HackathonEvent = hackathonEvent;
            _dbContext.Juniors.Add(junior);
        }

        foreach (var teamLead in _teamLeads)
        {
            teamLead.HackathonEvent = hackathonEvent;
            _dbContext.TeamLeads.Add(teamLead);
        }
        
        _dbContext.Hackathons.Add(hackathonEvent);
        _dbContext.SaveChanges();
        
        var teams = _hrManager.AssignTeams(_juniors, _teamLeads);
        
        foreach (var team in teams)
        {
            team.HackathonEvent = hackathonEvent;
            _dbContext.Teams.Add(team);

            team.Junior.CalculateSatisfactionIndex();
            team.TeamLead.CalculateSatisfactionIndex();

            _dbContext.Entry(team.Junior).State = EntityState.Modified;
            _dbContext.Entry(team.TeamLead).State = EntityState.Modified;
        }

        var allParticipants = _juniors.Cast<Participant>().Concat(_teamLeads).ToList();
        
        var harmonic = _hrDirector.EvaluateHackathon(allParticipants);
        
        hackathonEvent.Harmonic = harmonic;

        _dbContext.SaveChanges();

        return harmonic;
    }
}
