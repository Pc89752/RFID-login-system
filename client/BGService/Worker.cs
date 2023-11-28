namespace App.WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{
    private readonly BGService _BGService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    // private bool hasResult = false;

    public WindowsBackgroundService(
        BGService bGService,
        ILogger<WindowsBackgroundService> logger) =>
        (_BGService, _logger) = (bGService, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogWarning("Start listening usageRecordID");
            var task =  _BGService.reciveReordID();
            _logger.LogWarning("Pipe opened");
            await task;
            _logger.LogWarning($"usageRecordID: {_BGService.UsageRecordID}");
            // }
        }
        // catch (TaskCanceledException)
        // {
        //     // When the stopping token is canceled, for example, a call made from services.msc,
        //     // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        // }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        if(_BGService.UsageRecordID == -1)
        {
            _logger.LogWarning("usageRecord is -1, no return");
            return;
        };
        await _BGService.returnClosing();
        _logger.LogWarning("Returned usageRecord");
    }
}
