using bm.core;
using bm.core.data;
using bm.services.funda;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var fundaSvcKey = Environment.GetEnvironmentVariable("FUNDA_PARTNER_KEY");
if (string.IsNullOrEmpty(fundaSvcKey))
{
    Console.WriteLine("Missing FUNDA_PARTNER_KEY. Exiting..");
    return;
}

builder.Services.AddHttpClient();

var fundaSvcConfig = new Config { Key = fundaSvcKey };
var fundaSvcFactory = new Factory(fundaSvcConfig);

builder.Services.AddSingleton<IListingsProvider>(fundaSvcFactory.Create());

builder.Services.AddLogging();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IStatisticsCache, StatisticsCache>();
builder.Services.AddSingleton<IStatisticsCalculator, StatisticsCalculator>();

builder.Services.AddSingleton<IStatisticsProvider, Provider>();
builder.Services.AddSingleton<IBusyMakelaars, MakelaarsService>();

var app = builder.Build();

app.UseExceptionHandler(a => a.Run(async context =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature.Error;

    await context.Response.WriteAsJsonAsync(new { error = exception.Message });
}));

app.MapGet("/", ([FromServices] IBusyMakelaars svc) =>
{
    var filter = new Filter
    {
        City = "Amsterdam",
    };
    return svc.FetchBusiestMakelaars(filter, 10);
});

app.MapGet("/tuin", ([FromServices] IBusyMakelaars svc) =>
{
    var filter = new Filter
    {
        City = "Amsterdam",
        FeatureFilter = FeatureFilter.Garden
    };
    return svc.FetchBusiestMakelaars(filter, 10);
});


app.Run();
