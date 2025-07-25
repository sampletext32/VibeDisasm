using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Enumeration type with named integer values
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeEnumType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    /// <summary>
    /// Underlying type of the enum (typically int, byte, etc.)
    /// </summary>
    public RuntimeTypeRefType UnderlyingType { get; set; }

    /// <summary>
    /// Collection of enum members with their values
    /// </summary>
    public List<RuntimeEnumMember> Members { get; set; }

    public RuntimeEnumType(Guid id, string @namespace, string name, RuntimeTypeRefType underlyingType, List<RuntimeEnumMember> members) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        UnderlyingType = underlyingType;
        Members = members;
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitEnum(this);

    protected internal override string DebugDisplay => $"enum {Name} : {UnderlyingType.DebugDisplay} {{ {Members.Count} members }}";
}

/// <summary>
/// Represents a named value in an enum
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class RuntimeEnumMember
{
    /// <summary>
    /// Name of the enum member
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Integer value of the enum member
    /// </summary>
    public long Value { get; set; }

    public RuntimeEnumMember(string name, long value)
    {
        Name = name;
        Value = value;
    }

    protected internal string DebugDisplay => $"{Name} = {Value}";
}
