using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts all definite code offsets from a PE file
/// </summary>
public static class CodeOffsetsExtractor
{
    /// <summary>
    /// Extracts all definite code offsets from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Information about all definite code offsets</returns>
    public static CodeOffsetsInfo Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile is null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        var codeOffsetsInfo = new CodeOffsetsInfo();

        // 1. Add entry point as definite code
        ExtractEntryPoint(rawPeFile, codeOffsetsInfo);

        // 2. Add exported functions as definite code
        ExtractExportedFunctions(rawPeFile, codeOffsetsInfo);

        // 3. Add TLS callbacks as definite code
        ExtractTlsCallbacks(rawPeFile, codeOffsetsInfo);

        // 4. Add exception handlers as definite code
        ExtractExceptionHandlers(rawPeFile, codeOffsetsInfo);

        // 5. Add delay import thunks as definite code
        ExtractDelayImports(rawPeFile, codeOffsetsInfo);

        return codeOffsetsInfo;
    }

    private static void ExtractEntryPoint(RawPeFile rawPeFile, CodeOffsetsInfo codeOffsetsInfo)
    {
        // Extract basic PE information to get the entry point
        var peInfo = PeInfoExtractor.Extract(rawPeFile);

        if (peInfo.EntryPointRva > 0)
        {
            var entryPointOffset = Util.RvaToOffset(rawPeFile, peInfo.EntryPointRva);
            codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
            {
                FileOffset = entryPointOffset,
                RelativeVirtualAddress = peInfo.EntryPointRva,
                Source = "Entry Point",
                Description = "Program entry point"
            });
        }
    }

    private static void ExtractExportedFunctions(RawPeFile rawPeFile, CodeOffsetsInfo codeOffsetsInfo)
    {
        // Extract export information
        var exportInfo = ExportExtractor.Extract(rawPeFile);

        if (exportInfo is null || exportInfo.Functions.Count == 0)
        {
            return;
        }

        foreach (var exportedFunction in exportInfo.Functions)
        {
            // Skip forwarded exports (they don't have code in this file)
            if (exportedFunction.IsForwarded)
            {
                continue;
            }

            var exportOffset = Util.RvaToOffset(rawPeFile, exportedFunction.RelativeVirtualAddress);
            codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
            {
                FileOffset = exportOffset,
                RelativeVirtualAddress = exportedFunction.RelativeVirtualAddress,
                Source = "Export",
                Description = $"Exported function: {exportedFunction.Name} (Ordinal: {exportedFunction.Ordinal})"
            });
        }
    }

    private static void ExtractTlsCallbacks(RawPeFile rawPeFile, CodeOffsetsInfo codeOffsetsInfo)
    {
        // Extract TLS information
        var tlsInfo = TlsExtractor.Extract(rawPeFile);

        if (tlsInfo is null || tlsInfo.Callbacks.Count == 0)
        {
            return;
        }

        foreach (var callback in tlsInfo.Callbacks)
        {
            codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
            {
                FileOffset = callback.FileOffset,
                RelativeVirtualAddress = callback.RelativeVirtualAddress,
                Source = "TLS Callback",
                Description = $"TLS Callback #{callback.Index}"
            });
        }
    }

    private static void ExtractExceptionHandlers(RawPeFile rawPeFile, CodeOffsetsInfo codeOffsetsInfo)
    {
        // Extract exception handling information
        var exceptionInfo = ExceptionExtractor.Extract(rawPeFile);

        if (exceptionInfo is null || exceptionInfo.Functions.Count == 0)
        {
            return;
        }

        foreach (var function in exceptionInfo.Functions)
        {
            // Add the function start as definite code
            codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
            {
                FileOffset = function.BeginAddressFileOffset,
                RelativeVirtualAddress = function.BeginAddress,
                Source = "Exception Handler",
                Description = $"Function start for exception handler #{function.Index}"
            });

            // Add the unwind info as definite code (if it's not 0)
            if (function.UnwindInfoAddress > 0)
            {
                codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
                {
                    FileOffset = function.UnwindInfoFileOffset,
                    RelativeVirtualAddress = function.UnwindInfoAddress,
                    Source = "Exception Handler",
                    Description = $"Unwind info for exception handler #{function.Index}"
                });
            }
        }
    }

    private static void ExtractDelayImports(RawPeFile rawPeFile, CodeOffsetsInfo codeOffsetsInfo)
    {
        // Extract delay import information
        var delayImportInfo = DelayImportExtractor.Extract(rawPeFile);

        if (delayImportInfo is null || delayImportInfo.Modules.Count == 0)
        {
            return;
        }

        foreach (var module in delayImportInfo.Modules)
        {
            if (module.DelayImportAddressTableRva > 0)
            {
                var delayImportAddressTableOffset = Util.RvaToOffset(rawPeFile, module.DelayImportAddressTableRva);
                codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
                {
                    FileOffset = delayImportAddressTableOffset,
                    RelativeVirtualAddress = module.DelayImportAddressTableRva,
                    Source = "Delay Import",
                    Description = $"Delay import address table for {module.Name}"
                });
            }

            // Add individual functions from the module
            foreach (var function in module.Functions)
            {
                codeOffsetsInfo.Offsets.Add(new CodeOffsetInfo
                {
                    FileOffset = function.ImportAddressTableOffset,
                    RelativeVirtualAddress = function.ImportAddressTableRva,
                    Source = "Delay Import Function",
                    Description = $"Delay import function: {(function.ImportByOrdinal ? $"Ordinal: {function.Ordinal}" : function.Name)}"
                });
            }
        }
    }
}
