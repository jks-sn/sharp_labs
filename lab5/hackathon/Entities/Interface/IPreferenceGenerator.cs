//Preferences/IPreferenceGenerator.cs
namespace Entities.Interface;

public interface IPreferenceGenerator
{
    void GeneratePreferences(List<Participant> juniors, List<Participant> teamLeads);
}