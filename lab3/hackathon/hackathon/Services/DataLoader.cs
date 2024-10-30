// Model/DataLoader.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Hackathon.Options;
using Hackathon.Model;
using Hackathon.Interface;
using Microsoft.Extensions.Options;

namespace Hackathon.Services;
public class DataLoader : IDataLoader
{
    private readonly IFileSystem _fileSystem;
    private readonly string _juniorsFilePath;
    private readonly string _teamLeadsFilePath;

    public DataLoader(IOptions<DataLoaderOptions> options, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _juniorsFilePath = options.Value.JuniorsFilePath ?? throw new ArgumentNullException(nameof(options.Value.JuniorsFilePath));
        _teamLeadsFilePath = options.Value.TeamLeadsFilePath ?? throw new ArgumentNullException(nameof(options.Value.TeamLeadsFilePath));
    }

    public List<Junior> LoadJuniors()
    {
        if (!_fileSystem.File.Exists(_juniorsFilePath))
        {
            throw new FileNotFoundException($"Файл не найден: {_juniorsFilePath}");
        }

        var lines = _fileSystem.File.ReadAllLines(_juniorsFilePath);      
        var juniors = new List<Junior>();

        foreach (var line in lines.Skip(1)) 
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(';');
            if (parts.Length < 2)
                continue;

            var name = parts[1].Trim();
            if (string.IsNullOrEmpty(name))
                continue;

            juniors.Add(new Junior { Name = name });
        }

        return juniors;
    }

    public List<TeamLead> LoadTeamLeads()
    {
        if (!_fileSystem.File.Exists(_teamLeadsFilePath))
        {
            throw new FileNotFoundException($"Файл не найден: {_teamLeadsFilePath}");

        }

        var lines = _fileSystem.File.ReadAllLines(_teamLeadsFilePath);
        var teamLeads = new List<TeamLead>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(';');
            if (parts.Length < 2)
                continue; 

            var name = parts[1].Trim();
            if (string.IsNullOrEmpty(name))
                continue;

            teamLeads.Add(new TeamLead { Name = name });
        }

        return teamLeads;
    }
}
