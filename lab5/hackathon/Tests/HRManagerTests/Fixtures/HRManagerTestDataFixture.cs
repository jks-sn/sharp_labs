// Tests/HRManagerTests/Fixtures/HRManagerTestDataFixture.cs
using HRManagerTests.Builders;
using Entities;
using Entities.Consts;

namespace HRManagerTests.Fixtures;

public class HRManagerTestDataFixture
{
    public List<Participant> Participants { get; }
    public List<Wishlist> Wishlists { get; }

    public HRManagerTestDataFixture()
    {
        // Создаем участников
        Participants = new List<Participant>
        {
            new ParticipantBuilder().WithId(1).WithName("Junior1").WithTitle(ParticipantTitle.Junior).Build(),
            new ParticipantBuilder().WithId(2).WithName("TeamLead1").WithTitle(ParticipantTitle.TeamLead).Build()
        };
        
        Wishlists = new List<Wishlist>
        {
            new WishlistBuilder()
                .WithId(1)
                .WithParticipantId(2)
                .WithParticipantTitle(ParticipantTitle.TeamLead)
                .WithDesiredParticipants(new List<int>{1})
                .Build()
        };
        
        // Привязка к участникам
        Participants[1].Wishlists = Wishlists.Where(w => w.ParticipantId == 2).ToList();
    }
}