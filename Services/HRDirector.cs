// Model/HRDirector.cs

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
        // Логика анализа результатов
    }

    public void ProvideGuidance(IHRManager hrManager)
    {
        // Логика предоставления рекомендаций HRManager-у
    }
}