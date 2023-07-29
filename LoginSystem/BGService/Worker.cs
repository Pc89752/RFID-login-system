namespace App.WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{
    private readonly BGService _BGService;
    private readonly ILogger<WindowsBackgroundService> _logger;
    private bool hasResult = false;

    public WindowsBackgroundService(
        BGService bGService,
        ILogger<WindowsBackgroundService> logger) =>
        (_BGService, _logger) = (bGService, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     _logger.LogInformation("Listening usageRecordID");
            //     await _BGService.reciveReordID();
            //     var usageRecordID = _BGService.UsageRecordID;
            //     _logger.LogInformation($"usageRecordID: {usageRecordID}");

            //     await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            // }
            // if (!hasResult)
            // {
            _logger.LogWarning("Start listening usageRecordID");
            var task =  _BGService.reciveReordID();
            _logger.LogWarning("Pipe opened");
            // while(!task.IsCompleted)
            // {
            //     await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            //     _logger.LogWarning("Waiting");
            // }
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

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _BGService.returnClosing();
        return Task.CompletedTask;
    }
}
