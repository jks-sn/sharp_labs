// DataLoader.cs
using Hackathon.Model;

namespace Hackathon.Model
{
    public class DataLoader
    {
        public List<Junior> LoadJuniors(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Console.WriteLine("Загруженные имена джунов:");
            return lines.Skip(1).Select(line =>
            {
                var parts = line.Split(';');
                var name = "";
                if(parts.Length >= 2) { 
                    name = parts[1].Trim();
                }
                return new Junior { Name = name };
            }).ToList();
        }

        public List<TeamLead> LoadTeamLeads(string filePath)
        {
            List<TeamLead> teamLeads = new List<TeamLead>();
            var lines = File.ReadAllLines(filePath);
            Console.WriteLine("Загруженные имена тимлидов:");
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    var name = parts[1].Trim();
                    teamLeads.Add(new TeamLead { Name = name });
                }
            }

            return teamLeads;
        }
    }
}
