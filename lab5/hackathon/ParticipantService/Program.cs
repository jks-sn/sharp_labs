using ParticipantService;
using Entities;
using Entities.Consts;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Services;

//заводимся
var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию из переменных окружения и файла appsettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);

//Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(8081);
});

builder.Services.Configure<DataLoaderOptions>(builder.Configuration.GetSection("DataLoaderOptions"));

builder.Services.Configure<ServiceSettings>(options =>
{
    var id = int.Parse(builder.Configuration["ID"] ?? "1");
    if (!Enum.TryParse<ParticipantTitle>(builder.Configuration["TITLE"] ?? "Junior", true, out var title))
    {
        throw new ArgumentException("Invalid TITLE in configuration");
    }
    
    var name = builder.Configuration["NAME"] ?? "Armando";
    var participant = new Participant(id, title, name);
    options.Participant = participant;

    // Загрузка ProbableTeammates
    var dataLoader = new DataLoader(
        Options.Create(builder.Configuration.GetSection("DataLoaderOptions").Get<DataLoaderOptions>()));
    options.ProbableTeammates = dataLoader.LoadProbableTeammates(title);
});

builder.Services.AddSingleton<DataLoader>();

builder.Services.AddSingleton<ParticipantService.ParticipantService>();

// Настраиваем HttpClient для взаимодействия с HRManagerService
var hrManagerUriString = builder.Configuration["HrManagerUri"] ?? "http://hr_manager";
if (!Uri.TryCreate(hrManagerUriString, UriKind.Absolute, out var hrManagerUri))
{
    throw new Exception("HrManagerUri is not a valid URI");
}
builder.Services.AddHttpClient<ParticipantBackgroundService>(client =>
{
    client.BaseAddress = hrManagerUri;
});

builder.Services.AddSingleton<ParticipantBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<ParticipantBackgroundService>());

builder.Services.AddControllers();

var app = builder.Build();

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();