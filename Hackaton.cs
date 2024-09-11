using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hackathon
{
    public class Hackathon
    {
        private List<string> Juniors { get; set; }
        private List<string> TeamLeads { get; set; }

        private Dictionary<string, List<string>> JuniorWishlists { get; set; }
        private Dictionary<string, List<string>> TeamLeadWishlists { get; set; }
        public Hackathon(string juniorFilePath, string teamLeadFilePath)
        {
            Juniors = LoadParticipants(juniorFilePath);
            TeamLeads = LoadParticipants(teamLeadFilePath);

            JuniorWishlists = GenerateRandomWishlists(Juniors, TeamLeads);
            TeamLeadWishlists = GenerateRandomWishlists(TeamLeads, Juniors);
        }
        private static List<string> LoadParticipants(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); // Пропуск заголовка
            return lines.Select(line => line.Split(';')[1]).ToList(); // Получаем ФИО без номера
        }

        private static Dictionary<string, List<string>> GenerateRandomWishlists(List<string> participants, List<string> preferences)
        {
            var random = new Random();
            var wishlists = new Dictionary<string, List<string>>();

            foreach (var participant in participants)
            {
                var shuffledPreferences = preferences.OrderBy(x => random.Next()).ToList();
                wishlists[participant] = shuffledPreferences;
            }

            return wishlists;
        }
        public double RunHackathon()
        {
            var availableTeamLeads = new List<string>(TeamLeadWishlists.Keys);
            var random = new Random();
            double sumOfRefSatisfaction = 0;
            int totalParticipants = JuniorWishlists.Count * 2;
            int pairCount = JuniorWishlists.Count;

            foreach (var junior in JuniorWishlists.Keys)
            {
                var teamLead = availableTeamLeads.OrderBy(x => random.Next()).First();
                availableTeamLeads.Remove(teamLead);                          // надо покопать метод, чтобы сразу удалять

                var (teamLeadSatisfaction, juniorSatisfaction) = CalculatePairSatisfaction(junior, teamLead);
                sumOfRefSatisfaction += 1.0 / teamLeadSatisfaction;
                sumOfRefSatisfaction += 1.0 / juniorSatisfaction;
            }
            return totalParticipants / sumOfRefSatisfaction;
        }

        private (double, double) CalculatePairSatisfaction(string junior, string teamLead)
        {

                int teamLeadSatisfaction = 20 - TeamLeadWishlists[teamLead].IndexOf(junior);

                int juniorSatisfaction = 20 - JuniorWishlists[junior].IndexOf(teamLead);

                return (teamLeadSatisfaction, juniorSatisfaction);
        }
    }
}
