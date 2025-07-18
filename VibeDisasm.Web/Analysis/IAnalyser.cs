using FluentResults;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Analysis;

public interface IAnalyser
{
    Task<Result> Run(RuntimeUserProgram program, byte[] binaryData, CancellationToken cancellationToken);
}
