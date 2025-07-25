namespace VibeDisasm.Web.Models;

/// <summary>
/// Kind of program, that user can analyze
/// </summary>
public enum ProgramKind
{
    Undefined,

    /// <summary>Portable Executable (Windows)</summary>
    PE,

    /// <summary>Executable and Linkable Format (Linux, Android, embedded)</summary>
    ELF,

    /// <summary>macOS/iOS</summary>
    MachO,

    /// <summary>Android Package (usually contains ELF)</summary>
    APK,

    /// <summary>WebAssembly binary</summary>
    WASM,

    /// <summary>Raw memory dump or firmware</summary>
    RawBinary,

    // <summary>Dalvik Executable (inside APK)</summary>
    DEX,

    /// <summary>Plaintext script files (e.g. Lua, Python, etc.)</summary>
    Script
}
