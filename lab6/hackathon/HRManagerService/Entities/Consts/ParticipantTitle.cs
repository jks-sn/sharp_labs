//Entities/Consts/ParticipantTitle.cs

namespace HRManagerService.Entities.Consts;
public enum ParticipantTitle
{
    TeamLead,
    Junior
}

public static class ParticipantTitleExtensions
{
    public static string ToString(this ParticipantTitle title)
    {
        return Enum.GetName(typeof(ParticipantTitle), title) ?? "Junior";
    }

    public static ParticipantTitle FromString(string title)
    {
        return Enum.TryParse<ParticipantTitle>(title, out var result) ? result : throw new ArgumentException("Invalid participant title");
    }
}