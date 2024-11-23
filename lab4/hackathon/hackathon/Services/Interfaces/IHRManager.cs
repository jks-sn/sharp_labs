// File: Services/Interfaces/IHRManager.cs

using System.Collections.Generic;
using Hackathon.Model;

namespace Hackathon.Interface;

public interface IHRManager
{
    List<Team> AssignTeams(List<Junior> juniors, List<TeamLead> teamLeads);
}