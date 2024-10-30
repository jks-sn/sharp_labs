// HRDirector.cs

using Hackathon.Model;

namespace Hackathon.Model
{
    public class HRDirector
    {
        public double ComputeHarmonicity(List<Participant> participants)
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