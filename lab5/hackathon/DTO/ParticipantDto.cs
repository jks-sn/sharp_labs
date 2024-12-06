//DTO/ParticipantDto.cs

using Entities.Consts;

namespace Dto;

public record ParticipantDto(int Id, ParticipantTitle Title, string Name);
