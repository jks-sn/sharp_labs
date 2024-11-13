// Program.cs

using System.IO.Abstractions;
using System.Threading.Tasks;
using Hackathon.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Hackathon.HostedServices;
using Hackathon.Interface;
using Hackathon.Strategy;
using Hackathon.Options;
using Hackathon.Preferences;
using Hackathon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hackathon;
class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await RunApplicationAsync(host);
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var defaultBuilder = Host.CreateDefaultBuilder(args);
        defaultBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.AddFilter("Microsoft", LogLevel.Warning);
                logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            });
        defaultBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });
        defaultBuilder.ConfigureServices((context, services) =>
            {
                services.Configure<DataLoaderOptions>(context.Configuration.GetSection("DataLoaderOptions"));
                services.Configure<HackathonOptions>(context.Configuration.GetSection("HackathonOptions"));
                services.Configure<HRManagerOptions>(context.Configuration.GetSection("HRManagerOptions"));
                services.Configure<ConnectionOptions>(context.Configuration.GetSection("ConnectionOptions"));

                services.AddSingleton<IFileSystem, FileSystem>();

                services.AddSingleton<IDataLoader, DataLoader>();
                services.AddSingleton<IHRDirector, HRDirector>();
                services.AddSingleton<IHRManager, HRManager>();
                services.AddScoped<IHackathon, Services.Hackathon>();
                services.AddTransient<IAssignmentStrategy, GaleShapleyStrategy>();
                services.AddTransient<IAssignmentStrategy, RandomAssignmentStrategy>();
                services.AddSingleton<IAssignmentStrategyFactory, StrategyFactory>();

                services.AddSingleton<IPreferenceGenerator, RandomPreferenceGenerator>();

                services.AddScoped<HackathonPrinter>();

                services.AddDbContext<HackathonDbContext>((serviceProvider, optionsBuilder) =>
                {
                    var connectionOptions = serviceProvider.GetRequiredService<IOptions<ConnectionOptions>>().Value;
                    optionsBuilder.UseNpgsql(connectionOptions.DefaultConnection)
                        .EnableDetailedErrors(false)
                        .EnableSensitiveDataLogging(false);
                });
            });
        return defaultBuilder;
    }

    private static async Task RunApplicationAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var hackathonService = serviceProvider.GetRequiredService<IHackathon>();
        var hackathonPrinter = serviceProvider.GetRequiredService<HackathonPrinter>();

        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Провести один хакатон со случайными предпочтениями, сохранить условия и рассчитанную гармоничность в БД");
            Console.WriteLine("2 - Распечатать список участников, сформированные команды и рассчитанную гармоничность по идентификатору хакатона");
            Console.WriteLine("3 - Посчитать и распечатать среднюю гармоничность по всем хакатонам в БД");
            Console.WriteLine("0 - Выход");

            Console.Write("Ваш выбор: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await RunHackathonAsync(hackathonService);
                    break;
                case "2":
                    await PrintHackathonInfoAsync(hackathonPrinter);
                    break;
                case "3":
                    await PrintAverageHarmonicAsync(hackathonPrinter);
                    break;
                case "0":
                    Console.WriteLine("Выход из программы.");
                    return;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static Task RunHackathonAsync(IHackathon hackathonService)
    {
        var harmonic = hackathonService.Run();
        Console.WriteLine($"Хакатон проведён. Гармоничность = {harmonic:F2}");
        return Task.CompletedTask;
    }

    private static Task PrintHackathonInfoAsync(HackathonPrinter hackathonPrinter)
    {
        Console.Write("Введите ID хакатона: ");
        if (int.TryParse(Console.ReadLine(), out int hackathonId))
        {
            hackathonPrinter.PrintHackathonById(hackathonId);
        }
        else
        {
            Console.WriteLine("Некорректный ID.");
        }

        return Task.CompletedTask;
    }

    private static Task PrintAverageHarmonicAsync(HackathonPrinter hackathonPrinter)
    {
        hackathonPrinter.PrintAverageHarmonic();
        return Task.CompletedTask;
    }
}