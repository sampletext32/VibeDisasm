using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using VibeDisasm.Web.Analysis;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.DatabaseEntries;

namespace VibeDisasm.Web.Tests;

public class UnitTest1
{
    // [Fact]
    public async Task Test1()
    {
        var analyser = new PeAnalyser(new NullLogger<PeAnalyser>());

        var bytes = await File.ReadAllBytesAsync("C:\\Program Files (x86)\\Nikita\\Iron Strategy\\AniMesh.dll");
        var program = new RuntimeUserProgram(Guid.NewGuid(), "file-path", "file-name", bytes.Length);

        await analyser.Run(program, bytes, CancellationToken.None);

        var entry = program.Database.EntryManager.GetEntryAt(8);

        entry.Should().
            NotBeNull().
            And.BeOfType<StructUserProgramDatabaseEntry>().
            Which.Type.Name.Should().
            Be("IMAGE_DOS_HEADER");
    }
}
