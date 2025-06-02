using System.Diagnostics;

namespace VibeDisasm.DecompilerEngine.IREverything.Cfg;

[DebuggerDisplay("{DebugDisplay}")]
public class IRCfgEdge
{
    public uint From { get; set; }
    public uint To { get; set; }

    public IREdge Type { get; set; }

    public IRCfgEdge(uint from, uint to, IREdge type)
    {
        From = from;
        To = to;
        Type = type;
    }

    internal string DebugDisplay => $"IRCfgEdge(From: {From:X8}, To: {To:X8}, Type: {Type:G})";
}

public enum IREdge
{
    Taken,
    Fallthrough
}
