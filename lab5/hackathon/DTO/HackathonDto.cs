// SharedModels/HackathonDto.cs

using System.Collections.Generic;

namespace Dto;

public record HackathonDto(int Id, double MeanSatisfactionIndex, List<ParticipantDto> Participants, List<WishlistDto> Wishlists, List<TeamDto> Teams);