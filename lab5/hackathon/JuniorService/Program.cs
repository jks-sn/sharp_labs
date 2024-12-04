using JuniorService;
using Entities;

//заводимся
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(8081);
});

//доостаём нашего Джуна
var id = int.Parse(builder.Configuration["ID"] ?? "1");
var title = builder.Configuration["TITLE"] ?? "Junior";
var name = builder.Configuration["NAME"] ?? "Armando";
var participant = new Participant(id, title, name);

//достаём ему wishlist
//DataLoader Logic
// services.Configure<DataLoaderOptions>(context.Configuration.GetSection("DataLoaderOptions"));
// services.AddSingleton<DataLoader>();
// var probableTeammates = _dataLoader.LoadTeamLeads();
// _serviceSettings.ProbableTeammates = probableTeammates;

builder.Services.Configure<ServiceSettings>(settings =>
{
    settings.Participant = participant;
    settings.ProbableTeammates = teamLeads;
});

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient("HrManagerClient", client =>
        {
            client.BaseAddress = new Uri(context.Configuration["HrManagerUri"] ?? "http://hr_manager:8081/");
        });
    });

builder.Services.AddHttpClient<JuniorBackgroundService(client => client.BaseAddress = new Uri(builder.Configuration["BASE_URL"]));

builder.Services.AddSingleton<JuniorService.JuniorService>();
builder.Services.AddHostedService<JuniorBackgroundService>();

var host = builder.Build();

await host.RunAsync();