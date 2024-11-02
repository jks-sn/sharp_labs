// Tests/Fixtures/TestDataFixtures.cs

using Hackathon.Model;
using Hackathon.Tests.Builder;

namespace Hackathon.Tests.Fixtures;

public class TestDataFixture
{
    public List<Junior> Juniors { get; }
    public List<TeamLead> TeamLeads { get; }

    public TestDataFixture()
    {
        // Используем билдеры для создания объектов
        Juniors = new List<Junior>
        {
            new JuniorBuilder()
                .WithName("Junior1")
                .WithWishList(new List<string> { "TeamLead1", "TeamLead2", "TeamLead3" })
                .WithSatisfactionIndex(3)
                .Build(),
            new JuniorBuilder()
                .WithName("Junior2")
                .WithWishList(new List<string> { "TeamLead2", "TeamLead1", "TeamLead3" })
                .WithSatisfactionIndex(3)
                .Build(),
            new JuniorBuilder()
                .WithName("Junior3")
                .WithWishList(new List<string> { "TeamLead3", "TeamLead1", "TeamLead2" })
                .WithSatisfactionIndex(3)
                .Build()
        };

        TeamLeads = new List<TeamLead>
        {
            new TeamLeadBuilder()
                .WithName("TeamLead1")
                .WithWishList(new List<string> { "Junior1", "Junior2", "Junior3" })
                .WithSatisfactionIndex(4)
                .Build(),
            new TeamLeadBuilder()
                .WithName("TeamLead2")
                .WithWishList(new List<string> { "Junior2", "Junior1", "Junior3" })
                .WithSatisfactionIndex(4)
                .Build(),
            new TeamLeadBuilder()
                .WithName("TeamLead3")
                .WithWishList(new List<string> { "Junior3", "Junior1", "Junior2" })
                .WithSatisfactionIndex(4)
                .Build()
        };
    }
}
