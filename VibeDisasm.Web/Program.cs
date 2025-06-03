using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Web;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.MyConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCorsConfiguration(builder.Configuration);


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

app.MapPost(
    "/create-project",
    async (CreateProjectDto request, CreateProjectHandler handler) =>
    {
        var id = await handler.Handle(request);

        return Results.Ok(id);
    }
);

app.MapGet(
    "/list-projects",
    async (ListProjectsHandler handler) =>
    {
        var projects = await handler.Handle();
        return Results.Ok(projects);
    }
);


app.MapPost(
    "/import-program",
    async (ImportProgramDto request, ImportProgramHandler handler) =>
    {
        try
        {
            var programId = await handler.Handle(request);
            return Results.Ok(programId);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
);

app.MapGet(
    "/list-programs",
    async (Guid? projectId, ListProgramsHandler handler) =>
    {
        if (projectId is null)
        {
            return Results.BadRequest("projectId is required.");
        }

        try
        {
            var programs = await handler.Handle(projectId.Value);
            return Results.Ok(programs);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
);

app.Run();
