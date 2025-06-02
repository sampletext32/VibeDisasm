using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using VibeDisasm.Pe;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;
using VibeDisasm.Web.Extensions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.MyConfigureSwagger();
builder.Services.AddEndpointsApiExplorer();


// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<AppState>();
var app = builder.Build();


app.MapSwagger();
app.UseSwaggerUI();

app.MapPost(
    "/create-project",
    (AppState state) =>
    {
        var project = new UserProject() {Id = Guid.NewGuid()};

        state.Projects.Add(project);

        return Results.Ok(project.Id);
    }
);

app.MapGet(
    "/list-projects",
    (AppState state) =>
    {
        return Results.Ok(state.Projects.Select(x => x.Id));
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

app.MapGet(
    "/pe-info",
    (AppState state, Guid? programId) =>
    {
        if (programId is null)
        {
            return Results.BadRequest("programId is required.");
        }

        var program = state.Projects
            .SelectMany(p => p.Programs)
            .FirstOrDefault(p => p.Id == programId);

        if (program is null)
        {
            return Results.NotFound($"Program with ID {programId} not found.");
        }

        try
        {
            var rawPeFile = new RawPeParser().Parse(program.FileData);
            var peInfo = PeInfoExtractor.Extract(rawPeFile);
            var sections = SectionExtractor.Extract(rawPeFile);
            var exports = ExportExtractor.Extract(rawPeFile);
            var imports = ImportExtractor.Extract(rawPeFile);
            var entryPoint = EntryPointExtractor.Extract(rawPeFile, exports);
            var resources = ResourceExtractor.Extract(rawPeFile);

            return Results.Ok(new {
                BasicInfo = peInfo,
                Sections = sections,
                Exports = exports,
                Imports = imports,
                EntryPoint = entryPoint,
                Resources = resources
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to extract PE information: {ex}");
        }
    }
);

app.Run();

public class AppState
{
    public List<UserProject> Projects { get; set; } = [];
}

public class UserProject
{
    public Guid Id { get; set; }
    public List<UserProgram> Programs { get; set; } = [];
}

public class UserProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public byte[] FileData { get; set; }

    public UserProgram(Guid id, string filePath, byte[] fileData)
    {
        Id = id;
        FilePath = filePath;
        FileData = fileData;
        Name = Path.GetFileName(filePath);
    }

    public List<UserProgramFunction> Functions { get; set; } = [];
}

public class UserProgramFunction
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public UserProgramFunction(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
