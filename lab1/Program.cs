//Program.cs

using System;
using System.Collections.Immutable;
using System.Linq;
using Hackathon.Model;
using Hackathon.Strategy;
using Hackathon.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hackathon;
class Program
{
    static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        var services = new ServiceCollection();
        ImmutableList<string> abacaba;
        services.Configure<HackathonOptions>(configuration.GetSection("HackathonOptions"));
        services.Configure<HRManagerOptions>(configuration.GetSection("HRManagerOptions"));
        
        services.AddTransient<IAssignmentStrategy, GaleShapleyStrategy>();
        services.AddTransient<IAssignmentStrategy, RandomAssignmentStrategy>();
        
        services.AddSingleton<IAssignmentStrategyFactory, StrategyFactory>();        
        
        services.AddTransient<HRDirector>();
        services.AddTransient<HRManager>();
        
        var serviceProvider = services.BuildServiceProvider();

        var hackathonOptions = serviceProvider.GetService<IOptions<HackathonOptions>>().Value;
        var hrManagerOptions = serviceProvider.GetService<IOptions<HRManagerOptions>>().Value;
        
        var juniors = DataLoader.LoadJuniors(hackathonOptions.JuniorsFilePath);
        var teamLeads = DataLoader.LoadTeamLeads(hackathonOptions.TeamLeadsFilePath);

        
        var strategyFactory = serviceProvider.GetService<IAssignmentStrategyFactory>();
        var strategy = strategyFactory.GetStrategy(hrManagerOptions.AssignmentStrategy);
        
        var hrManager = new HRManager(strategy);


        double totalHarmonicity = 0;

        for (int i = 0; i < hackathonOptions.HackathonCount; i++)
        {
            var juniorsClone = juniors.Select(j => new Junior { Name = j.Name }).ToList();
            var teamLeadsClone = teamLeads.Select(tl => new TeamLead { Name = tl.Name }).ToList();

            var hackathon = new Model.Hackathon(juniorsClone, teamLeadsClone, hrManager);
            double harmonicity = hackathon.RunHackathon();
            totalHarmonicity += harmonicity;

            Console.WriteLine($"Хакатон {i + 1}: Гармоничность = {harmonicity:F2}");
        }

        double averageHarmonicity = totalHarmonicity / hackathonOptions.HackathonCount;
        Console.WriteLine($"\nСредняя гармоничность по {hackathonOptions.HackathonCount} хакатонам: {averageHarmonicity:F2}");
    }
}