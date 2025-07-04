using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents a type in IR.
/// Example: int, float*, struct S
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRType : IRNode
{
    public string Name { get; init; }

    private IRType(string name) => Name = name;

    public static IRType Byte => new IRType("byte");
    public static IRType Int => new IRType("int");
    public static IRType Uint => new IRType("uint");
    public static IRType Short => new IRType("short");
    public static IRType UShort => new IRType("ushort");
    public static IRType Long => new IRType("long");
    public static IRType Ulong => new IRType("ulong");
    public static IRType Bool => new IRType("bool");

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitType(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitType(this);

    internal override string DebugDisplay => $"IRType({Name})";
}
