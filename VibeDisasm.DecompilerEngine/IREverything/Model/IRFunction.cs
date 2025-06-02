using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Cfg;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents a function in IR form.
/// Example: int add(int a, int b) { return a + b; } -> IRFunction(Name="add", ...)
/// </summary>
[DebuggerDisplay("{DebugDisplay,nq}")]
public class IRFunction : IRNode
{
    public string Name { get; init; }
    public IRType ReturnType { get; init; }
    public List<IRVariable> Parameters { get; init; }
    public IRSequenceNode Body { get; init; }

    public IRCfgEdgesLookup CfgEdges { get; init; }

    public IRFunction(string name, IRType returnType, List<IRVariable> parameters, IRSequenceNode body, IRCfgEdgesLookup cfgEdges)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
        Body = body;
        CfgEdges = cfgEdges;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitFunction(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFunction(this);

    internal override string DebugDisplay => $"{ReturnType.DebugDisplay} {Name}({string.Join(", ", Parameters)})";
}
