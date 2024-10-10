// Model/HRDirector.cs

using Hackathon.Model;
using Hackathon.Options;
namespace Hackathon.Model
{
    public class HRDirector(DataLoader dataLoader)
    {
        private readonly DataLoader _dataLoader = dataLoader;
        public List<Junior> LoadJuniors()
        {
            return _dataLoader.LoadJuniors();
        }
        public List<TeamLead> LoadTeamLeads()
        {
            return _dataLoader.LoadTeamLeads();
        }
        public double ComputeHarmonic(List<Participant> participants)
        {
            int n = participants.Count;
            double denominator = 0;

            foreach (var participant in participants)
            {
                denominator += 1.0 / participant.SatisfactionIndex;
            }

            return n / denominator;
        }
    }
}