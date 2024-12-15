namespace Messages;

public interface IParticipantWithWishlist
{
    int ParticipantId { get; }
    string ParticipantTitle { get; }
    string ParticipantName { get; }
    int HackathonId { get; }
    int[] DesiredParticipants { get; }
}