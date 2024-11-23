// Services/ConsoleInteractionService.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Hackathon.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hackathon.Services;
public class ConsoleInteractionService(IMediator mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
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
                    var harmonic = await mediator.Send(new RunHackathon(), stoppingToken);
                    Console.WriteLine($"Гармоничность хакатона: {harmonic:F2}");
                    break;
                case "2":
                    var hackathonIds = await mediator.Send(new GetHackathonIds(), stoppingToken);
                    if (hackathonIds.Count == 0)
                    {
                        Console.WriteLine("Нет доступных хакатонов.");
                        break;
                    }

                    Console.WriteLine("Доступные ID хакатонов:");
                    foreach (var id in hackathonIds)
                    {
                        Console.WriteLine($"- {id}");
                    }

                    Console.Write("Введите ID хакатона: ");
                    if (int.TryParse(Console.ReadLine(), out var hackathonId))
                    {
                        await mediator.Send(new PrintHackathonInfo(hackathonId), stoppingToken);
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ID.");
                    }
                    break;
                case "3":
                    await mediator.Send(new PrintAverageHarmonic(), stoppingToken);
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
}
