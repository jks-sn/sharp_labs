//DTO/WishlistDto.cs

namespace Dto;

public record WishlistDto(int ParticipantId, string ParticipantTitle, int[] DesiredParticipants);