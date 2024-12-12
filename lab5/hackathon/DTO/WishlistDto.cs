//DTO/WishlistDto.cs

using Entities.Consts;

namespace Dto;

public record WishlistDto(int ParticipantId, string ParticipantTitle, List<int> DesiredParticipants);