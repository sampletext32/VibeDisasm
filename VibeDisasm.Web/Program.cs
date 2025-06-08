using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Web.Endpoints;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
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
builder.Services.AddSingleton<UserProgramRepository>();
builder.Services.AddSingleton<UserProgramDataRepository>();

// Register state services
builder.Services.AddSingleton<AppState>();

// Register handlers
builder.Services.AddSingleton<CreateProjectHandler>();
builder.Services.AddSingleton<OpenProjectHandler>();
builder.Services.AddSingleton<ListProjectsHandler>();
builder.Services.AddSingleton<ImportProgramHandler>();
builder.Services.AddSingleton<ListProgramsHandler>();
builder.Services.AddSingleton<GetProgramPeDataHandler>();
builder.Services.AddSingleton<SaveProjectHandler>();

builder.Services.AddSingleton<ProjectArchiveService>();


var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseCorsConfiguration();

app.MapSwagger();
app.UseSwaggerUI();

// Map API endpoints
app.MapProjectEndpoints();
app.MapProgramEndpoints();
app.MapProgramPeDataEndpoints();

app.Run();
