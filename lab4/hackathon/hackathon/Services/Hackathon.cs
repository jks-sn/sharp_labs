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
    private readonly IDataLoader _dataLoader;
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
        _dataLoader = dataLoader;
        _dbContext = dbContext;
    }
    
    public double Run()
    {
        var juniors = _dataLoader.LoadJuniors();
        var teamLeads = _dataLoader.LoadTeamLeads();
        
        var hackathonEvent = new HackathonEvent();
        
        _preferenceGenerator.GeneratePreferences(juniors,teamLeads);

        foreach (var junior in juniors)
        {
            junior.HackathonEvent = hackathonEvent;
            _dbContext.Juniors.Add(junior);
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.HackathonEvent = hackathonEvent;
            _dbContext.TeamLeads.Add(teamLead);
        }
        
        _dbContext.Hackathons.Add(hackathonEvent);
        _dbContext.SaveChanges();
        
        var teams = _hrManager.AssignTeams(juniors, teamLeads);
        
        foreach (var team in teams)
        {
            team.HackathonEvent = hackathonEvent;
            _dbContext.Teams.Add(team);

            team.Junior.CalculateSatisfactionIndex();
            team.TeamLead.CalculateSatisfactionIndex();

            _dbContext.Entry(team.Junior).State = EntityState.Modified;
            _dbContext.Entry(team.TeamLead).State = EntityState.Modified;
        }

        var allParticipants = juniors.Cast<Participant>().Concat(teamLeads).ToList();
        
        var harmonic = _hrDirector.EvaluateHackathon(allParticipants);
        
        hackathonEvent.Harmonic = harmonic;

        _dbContext.SaveChanges();

        return harmonic;
    }
}
