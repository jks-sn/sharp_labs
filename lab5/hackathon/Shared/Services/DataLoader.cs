using Entities;
using Entities.Consts;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Utils;

namespace Shared.Services;

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