// Tests/HRManagerTests/Services/TestHRManagerBackgroundService.cs

using System.Threading;
using System.Threading.Tasks;
using HRManagerService.Services;
using Microsoft.Extensions.Logging;
using System;

namespace HRManagerTests.Services;

public class TestHRManagerBackgroundService(
    ILogger<HRManagerBackgroundService> logger,
    IServiceProvider serviceProvider)
    : HRManagerBackgroundService(logger, serviceProvider)
{
    public Task ExecuteAsyncPublic(CancellationToken cancellationToken)
    {
        return base.ExecuteAsync(cancellationToken);
    }
}