using bm.core;
using bm.core.cache;
using bm.core.statistics;
using bm.services.funda;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var fundaSvcConfig = new Config { Key = "ac1b0b1572524640a0ecc54de453ea9f" };
var fundaSvcFactory = new Factory(fundaSvcConfig);

builder.Services.AddSingleton<IListingsProvider>(fundaSvcFactory.Create());

builder.Services.AddLogging();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IStatisticsCache, StatisticsCache>();
builder.Services.AddSingleton<IStatisticsCalculator, StatisticsCalculator>();

builder.Services.AddSingleton<IStatisticsProvider, Statistics>();
builder.Services.AddSingleton<IBusyMakelaars, MakelaarsService>();

var app = builder.Build();

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
