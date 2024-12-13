//ParticipantService/Services/DataLoader.cs

using Microsoft.Extensions.Options;
using ParticipantService.Entities;
using ParticipantService.Entities.Consts;
using ParticipantService.Options;
using ParticipantService.Utils;

namespace ParticipantService.Services;

public class DataLoader(IOptions<DataLoaderOptions> options)
{
    private readonly DataLoaderOptions _options = options.Value;

    public List<Participant> LoadProbableTeammates(ParticipantTitle title)
    {
        var filePath = title switch
        {
            ParticipantTitle.TeamLead => _options.TeamLeadsFilePath,
            ParticipantTitle.Junior => _options.JuniorsFilePath,
            _ => throw new ArgumentException("Invalid ParticipantTitle", nameof(title))
        };

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV file not found at path: {filePath}");
        }
        return CsvParser.ParseCsvFileWithParticipants(filePath, title).ToList();
    }
}