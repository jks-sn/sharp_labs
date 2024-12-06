//DTO/WishlistDto.cs

using Entities.Consts;

namespace Dto;

public record WishlistDto(int ParticipantId, ParticipantTitle ParticipantTitle, List<int> DesiredParticipants);