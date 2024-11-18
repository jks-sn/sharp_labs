// Tests/Fixtures/FixedPreferenceGenerator.cs

using System.Collections.Generic;
using Hackathon.Interface;
using Hackathon.Model;
using Hackathon.Preferences;

namespace Hackathon.Tests.Fixtures;

public class FixedPreferenceGenerator : IPreferenceGenerator
{
    public void GeneratePreferences(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        foreach (var junior in juniors)
        {
            junior.Preferences = new List<Preference>();
            foreach (var teamLead in teamLeads)
            {
                junior.Preferences.Add(new Preference
                {
                    ParticipantId = junior.Id,
                    PreferredName = teamLead.Name,
                    Rank = teamLeads.IndexOf(teamLead) + 1
                });
            }
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Preferences = new List<Preference>();
            foreach (var junior in juniors)
            {
                teamLead.Preferences.Add(new Preference
                {
                    ParticipantId = teamLead.Id,
                    PreferredName = junior.Name,
                    Rank = juniors.IndexOf(junior) + 1
                });
            }
        }
    }
}