using HRDirectorService.Data;
using HRDirectorService.Interfaces;
using HRDirectorService.Repositories;
using HRDirectorService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Настраиваем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var port = builder.Configuration.GetValue<int>("AppPort", 8083);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

var connectionString = builder.Configuration.GetConnectionString("HRDirectorConnection");
builder.Services.AddDbContext<HRDirectorDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
});

builder.Services.AddTransient<IHackathonRepository, HackathonRepository>();
builder.Services.AddTransient<HackathonService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HRDirectorDbContext>();
    db.Database.Migrate();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Запускаем приложение
await app.RunAsync();