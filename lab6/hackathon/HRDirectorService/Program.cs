//HRDirectorService/Program.cs

using HRDirectorService;
using HRDirectorService.Consumers;
using HRDirectorService.Data;
using HRDirectorService.Interfaces;
using HRDirectorService.Options;
using HRDirectorService.Repositories;
using HRDirectorService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ParticipantWithWishlistConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
    });
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("hrdirector_participants", e =>
        {
            e.ConfigureConsumer<ParticipantWithWishlistConsumer>(ctx);
        });
    });
});

var connectionString = builder.Configuration.GetConnectionString("HRDirectorConnection");
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
builder.Services.AddDbContext<HRDirectorDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableDetailedErrors();
}, ServiceLifetime.Scoped);

builder.Services.Configure<HackathonOptions>(builder.Configuration.GetSection("HackathonOptions"));

builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IHackathonRepository, HackathonRepository>();

builder.Services.AddScoped<HRDirectorOrchestrationService>();

builder.Services.AddHostedService<HRDirectorBackgroundService>();

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

await app.RunAsync();