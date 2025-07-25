using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Core;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Analysis;
using VibeDisasm.Web.BackgroundServices;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;
using VibeDisasm.Web.Endpoints;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

using var loggerFactory = LoggerFactory.Create(x => x.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCorsConfiguration(builder.Configuration, logger);
builder.Services.AddSwaggerGen();
builder.Services.MyConfigureSwagger();

// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<UserRuntimeProjectRepository>();
builder.Services.AddSingleton<UserProgramDataRepository>();
builder.Services.AddSingleton<BackgroundJobRepository>();

var channel = Channel.CreateBounded<BinaryAnalysisJob>(
    new BoundedChannelOptions(1)
    {
        Capacity = 1,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.Wait // handles only 1 concurrent job and waits for other writes
    }
);

builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<BinaryAnalysisBackgroundService>();

builder.Services.AddSingleton<PeAnalyser>();
builder.Services.AddSingleton<AnalyserResolver>();
builder.Services.AddSingleton<AnalysisExecutor>();

// Register handlers
foreach (var handlerType in Utils.GetAssignableTypes<IHandler>())
{
    logger.LogInformation("Registered handler: {Handler}", handlerType.Name);
    builder.Services.AddScoped(handlerType);
}

builder.Services.AddSingleton<ProjectArchiveService>();
builder.Services.AddSingleton<TypeArchiveService>();
builder.Services.AddSingleton<RecentsService>();

var app = builder.Build();

await app.Services.GetRequiredService<RecentsService>().TryLoad();

app.UseDeveloperExceptionPage();

app.UseCorsConfiguration();

app.MapSwagger();
app.UseSwaggerUI();

// Map API endpoints
app.MapProjectEndpoints();
app.MapProgramEndpoints();
app.MapListingEndpoints();
app.MapTypesEndpoints();
app.MapGeneralEndpoints();

await app.RunAsync();
