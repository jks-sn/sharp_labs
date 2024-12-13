// ParticipantService/Program.cs

using MassTransit;
using Microsoft.Extensions.Options;
using ParticipantService;
using ParticipantService.Clients;
using ParticipantService.Entities;
using ParticipantService.Entities.Consts;
using ParticipantService.Options;
using ParticipantService.Services;
using Refit;

//заводимся
var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию из переменных окружения и файла appsettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);

//Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Получаем HrManagerUri и AppPort из конфигурации
var hrManagerUri = builder.Configuration["HrManagerUri"] ?? "http://hr_manager:8082/";
var appPort = int.Parse(builder.Configuration["APP_PORT"] ?? "8081");

// Настраиваем Kestrel для прослушивания на указанном порту
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(appPort);
});


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ParticipantBackgroundService>();
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("participant-service", e =>
        {
            e.ConfigureConsumer<ParticipantBackgroundService>(ctx);
        });
    });
});


// Регистрация опций
builder.Services.Configure<DataLoaderOptions>(builder.Configuration.GetSection("DataLoaderOptions"));
builder.Services.Configure<RetryOptions>(builder.Configuration.GetSection("RetryOptions"));
builder.Services.Configure<ServiceOptions>(options =>
{
    options.Participant = new Participant
    {
        Id = int.Parse(builder.Configuration["ID"] ?? "1"),
        Name = builder.Configuration["NAME"] ?? "Participant",
        Title = Enum.TryParse<ParticipantTitle>(builder.Configuration["TITLE"], true, out var title) ? title : ParticipantTitle.Junior
    };
});

var id = int.Parse(builder.Configuration["ID"] ?? "1");
// Регистрация Refit клиента
builder.Services.AddRefitClient<IHrManagerApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(hrManagerUri));

// Регистрация сервисов
builder.Services.AddSingleton<DataLoader>();
builder.Services.AddSingleton<ParticipantService.Services.ParticipantService>();
builder.Services.AddHostedService<ParticipantBackgroundService>();

// Добавление контроллеров
builder.Services.AddControllers();

// Создание приложения
var app = builder.Build();

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Запуск приложения
await app.RunAsync();