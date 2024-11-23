// Services/HRDirector.cs

using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Interface;
using Hackathon.Utilities;

namespace Hackathon.Services;
public class HRDirector : IHRDirector
{
    public double EvaluateHackathon(IEnumerable<Participant> participants)
    {
        var satisfactionIndices = participants.Select(p => p.SatisfactionIndex);
        
        var zeroIndices = participants.Where(p => p.SatisfactionIndex == 0).ToList();
        
        if (zeroIndices.Any())
        {
            foreach (var participant in zeroIndices)
            {
                Console.WriteLine($"Участник с ID {participant.Id} имеет SatisfactionIndex равный нулю.");
            }
        }
        
        return MathUtils.ComputeHarmonicMean(satisfactionIndices);
    }

    public void AnalyzeResults(double harmonic)
    {
        
    }

    public void ProvideGuidance(IHRManager hrManager)
    {
        
    }
}