// Strategy/AssignmentStrategyFactory.cs

using System;
using System.Collections.Generic;
using System.Linq;


namespace Hackathon.Strategy;
public class StrategyFactory(IEnumerable<IAssignmentStrategy> strategies) : IAssignmentStrategyFactory
{
    private readonly IEnumerable<IAssignmentStrategy> _strategies = strategies;

    public IAssignmentStrategy GetStrategy(string strategyName)
    {
        var strategy = _strategies.FirstOrDefault(s => s.GetType().Name == strategyName);
        
        if (strategy == null)
        {
            throw new NotSupportedException($"Стратегия с именем {strategyName} не найдена.");
        }

        return strategy;
    }
}