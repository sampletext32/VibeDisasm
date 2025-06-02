namespace VibeDisasm.DecompilerEngine.IREverything.Cfg;

/// <summary>
/// Convenience class for looking up edges in a control flow graph (CFG).
/// </summary>
public class IRCfgEdgesLookup
{
    /// <summary>
    /// List of all edges in the CFG.
    /// </summary>
    public List<IRCfgEdge> Edges { get; }

    /// <summary>
    /// Lookup for edges by their 'from' node ID.
    /// </summary>
    public ILookup<uint, IRCfgEdge> EdgesByFrom { get; }

    /// <summary>
    /// Lookup for edges by their 'to' node ID.
    /// </summary>
    public ILookup<uint, IRCfgEdge> EdgesByTo { get; }

    public IRCfgEdgesLookup(List<IRCfgEdge> edges, ILookup<uint, IRCfgEdge> edgesByFrom, ILookup<uint, IRCfgEdge> edgesByTo)
    {
        Edges = edges;
        EdgesByFrom = edgesByFrom;
        EdgesByTo = edgesByTo;
    }
}
