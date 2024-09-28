// Strategy/AssignmentStrategyFactory.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Hackathon.Options;
using Microsoft.Extensions.Options;

namespace Hackathon.Strategy;
public class StrategyFactory(IEnumerable<IAssignmentStrategy> strategies) : IAssignmentStrategyFactory
{
    private readonly IEnumerable<IAssignmentStrategy> _strategies = strategies;

    public IAssignmentStrategy GetStrategy(string strategyName)
    {
        IAssignmentStrategy? strategy = _strategies.FirstOrDefault(s => s.GetType().Name == strategyName);

        if (strategy == null)
        {
            throw new NotSupportedException($"Стратегия с именем {strategyName} не найдена.");
        }

        return strategy;
    }
}