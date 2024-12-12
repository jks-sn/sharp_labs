// SharedModels/HackathonDto.cs

using System.Collections.Generic;

namespace Dto;

public record HackathonDto(int HackathonId, double MeanSatisfactionIndex, List<ParticipantDto> Participants, List<WishlistDto> Wishlists, List<TeamDto> Teams);