namespace Messages;

public interface IWishlistInfo
{
    int ParticipantId { get; }
    string ParticipantTitle { get; }
    int HackathonId { get; }
    int[] DesiredParticipants { get; }
}