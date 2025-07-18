using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;

namespace VibeDisasm.Web.Repositories;

/// <summary>
/// Repository for background jobs
/// </summary>
public class BackgroundJobRepository
{
    private readonly Dictionary<Type, List<BackgroundJob>> _jobs = [];

    public Task Add<T>(T job)
        where T : BackgroundJob
    {
        ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(_jobs, typeof(T), out var exists);
        if (!exists || list is null)
        {
            list = [];
        }

        list.Add(job);
        return Task.CompletedTask;
    }

    public Task<T?> Get<T>(Guid jobId)
        where T : BackgroundJob
    {
        ref var list = ref CollectionsMarshal.GetValueRefOrNullRef(_jobs, typeof(T));
        return Task.FromResult(Unsafe.IsNullRef(ref list) ? null : list.Find(x => x.JobId == jobId) as T);
    }
}
