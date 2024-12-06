// HRManagerService/Interfaces/IHRDirectorClient.cs

using System.Threading.Tasks;

namespace HRManagerService.Interfaces;
public interface IHRDirectorClient
{
    Task SendHackathonDataAsync(int hackathonId);
}