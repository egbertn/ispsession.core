// ISP Session 10.0.x (c) 2024 Nierop Computer Vision
// ISP Session is being maintained by Nierop Computer Vision and is commercial software
// Apply for an affordable license at https://www.nieropcomputervision.com/ispsession
// Thank you for your kind support and I am sure you will love ISP Session!

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NCV.ISPSession;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(i => i.SingleLine = true);

// these are the only runtime options you would need to customize
// the other ones go into appsettings
services.AddISPSessionService(options =>
{
    //note these options already have defaults for easy start
    // for demo purpose we show how to use it.
    //options.ApplicationName = "Demo";
    options.CompressData = true;
    options.AffinityMethod = AffinityMethods.Cookie;
    //options.DataTimeOut = TimeSpan.FromSeconds(10);
    //options.CorrellationCookieName = "sessioncorrelation";
    //options.SessionCookieName = "ispsession";
    // use both Application State and Session State
    options.Mode = UseMode.Both;
    options.SameSite = SameSiteMode.Lax;
});

// if you want application level variable expiration optionally
// add your own hosted service
services.AddHostedService<MyDataExpirationService>();
var app = builder.Build();

// put SessionState and/or ApplicationState in the request pipeline
app.UseISPSession();

app.MapGet("/complexdata", ( [FromServices]IApplicationState appState) =>
{
    // this data is set in the MyDataExpirationService, just to demonstrate persistance
    var persistComplexData = appState.Get<ServiceDto>("complex_data");
    return persistComplexData;

});

// some simple API examples using SessionState
// ISP Session is supported for Razor Pages, for ApiController classes
app.MapGet("/counter", ([FromServices] ISessionState sessionState) =>
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


//Demonstraties how to get the session in cases
// where you need e.g. deep inspection of the request body
//  you must set this AffinityMethod first
//    options.AffinityMethod = AffinityMethods.CustomInit;
app.MapPost("/countercustom", async (HttpContext context) =>
{

    async Task <string?> RetrieveSessionIdFromBody()
    {
        var request = context.Request;
        var response = context.Response;
        if (request.ContentType != "application/json")
        {
            throw new InvalidOperationException("expected JSON");
        }
        var myJson =await JsonSerializer.DeserializeAsync<DeepInspectBody>(request.Body);
        return myJson.PhoneNumber;
    }

    await using var sessionState = await context.GetCustomSessionId(await RetrieveSessionIdFromBody());

    // from here the usual code
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



app.MapGet("/apponly", ([FromServices] IApplicationState appState) =>
{
    var appCounter = appState.Get<int>("Counter");
    appCounter++;
    appState.Set("Counter", appCounter);
    return new
    {
        AppCounter = appCounter
    };
});

app.MapGet("/abandon", (HttpContext httpContext, [FromServices]ISessionState sessionState) =>
{
    //abandon cleans up immediately data  which normally expires
    sessionState.Abandon(httpContext);
});

app.MapGet("/appkeyexpire", ([FromServices]IApplicationState appState) =>
{
    //expire the application key in one second
    // MyDataExpirationService should pretty much immediately
    //notify you of this event see your debugging console
    appState.ExpireKeyAt("Counter", TimeSpan.FromSeconds(1));
});

app.MapGet("/counterWithApp", ([FromServices] ISessionState sessionState, [FromServices]IApplicationState appState) =>
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

