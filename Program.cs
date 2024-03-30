// ISP Session 10.0.x (c) 2024 Nierop Computer Vision
// ISP Session is being maintained by Nierop Computer Vision and is commercial software
// Apply for an affordable licence at https://www.nieropcomputervision.com/ispsession
// Thank you for your kind support and I am sure you will love ISP Session!

using Microsoft.AspNetCore.Mvc;
using NCV.ISPSession;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(i => i.SingleLine = true);
services.AddISPSessionService(builder.Configuration, options =>
{
    //note these options already have defaults for easy start
    // for demo purpose we show how to use it.
    options.ApplicationName = "Demo";
    options.CompressData = true;
    options.AffinityMethod = AffinityMethods.Cookie;
    options.CorrellationCookieName = "sessioncorrelation";
    options.SessionCookieName = "ispsession";
    options.UseRedisDataProtection = true;
});
services.AddHostedService<MyDataExpirationService>();
var app = builder.Build();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// some simple API examples using SessionState
app.MapGet("/counter", ([FromServices] SessionState sessionState) =>
{
    var counter = sessionState.Get<int>("Counter");
    counter++;
    sessionState.Set("Counter", counter);
    return new
    {
        SessionCounter = counter,
        IsExpiredSession = sessionState.IsExpired,
        IsNewSession    = sessionState.IsNew,
        SessionId = sessionState.SessionId
    };
});
app.MapGet("/apponly", ([FromServices] ApplicationState appState) =>
{
    string str = new("asdf");
    var appCounter = appState.Get<int>("Counter");
    appCounter++;
    appState.Set("Counter", appCounter);
    return new
    {
        AppCounter = appCounter
    };
});
app.UseISPSession(UseMode.Both);

app.MapGet("/abandon", (HttpContext httpContext, [FromServices]SessionState sessionState) =>
{
    sessionState.Abandon(httpContext);
});

app.MapGet("/appkeyexpire", ([FromServices]ApplicationState appState) =>
{
    appState.ExpireKeyAt("Counter", TimeSpan.FromSeconds(1));
});

app.MapGet("/counterWithApp", ([FromServices] SessionState sessionState, [FromServices]ApplicationState appState) =>
{
    var counter = sessionState.Get<int>("Counter");
    counter++;
    sessionState.Set("Counter", counter);
    var appCounter = appState.Get<int>("Counter");
    appCounter++;
    appState.Set("Counter", appCounter);

    return new
    {
        SessionCounter = counter,
        IsNewSession = sessionState.IsNew,
        IsExpiredSession = sessionState.IsExpired,
        SessionId = sessionState.SessionId,
        AppCounter = appCounter,

    };
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
