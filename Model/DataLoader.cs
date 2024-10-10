// Model/DataLoader.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hackathon.Options;
using Microsoft.Extensions.Options;

namespace Hackathon.Model;
public class DataLoader(IOptions<DataLoaderOptions> options)
{
    private readonly DataLoaderOptions _options = options.Value;
    public List<Junior> LoadJuniors()
    {
        var lines = File.ReadAllLines(_options.JuniorsFilePath);
        Console.WriteLine("Загруженные имена джунов:");
        return lines.Skip(1).Select(line =>
        {
            var parts = line.Split(';');
            var name = parts.Length >= 2 ? parts[1].Trim() : string.Empty;
            return new Junior { Name = name };
        }).ToList();
    }

    public List<TeamLead> LoadTeamLeads()
    {
        var lines = File.ReadAllLines(_options.TeamLeadsFilePath);
        Console.WriteLine("Загруженные имена тимлидов:");
        return lines.Skip(1).Select(line =>
        {
            var parts = line.Split(';');
            var name = parts.Length >= 2 ? parts[1].Trim() : string.Empty;
            return new TeamLead { Name = name };
        }).ToList();
    }
}
