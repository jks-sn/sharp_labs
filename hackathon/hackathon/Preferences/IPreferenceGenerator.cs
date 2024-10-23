//Preferences/IPreferenceGenerator

using Hackathon.Model;

namespace Hackathon.Preferences;

public interface IPreferenceGenerator
{
    void GeneratePreferences(List<Junior> juniors, List<TeamLead> teamLeads);
}