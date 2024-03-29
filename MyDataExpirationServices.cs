// ISP Session 10.0.x (c) 2024 Nierop Computer Vision
// ISP Session is being maintained by Nierop Computer Vision and is commercial software
// Apply for an affordable licence at https://www.nieropcomputervision.com/ispsession
// Thank you for your kind support and I am sure you will love ISP Session!

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