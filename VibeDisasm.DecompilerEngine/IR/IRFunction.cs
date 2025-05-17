using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Represents a function in the intermediate representation.
/// <para>
/// An IRFunction contains a collection of basic blocks (IRBlockStatement) that make up the function's body.
/// Each block is identified by its starting address in the original binary.
/// </para>
/// <para>
/// Example function structure:
/// ```
/// Function MyFunction:
///   Block_0x1000 (Entry):
///   {
///     eax = 0;
///     ebx = [ebp+8];
///     if (ebx == 0) goto Block_0x1020;
///   }
///   
///   Block_0x1010:
///   {
///     eax = ebx;
///     return eax;
///   }
///   
///   Block_0x1020:
///   {
///     eax = -1;
///     return eax;
///   }
/// ```
/// </para>
/// </summary>
public class IRFunction : IRNode
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<uint, IRBlockStatement> Blocks { get; } = [];
    public IRBlockStatement? EntryBlock { get; private set; }
    public ControlFlowFunction? SourceFunction { get; }
    
    public IRFunction(ControlFlowFunction? sourceFunction = null) 
        : base(IRNodeType.Block)
    {
        SourceFunction = sourceFunction;
    }
    
    public void AddBlock(uint address, IRBlockStatement block)
    {
        Blocks[address] = block;
        AddChild(block);
        
        // If this is the entry block, set it
        if (SourceFunction != null && 
            SourceFunction.Blocks.TryGetValue(address, out var cfgBlock) && 
            cfgBlock.IsEntryBlock)
        {
            EntryBlock = block;
        }
    }
}
