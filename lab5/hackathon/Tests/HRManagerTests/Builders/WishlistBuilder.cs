// Tests/HRManagerTests/Builders/WishlistBuilder.cs
using Entities;

namespace HRManagerTests.Builders;

public class WishlistBuilder
{
    private int _id = 1;
    private int _participantId = 1;
    private Entities.Consts.ParticipantTitle _participantTitle = Entities.Consts.ParticipantTitle.Junior;
    private List<int> _desiredParticipants = new() { 2, 3, 4 };

    public WishlistBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public WishlistBuilder WithParticipantId(int participantId)
    {
        _participantId = participantId;
        return this;
    }

    public WishlistBuilder WithParticipantTitle(Entities.Consts.ParticipantTitle title)
    {
        _participantTitle = title;
        return this;
    }

    public WishlistBuilder WithDesiredParticipants(List<int> desired)
    {
        _desiredParticipants = desired;
        return this;
    }

    public Wishlist Build()
    {
        return new Wishlist
        {
            Id = _id,
            ParticipantId = _participantId,
            ParticipantTitle = _participantTitle,
            DesiredParticipants = _desiredParticipants
        };
    }
}