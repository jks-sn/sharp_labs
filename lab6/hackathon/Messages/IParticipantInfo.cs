namespace Messages;

public interface IParticipantInfo
{
    int Id { get; }
    string Title { get; }
    string Name { get; }
    int HackathonId { get; }
}