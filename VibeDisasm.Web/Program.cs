using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Web;
using VibeDisasm.Web.Extensions;
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
var app = builder.Build();

app.UseCorsConfiguration();

app.MapSwagger();
app.UseSwaggerUI();

app.MapPost(
    "/create-project",
    (CreateProjectDto request, AppState state) =>
    {
        var project = new UserProject() {Id = Guid.NewGuid(), Title = request.Title ?? ""};

        state.Projects.Add(project);

        return Results.Ok(project.Id);
    }
);

app.MapGet(
    "/list-projects",
    (AppState state) =>
    {
        var projectDetails = state.Projects.Select(p => new ProjectDetailsDto(p.Id, p.Title));
        return Results.Ok(projectDetails);
    }
);


app.MapPost(
    "/import-program",
    (AppState state, Guid? projectId) =>
    {
        if (projectId is null)
        {
            return Results.BadRequest("projectId is required.");
        }

        var project = state.Projects.First(x => x.Id == projectId);

        const string filePath = @"C:\Projects\CSharp\VibeDisasm\VibeDisasm.TestLand\DLLs\iron_3d.exe";

        var fileData = File.ReadAllBytes(filePath);

        var program = new UserProgram(Guid.NewGuid(), filePath, fileData);

        project.Programs.Add(program);

        return Results.Ok(program.Id);
    }
);

app.MapGet(
    "/list-programs",
    (AppState state, Guid? projectId) =>
    {
        if (projectId is null)
        {
            return Results.BadRequest("projectId is required.");
        }

        var project = state.Projects.First(x => x.Id == projectId);

        return Results.Ok(project.Programs.Select(x => new {x.Id, x.Name, x.FilePath}));
    }
);

app.Run();
