// Tests/Fixtures/TestDataFixture.cs

using System.Collections.Generic;
using Hackathon.Model;
using Hackathon.Tests.Builder;

namespace Hackathon.Tests.Fixtures;
public class TestDataFixture
{
    public List<Junior> Juniors { get; } =
    [
        new JuniorBuilder().WithName("Junior1").Build(),
        new JuniorBuilder().WithName("Junior2").Build(),
        new JuniorBuilder().WithName("Junior3").Build()
    ];

    public List<TeamLead> TeamLeads { get; } =
    [
        new TeamLeadBuilder().WithName("TeamLead1").Build(),
        new TeamLeadBuilder().WithName("TeamLead2").Build(),
        new TeamLeadBuilder().WithName("TeamLead3").Build()
    ];
}