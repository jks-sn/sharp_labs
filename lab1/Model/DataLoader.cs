// DataLoader.cs
using Hackathon.Model;

namespace Hackathon.Model
{
    public static class DataLoader
    {
        public static List<Junior> LoadJuniors(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Console.WriteLine("Загруженные имена джунов:");
            return lines.Skip(1).Select(line =>
            {
                var parts = line.Split(';');
                var name = parts[1].Trim();
                
                //Console.WriteLine($"Имя: '{name}' (длина: {name.Length})");

                return new Junior { Name = name };
            }).ToList();
        }

        public static List<TeamLead> LoadTeamLeads(string filePath)
        {
            List<TeamLead> teamLeads = new List<TeamLead>();
            var lines = File.ReadAllLines(filePath);
            Console.WriteLine("Загруженные имена тимлидов:");
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                var name = parts[1].Trim();
                //Console.WriteLine($"Имя: '{name}' (длина: {name.Length})");
                teamLeads.Add(new TeamLead { Name = name });
            }

            return teamLeads;
        }
    }
}
