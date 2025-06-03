using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Web;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Endpoints;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models;
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

builder.Services.AddSingleton<AppState>();
builder.Services.AddSingleton<CreateProjectHandler>();
builder.Services.AddSingleton<ListProjectsHandler>();
builder.Services.AddSingleton<ImportProgramHandler>();
builder.Services.AddSingleton<ListProgramsHandler>();


var app = builder.Build();

app.UseCorsConfiguration();

app.MapSwagger();
app.UseSwaggerUI();

// Map API endpoints
app.MapProjectEndpoints();
app.MapProgramEndpoints();

app.Run();
