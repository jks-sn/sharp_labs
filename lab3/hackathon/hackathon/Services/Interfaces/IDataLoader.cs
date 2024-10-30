// File: Services/Interfaces/IDataLoader.cs

using System.Collections.Generic;
using Hackathon.Model;

namespace Hackathon.Interface;

public interface IDataLoader
{ 
    List<Junior> LoadJuniors();
    List<TeamLead> LoadTeamLeads();
}