using System.Text.Json;

namespace VibeDisasm.Web.Services;

/// <summary>
/// Service for loading and managing recently opened projects.
/// </summary>
public class RecentsService
{
    private List<RecentJsonMetadata> _recents = [];

    private readonly ILogger<RecentsService> _logger;

    public RecentsService(IHostApplicationLifetime lifetime, ILogger<RecentsService> logger)
    {
        _logger = logger;

        lifetime.ApplicationStopping.Register(OnApplicationStopping);
    }

    private void OnApplicationStopping()
    {
        _logger.LogInformation("Application is stopping, saving recents...");

        var path = Path.Combine(Environment.CurrentDirectory, "recents.json");

        FileStream fs;
        if (!File.Exists(path))
        {
            _logger.LogInformation("Recents file does not exist, creating a new one.");
            fs = File.Create(path);
        }
        else
        {
            _logger.LogInformation("Recents file exists, truncating it.");
            fs = File.Open(path, FileMode.Truncate);
        }

        JsonSerializer.Serialize(fs, _recents);

        fs.Flush();
        fs.Dispose();
        _logger.LogInformation("Recents saved to {Path}", path);
    }

    public IEnumerable<RecentJsonMetadata> Get() => _recents;

    public async Task TryLoad()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "recents.json");

        if (!File.Exists(path))
        {
            _logger.LogInformation("Recents file does not exist, skipped loading recents.");
            return;
        }

        var text = await File.ReadAllTextAsync(path);

        var recents = JsonSerializer.Deserialize<List<RecentJsonMetadata>>(text);

        _recents = recents ?? [];

        _logger.LogInformation("Recents loaded: {Count} items", _recents.Count);
    }

    public void Track(string path)
    {
        var existingRecent = _recents.FirstOrDefault(x => x.Path == path);
        if (existingRecent is not null)
        {
            _recents.Remove(existingRecent);
            existingRecent = existingRecent with {LastOpened = DateTime.UtcNow};
        }
        else
        {
            existingRecent = new(Guid.NewGuid(), path, DateTime.UtcNow);
        }

        _recents.Add(existingRecent);
    }

    public void RemoveByProjectId(Guid projectId)
    {
        var existingRecent = _recents.FirstOrDefault(x => x.ProjectId == projectId);
        if (existingRecent is not null)
        {
            _logger.LogInformation("Removing recent project: {ProjectId} - {Path}", existingRecent.ProjectId, existingRecent.Path);
            _recents.Remove(existingRecent);
        }
        else
        {
            _logger.LogWarning("Attempted to remove recent project with ID {ProjectId}, but it was not found.", projectId);
        }
    }
}
