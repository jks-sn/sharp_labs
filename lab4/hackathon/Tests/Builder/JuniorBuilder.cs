// Tests/Builder/JuniorBuilder.cs

using Hackathon.Model;

namespace Hackathon.Tests.Builder;
public class JuniorBuilder
{
    private string _name = "Junior";
    private List<Preference> _preferences = new List<Preference>();
    private int _satisfactionIndex = 0;

    public JuniorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public JuniorBuilder WithPreferences(List<Preference> preferences)
    {
        _preferences = preferences;
        return this;
    }

    public JuniorBuilder WithSatisfactionIndex(int index)
    {
        _satisfactionIndex = index;
        return this;
    }

    public Junior Build()
    {
        return new Junior
        {
            Name = _name,
            Preferences = _preferences,
            SatisfactionIndex = _satisfactionIndex
        };
    }
}