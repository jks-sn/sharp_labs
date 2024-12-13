namespace HRManagerService.Interfaces;

public interface ITeamBuildingOrchestrationService
{
    void OnHackathonStart(int hackathonId, int expectedCount);
    void OnDataReceived(int hackathonId);
    bool IsReadyToBuildTeams(int hackathonId);
    void BuildAndSendTeams(int hackathonId);
}