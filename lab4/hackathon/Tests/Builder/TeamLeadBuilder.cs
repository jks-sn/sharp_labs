// Tests/Builder/TeamLeadBuilder.cs

using Hackathon.Model;

namespace Hackathon.Tests.Builder;

public class TeamLeadBuilder
{
    private string _name = "TeamLead";
    private List<Preference> _preferences = new List<Preference>();
    private int _satisfactionIndex = 0;

    public TeamLeadBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TeamLeadBuilder WithPreferences(List<Preference> preferences)
    {
        _preferences = preferences;
        return this;
    }

    public TeamLeadBuilder WithSatisfactionIndex(int index)
    {
        _satisfactionIndex = index;
        return this;
    }

    public TeamLead Build()
    {
        return new TeamLead
        {
            Name = _name,
            Preferences = _preferences,
            SatisfactionIndex = _satisfactionIndex
        };
    }
}