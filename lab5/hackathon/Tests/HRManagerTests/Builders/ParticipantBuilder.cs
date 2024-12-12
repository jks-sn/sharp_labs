// Tests/HRManagerTests/Builders/ParticipantBuilder.cs
using Entities.Consts;
using Entities;

namespace HRManagerTests.Builders;

public class ParticipantBuilder
{
    private int _id = 1;
    private string _name = "Test Participant";
    private ParticipantTitle _title = ParticipantTitle.Junior;
    private int? _hackathonId = null;

    public ParticipantBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ParticipantBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ParticipantBuilder WithTitle(ParticipantTitle title)
    {
        _title = title;
        return this;
    }

    public ParticipantBuilder WithHackathonId(int? hackathonId)
    {
        _hackathonId = hackathonId;
        return this;
    }

    public Participant Build()
    {
        return new Participant
        {
            Id = _id,
            Name = _name,
            Title = _title,
            HackathonId = _hackathonId,
            Wishlists = new List<Wishlist>()
        };
    }
}