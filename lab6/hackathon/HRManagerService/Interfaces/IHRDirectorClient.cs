// HRManagerService/Interfaces/IHRDirectorClient.cs

using System.Threading.Tasks;
using Dto;

namespace HRManagerService.Interfaces;
public interface IHRDirectorClient
{
    Task SendHackathonDataAsync(HackathonDto hackathonDto);
}