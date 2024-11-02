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
        return MathUtils.ComputeHarmonicMean(satisfactionIndices);
    }

    public void AnalyzeResults(double harmonic)
    {
        
    }

    public void ProvideGuidance(IHRManager hrManager)
    {
        
    }
}