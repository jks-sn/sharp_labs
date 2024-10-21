// Model/HRDirector.cs

using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Utilities;
namespace Hackathon.Services
{
    public class HRDirector
    {
        public double EvaluateHackathon(IEnumerable<Participant> participants)
        {
            var satisfactionIndices = participants.Select(p => p.SatisfactionIndex);
            return MathUtils.ComputeHarmonicMean(satisfactionIndices);
        }

        public void AnalyzeResults(double harmonicity)
        {
            // Логика анализа результатов
        }

        public void ProvideGuidance(HRManager hrManager)
        {
            // Логика предоставления рекомендаций HRManager-у
        }
    }
}