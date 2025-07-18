using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Analysis;

/// <summary>
/// Resolves an analyser from DI capable of analysing specified program kind
/// </summary>
public class AnalyserResolver
{
    private readonly IServiceProvider _serviceProvider;

    public AnalyserResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAnalyser? Resolve(ProgramKind programKind)
    {
        return programKind switch
        {
            ProgramKind.PE => _serviceProvider.GetRequiredService<PeAnalyser>(),
            _ => null
        };
    }
}
