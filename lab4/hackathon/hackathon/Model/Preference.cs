// Model/Preference.cs

namespace Hackathon.Model;

public class Preference
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }
    public Participant Participant { get; set; }

    public string PreferredName { get; set; }

    public int Rank { get; set; }
}