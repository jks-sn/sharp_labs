using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories;

namespace HRDirectorService;

public class HRDirectorBackgroundService : BackgroundService
{
    private readonly ILogger<HRDirectorBackgroundService> _logger;
    private readonly HRDirectorService _hrDirectorService;
    private readonly AppDbContext _dbContext;

    public HRDirectorBackgroundService(
        ILogger<HRDirectorBackgroundService> logger,
        HRDirectorService hrDirectorService,
        AppDbContext dbContext)
    {
        _logger = logger;
        _hrDirectorService = hrDirectorService;
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HRDirector is waiting for teams data...");

        while (!_hrDirectorService.DataReceived)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("HRDirector has received teams data.");

        var meanSatisfactionIndex = _hrDirectorService.CalculateMeanSatisfactionIndex();

        _logger.LogInformation("Harmony index is: {MeanSatisfactionIndex}", meanSatisfactionIndex);

        // Save results to database
        await _hrDirectorService.SaveResultsAsync(_dbContext);

        _logger.LogInformation("Results have been saved to the database.");
    }
}