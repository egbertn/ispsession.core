

using NCV.ISPSession;

public class MyDataExpirationService : BackgroundService
{
    private readonly KeyExpiredEventHook _keyExpiredHook;
    private readonly ILogger<MyDataExpirationService> _logger;

    public MyDataExpirationService
    (
        KeyExpiredEventHook keyExpiredEventHook,
        ILogger<MyDataExpirationService> logger
    )
    {
        _keyExpiredHook = keyExpiredEventHook;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _keyExpiredHook.KeyExpiredAsync += (key, appState) =>
        {
            _logger.LogInformation("The following application key just has expired: {key}", key);
            // set some random integer to prove it worked
            appState.Set(key, Random.Shared.Next());
            return Task.CompletedTask;
        };
        await Task.Delay(-1, stoppingToken);
    }
}