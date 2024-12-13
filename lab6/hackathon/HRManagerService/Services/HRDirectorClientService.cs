//HRManagerService/Services/HRDirectorClientService.cs

using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dto;
using Entities;
using HRManagerService.Clients;
using HRManagerService.Data;
using HRManagerService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagerService.Services;
public class HRDirectorClientService(
    HttpClient httpClient,
    ILogger<HRDirectorClientService> logger)
    : IHRDirectorClient
{
    public async Task SendHackathonDataAsync(HackathonDto hackathonDto)
    {
        var content = new StringContent(JsonSerializer.Serialize(hackathonDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/hr_director/hackathon", content);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Hackathon {Id} data sent to HRDirector", hackathonDto.HackathonId);
    }
}
