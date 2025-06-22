using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Web.Endpoints;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.MyConfigureSwagger();

// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(
    options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<UserRuntimeProjectRepository>();
builder.Services.AddSingleton<UserProgramDataRepository>();

// Register handlers
builder.Services.AddSingleton<CreateProjectHandler>();
builder.Services.AddSingleton<OpenRecentHandler>();
builder.Services.AddSingleton<ListRecentsHandler>();
builder.Services.AddSingleton<ImportProgramHandler>();
builder.Services.AddSingleton<ListProgramsHandler>();
builder.Services.AddSingleton<SaveProjectHandler>();
builder.Services.AddSingleton<DeleteRecentHandler>();

builder.Services.AddSingleton<ListingAtAddressHandler>();
builder.Services.AddSingleton<ListingAddEntryHandler>();
builder.Services.AddSingleton<ListArchivesHandler>();
builder.Services.AddSingleton<ListArchiveTypesHandler>();

builder.Services.AddSingleton<ProjectArchiveService>();
builder.Services.AddSingleton<TypeArchiveService>();
builder.Services.AddSingleton<RecentsService>();

var app = builder.Build();

await app.Services.GetRequiredService<RecentsService>()
    .TryLoad();

app.UseDeveloperExceptionPage();

app.UseCorsConfiguration();

app.MapSwagger();
app.UseSwaggerUI();

// Map API endpoints
app.MapProjectEndpoints();
app.MapProgramEndpoints();
app.MapListingEndpoints();
app.MapTypesEndpoints();

await app.RunAsync();
