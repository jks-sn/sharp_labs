// HRManagerService/Clients/IHRDirectorApi.cs

using System.Threading.Tasks;
using Dto;
using Refit;

namespace HRManagerService.Clients;

public interface IHRDirectorApi
{
    [Post("/api/hr_director/hackathon")]
    Task SendHackathonDataAsync([Body] TeamsPayloadDto teamsPayloadDto);
}