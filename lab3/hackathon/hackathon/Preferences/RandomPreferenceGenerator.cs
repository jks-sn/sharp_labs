//Prefrences/RandomPreferenceGenerator.cs

using Hackathon.Model;

namespace Hackathon.Preferences;

public class RandomPreferenceGenerator : IPreferenceGenerator
{
    public void GeneratePreferences(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        var random = new Random();

        foreach (var junior in juniors)
        {
            junior.WishList = teamLeads
                .Select(tl => tl.Name)
                .OrderBy(x => random.Next())
                .ToList();
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.WishList = juniors
                .Select(j => j.Name)
                .OrderBy(x => random.Next())
                .ToList();
        }
    }
}