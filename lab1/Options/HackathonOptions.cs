//Options/HackathonOptions

namespace Hackathon.Options
{
    public class HackathonOptions
    {
        public string JuniorsFilePath => "./data/Juniors20.csv";
        public string TeamLeadsFilePath => "./data/Teamleads20.csv";
        public int HackathonCount { get; set; } = 1000;
    }
}
