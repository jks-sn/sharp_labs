using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию из переменных окружения и файла appsettings.json
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настраиваем Kestrel на порт из конфигурации или по умолчанию 8083
var port = builder.Configuration.GetValue<int>("AppPort", 8083);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// Подключение к Postgres через EF Core
var connectionString = builder.Configuration.GetConnectionString("HRDirectorConnection");
builder.Services.AddDbContext<HRDirectorDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
});

// Регистрация сервисов, репозиториев
builder.Services.AddTransient<IHackathonRepository, HackathonRepository>();
builder.Services.AddTransient<HackathonService>();

// Контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Создание приложения
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HRDirectorDbContext>();
    db.Database.Migrate();
}

// Настраиваем маршрутизацию и эндпоинты
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Запускаем приложение
await app.RunAsync();