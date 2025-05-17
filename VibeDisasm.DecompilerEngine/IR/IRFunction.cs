using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Represents a function in the intermediate representation
/// </summary>
public class IRFunction : IRNode
{
    public string Name { get; set; } = string.Empty;
    public IRBlockStatement Body { get; }
    public ControlFlowFunction? SourceFunction { get; }
    
    public IRFunction(IRBlockStatement body, ControlFlowFunction? sourceFunction = null) 
        : base(IRNodeType.Block)
    {
        Body = body;
        SourceFunction = sourceFunction;
        AddChild(body);
    }
}
