using HRManagerService;
using Entities;
using Entities.Interface;
using HRManagerService.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию из переменных окружения и файла appsettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настраиваем Kestrel на порт 8082
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082);
});

builder.Services.Configure<ControllerOptions>(builder.Configuration.GetSection("ControllerOptions"));

// Регистрируем HttpClient для взаимодействия с HRDirectorService
builder.Services.AddHttpClient(nameof(HRManagerBackgroundService), client =>
{
    var hrDirectorUri = builder.Configuration["HrDirectorUri"];
    if (!Uri.TryCreate(hrDirectorUri, UriKind.Absolute, out var uriResult))
    {
        throw new Exception("HrDirectorUri is not a valid URI");
    }
    client.BaseAddress = uriResult;
});

// Регистрируем сервисы
builder.Services.AddSingleton<ITeamBuildingStrategy, GaleShapleyStrategy>();
builder.Services.AddSingleton<HRManager>();
builder.Services.AddSingleton<HRManagerService.HRManagerService>(provider =>
{
    var hrManager = provider.GetRequiredService<HRManager>();
    var logger = provider.GetRequiredService<ILogger<HRManagerService.HRManagerService>>();
    var options = provider.GetRequiredService<IOptions<ControllerOptions>>();
    return new HRManagerService.HRManagerService(hrManager, options.Value.ParticipantsNumber, logger);
});

// Регистрируем BackgroundService
builder.Services.AddHostedService<HRManagerBackgroundService>();

// Регистрируем контроллеры
// Регистрируем контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Сериализуем Enum как строки
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Собираем приложение
var app = builder.Build();

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

// Запускаем приложение
await app.RunAsync();
