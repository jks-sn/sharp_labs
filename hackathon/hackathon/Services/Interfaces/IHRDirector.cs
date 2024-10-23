using Hackathon.Model;
using Hackathon.Services;

namespace Hackathon.Interface;

public interface IHRDirector
{
    public decimal EvaluateHackathon(IEnumerable<Participant> participants);
    public void AnalyzeResults(decimal harmonic);
    public void ProvideGuidance(IHRManager hrManager);
}