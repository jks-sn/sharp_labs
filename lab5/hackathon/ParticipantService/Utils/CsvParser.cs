//ParticipantServer/Utils/CsvParser.cs

using Entities;
using Entities.Consts;

namespace ParticipantService.Utils;
public static class CsvParser
{
    public static IEnumerable<Participant> ParseCsvFileWithParticipants(string filePath, ParticipantTitle title)
    {
        var participantsList = new List<Participant>();
        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var tokens = line.Split(';');
            if (tokens.Length < 2)
                continue;

            if (!int.TryParse(tokens[0], out var id))
                continue;

            var name = tokens[1];
            participantsList.Add(new Participant(id, title, name));
        }
        return participantsList;
    }
}