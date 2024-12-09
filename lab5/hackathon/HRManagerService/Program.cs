//HRManagerService/Program.cs

using Entities;
using HRManagerService.Clients;
using HRManagerService.Data;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using HRManagerService.Options;
using HRManagerService.Repositories;
using HRManagerService.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

// Подключение к Postgres через EF Core
var connectionString = builder.Configuration.GetConnectionString("HRManagerConnection");
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
builder.Services.AddDbContext<HRManagerDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
}, ServiceLifetime.Transient);

// Регистрация Refit-клиента для HRDirectorService
var hrDirectorUri = builder.Configuration["HrDirectorUri"] ?? "http://hr_director:8083/";
builder.Services.AddRefitClient<IHRDirectorApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(hrDirectorUri));

// Регистрируем стратегии, сервисы
builder.Services.AddTransient<ITeamBuildingStrategy, GaleShapleyStrategy>();

builder.Services.AddTransient<IParticipantRepository, ParticipantRepository>();
builder.Services.AddTransient<IWishlistRepository, WishlistRepository>();
builder.Services.AddTransient<ITeamRepository, TeamRepository>();
builder.Services.AddTransient<IHackathonRepository, HackathonRepository>();

builder.Services.AddTransient<HRManagerService.Services.HRManagerService>();
builder.Services.AddTransient<IParticipantService, ParticipantService>();
builder.Services.AddTransient<IHRDirectorClient, HRDirectorClientService>();

builder.Services.AddTransient<HRManagerBackgroundService>();


// Регистрируем контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Собираем приложение
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HRManagerDbContext>();
    db.Database.Migrate();
}

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Запускаем приложение
await app.RunAsync();
