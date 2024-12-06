//HRManagerService/Program.cs

using Entities;
using HRManagerService.Clients;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using HRManagerService.Options;
using HRManagerService.Repositories;
using HRManagerService.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию из переменных окружения и файла appsettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настраиваем Kestrel на порт из конфигурации или по умолчанию 8082
var port = builder.Configuration.GetValue<int>("AppPort", 8082);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// Настройка опций
builder.Services.Configure<ControllerOptions>(builder.Configuration.GetSection("ControllerOptions"));
builder.Services.Configure<RetryOptions>(builder.Configuration.GetSection("RetryOptions"));

// Добавляем миграции от EF (при необходимости)
// dotnet ef migrations add InitialCreate -p HRManagerService -s HRManagerService
// dotnet ef database update -p HRManagerService -s HRManagerService

// Регистрация Refit-клиента для HRDirectorService
var hrDirectorUri = builder.Configuration["HrDirectorUri"] ?? "http://hr_director:8083/";
builder.Services.AddRefitClient<IHRDirectorApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(hrDirectorUri));

// Регистрируем стратегии, сервисы
builder.Services.AddSingleton<ITeamBuildingStrategy, GaleShapleyStrategy>();

builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IHackathonRepository, HackathonRepository>();

builder.Services.AddScoped<HRManagerService.Services.HRManagerService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IHRDirectorClient, HRDirectorClientService>();
builder.Services.AddHostedService<HRManagerBackgroundService>();


// Регистрируем контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Собираем приложение
var app = builder.Build();

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

// Запускаем приложение
await app.RunAsync();
