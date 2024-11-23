//Services/Interfaces/IHRDirector.cs

using Hackathon.Model;
using Hackathon.Services;

namespace Hackathon.Interface;

public interface IHRDirector
{
    public double EvaluateHackathon(IEnumerable<Participant> participants);
    public void AnalyzeResults(double harmonic);
    public void ProvideGuidance(IHRManager hrManager);
}