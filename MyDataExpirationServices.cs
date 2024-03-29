using NCV.ISPSession;

public class MyDataExpirationService(
    KeyExpiredEventHook keyExpiredEventHook,
    ILogger<MyDataExpirationService> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        keyExpiredEventHook.KeyExpiredAsync += (key, appState) =>
        {
            logger.LogInformation("The following application key just has expired: {key}", key);
            // set some random integer to prove it worked
            appState.Set(key, Random.Shared.Next());
            return Task.CompletedTask;
        };
        await Task.Delay(-1, stoppingToken);
    }
}