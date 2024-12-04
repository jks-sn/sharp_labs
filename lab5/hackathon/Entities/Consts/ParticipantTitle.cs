namespace Entities.Consts
{
    public enum ParticipantTitle
    {
        TeamLead,
        Junior
    }

    public static class ParticipantTitleExtensions
    {
        public static string ToString(this ParticipantTitle title)
        {
            return title.ToString();
        }

        public static ParticipantTitle FromString(string title)
        {
            return Enum.TryParse(title, out ParticipantTitle result) ? result : throw new ArgumentException("Invalid participant title");
        }
    }
}