// Strategy/IAssignmentStrategyFactory.cs
namespace Hackathon.Strategy;

public interface IAssignmentStrategyFactory
{
    IAssignmentStrategy GetStrategy(string strategyName);
}