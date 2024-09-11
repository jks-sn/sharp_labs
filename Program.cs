
namespace Hackathon
{
    class Program
    {
        private const int hackathonCount = 1000;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Hackaton!");

            string juniorFilePath = "data/Juniors20.csv";
            string teamLeadFilePath = "data/Teamleads20.csv";

            Hackathon hackathon = new Hackathon(juniorFilePath, teamLeadFilePath);

            double totalHarmonicSatisfaction = 0;

            for (int i = 0; i < hackathonCount; i++)
            {
                var harmonicSatisfaction = hackathon.RunHackathon();
                totalHarmonicSatisfaction += harmonicSatisfaction;

                Console.WriteLine($"Hackathon {i + 1}: Harmonic Satisfaction = {harmonicSatisfaction:F2}");
            }

            Console.WriteLine($"Average Harmonic Satisfaction over {hackathonCount} Hackathons: {totalHarmonicSatisfaction / hackathonCount:F2}");
        }
    }
}
