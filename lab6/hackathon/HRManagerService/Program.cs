//HRManagerService/Program.cs

using Entities;
using HRManagerService.Clients;
using HRManagerService.Consumers;
using HRManagerService.Data;
using HRManagerService.Interface;
using HRManagerService.Interfaces;
using HRManagerService.Options;
using HRManagerService.Repositories;
using HRManagerService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var port = builder.Configuration.GetValue<int>("AppPort", 8082);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HackathonStartConsumer>();
    x.AddConsumer<ParticipantConsumer>();
    x.AddConsumer<WishlistConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("hrmanager", e =>
        {
            e.ConfigureConsumer<HackathonStartConsumer>(ctx);
            e.ConfigureConsumer<ParticipantConsumer>(ctx);
            e.ConfigureConsumer<WishlistConsumer>(ctx);
        });
    });
});

// Настройка опций
builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection("ConnectionOptions"));
builder.Services.Configure<ControllerOptions>(builder.Configuration.GetSection("ControllerOptions"));
builder.Services.Configure<RetryOptions>(builder.Configuration.GetSection("RetryOptions"));

// Подключение к Postgres через EF Core
var connectionString = builder.Configuration.GetConnectionString("HRManagerConnection");
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
builder.Services.AddDbContext<HRManagerDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
}, ServiceLifetime.Scoped);

// Регистрация Refit-клиента для HRDirectorService
var hrDirectorUri = builder.Configuration["HrDirectorUri"] ?? "http://hr_director:8083/";
builder.Services.AddRefitClient<IHRDirectorApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(hrDirectorUri));

// Регистрируем стратегии, сервисы
builder.Services.AddScoped<ITeamBuildingStrategy, GaleShapleyStrategy>();

builder.Services.AddScoped<ITeamBuildingOrchestrationService, TeamBuildingOrchestrationService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IHRDirectorClient, HRDirectorClientService>();

builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

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
