// Preferences/RandomPreferenceGenerator.cs

using Hackathon.Model;

namespace Hackathon.Preferences;

public class RandomPreferenceGenerator : IPreferenceGenerator
{
    public void GeneratePreferences(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        var random = new Random();

        foreach (var junior in juniors)
        {
            junior.Preferences.Clear();
            var shuffledTeamLeads = teamLeads.OrderBy(x => random.Next()).ToList();
            var rank = 1;
            foreach (var teamLead in shuffledTeamLeads)
            {
                var preference = new Preference
                {
                    Participant = junior,
                    PreferredName = teamLead.Name,
                    Rank = rank++
                };
                junior.Preferences.Add(preference);
            }
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Preferences.Clear();
            var shuffledJuniors = juniors.OrderBy(x => random.Next()).ToList();
            int rank = 1;
            foreach (var junior in shuffledJuniors)
            {
                var preference = new Preference
                {
                    Participant = teamLead,
                    PreferredName = junior.Name,
                    Rank = rank++
                };
                teamLead.Preferences.Add(preference);
            }
        }
    }
}