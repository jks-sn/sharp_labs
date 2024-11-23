// Tests/Fixtures/TestDataLoader.cs

using System.Collections.Generic;
using Hackathon.Interface;
using Hackathon.Model;

namespace Hackathon.Tests.Fixtures;

public class TestDataLoader(List<Junior> juniors, List<TeamLead> teamLeads) : IDataLoader
{
    public List<Junior> LoadJuniors()
    {
        return juniors;
    }

    public List<TeamLead> LoadTeamLeads()
    {
        return teamLeads;
    }
}