//Tests/Fixtures/TestDataFixtures.cs

using Hackathon.Model;

namespace Hackathon.Tests.Fixtures;

public class TestDataFixture
{
    public List<Junior> Juniors { get; } = new()
    {
        new Junior { Name = "Junior1" },
        new Junior { Name = "Junior2" },
        new Junior { Name = "Junior3" }
    };

    public List<TeamLead> TeamLeads { get; } = new()
    {
        new TeamLead { Name = "TeamLead1" },
        new TeamLead { Name = "TeamLead2" },
        new TeamLead { Name = "TeamLead3" }
    };
}