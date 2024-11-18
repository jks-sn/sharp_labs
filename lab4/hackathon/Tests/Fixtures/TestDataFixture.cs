// Tests/Fixtures/TestDataFixture.cs

using System.Collections.Generic;
using Hackathon.Model;
using Hackathon.Tests.Builder;

namespace Hackathon.Tests.Fixtures;

public class TestDataFixture
{
    public List<Junior> Juniors { get; }
    public List<TeamLead> TeamLeads { get; }

    public TestDataFixture()
    {
        Juniors = new List<Junior>
        {
            new JuniorBuilder().WithName("Junior1").Build(),
            new JuniorBuilder().WithName("Junior2").Build(),
            new JuniorBuilder().WithName("Junior3").Build()
        };

        TeamLeads = new List<TeamLead>
        {
            new TeamLeadBuilder().WithName("TeamLead1").Build(),
            new TeamLeadBuilder().WithName("TeamLead2").Build(),
            new TeamLeadBuilder().WithName("TeamLead3").Build()
        };
        
        var id = 1;
        foreach (var junior in Juniors)
        {
            junior.Id = id++;
        }
        foreach (var teamLead in TeamLeads)
        {
            teamLead.Id = id++;
        }
    }
}