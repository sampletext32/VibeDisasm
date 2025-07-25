using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Analysis;

/// <summary>
/// Resolves an analyser from DI capable of analysing specified program kind
/// </summary>
public class AnalyserResolver(IServiceProvider serviceProvider)
{
    public IAnalyser? Resolve(ProgramKind programKind)
    {
        return programKind switch
        {
            ProgramKind.PE => serviceProvider.GetRequiredService<PeAnalyser>(),
            _ => null
        };
    }
}
