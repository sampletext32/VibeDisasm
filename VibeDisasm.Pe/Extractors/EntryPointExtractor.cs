using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

public static class EntryPointExtractor
{
    public static List<EntryPointInfo> Extract(RawPeFile rawPeFile, ExportInfo? exportInfo)
    {
        // Create a list to store all definite code offsets
        var entryPoints = new List<EntryPointInfo>();

        // 1. Add entry point as definite code
        if (rawPeFile.OptionalHeader.AddressOfEntryPoint > 0)
        {
            var entryPointOffset = Util.RvaToOffset(rawPeFile, rawPeFile.OptionalHeader.AddressOfEntryPoint);
            entryPoints.Add(new EntryPointInfo(
                entryPointOffset,
                rawPeFile.OptionalHeader.AddressOfEntryPoint,
                "Entry Point",
                "Program entry point"));
        }

        // 2. Add exported functions as definite code
        if (exportInfo != null && exportInfo.Functions.Count > 0)
        {
            foreach (var exportedFunction in exportInfo.Functions)
            {
                // Skip forwarded exports (they don't have code in this file)
                if (exportedFunction.IsForwarded)
                {
                    continue;
                }

                var exportOffset = Util.RvaToOffset(rawPeFile, exportedFunction.RelativeVirtualAddress);
                entryPoints.Add(new EntryPointInfo(
                    exportOffset,
                    exportedFunction.RelativeVirtualAddress,
                    "Export",
                    $"Exported function: {exportedFunction.Name} (Ordinal: {exportedFunction.Ordinal})"));
            }
        }

        return entryPoints;
    }
}
