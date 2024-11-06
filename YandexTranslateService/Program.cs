using Microsoft.AspNetCore.Server.Kestrel.Core;
using RestSharp;
using YandexTranslateService.Cache;
using YandexTranslateService.Services;
using Microsoft.Extensions.Configuration;
using YandexTranslateService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<ICache, InMemoryCache>();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5106, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

var apiKey = configuration["YandexTranslate:ApiKey"];
var baseUrl = configuration["YandexTranslate:BaseUrl"];

builder.Services.AddSingleton<RestClient>(sp => new RestClient(baseUrl));

builder.Services.AddSingleton<TranslatorService>(sp =>
{
    var cache = sp.GetRequiredService<ICache>();
    var client = sp.GetRequiredService<RestClient>();
    return new TranslatorService(apiKey, baseUrl, cache, client);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TranslatorService>();
app.MapGrpcReflectionService();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();