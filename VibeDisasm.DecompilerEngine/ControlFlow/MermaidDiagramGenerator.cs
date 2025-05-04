using System.Text;

namespace VibeDisasm.DecompilerEngine.ControlFlow;

/// <summary>
/// Generates Mermaid diagrams from control flow graphs
/// </summary>
public static class MermaidDiagramGenerator
{
    /// <summary>
    /// Generates a Mermaid flowchart diagram from a control flow function
    /// </summary>
    /// <param name="function">The control flow function to visualize</param>
    /// <returns>A string containing the Mermaid diagram markdown</returns>
    public static string GenerateDiagram(ControlFlowFunction function, ILookup<uint, ControlFlowEdge> edges)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        
        var sb = new StringBuilder();
        sb.AppendLine("```mermaid");
        sb.AppendLine("flowchart TD");
        
        // Add nodes
        foreach (var block in function.Blocks.Values)
        {
            string nodeId = GetNodeId(block.StartAddress);
            string label = GetNodeLabel(block);
            
            sb.AppendLine($"    {nodeId}[\"{label}\"]");
        }
        
        sb.AppendLine();
        
        // Add edges
        foreach (var edge in edges.SelectMany(group => group))
        {
            string fromNodeId = GetNodeId(edge.FromBlockAddress);
            string toNodeId = GetNodeId(edge.ToBlockAddress);
            string edgeStyle = GetEdgeStyle(edge.JumpType);
            string edgeLabel = GetEdgeLabel(edge.JumpType);
            
            sb.AppendLine($"    {fromNodeId} {edgeStyle} |{edgeLabel}|{toNodeId}");
        }
        
        sb.AppendLine("```");
        
        return sb.ToString();
    }

    // Helper methods
    private static string GetNodeId(uint address) => $"block_{address:X8}";
    
    private static string GetNodeLabel(ControlFlowBlock block)
    {
        var label = new StringBuilder();
        
        // Add block address
        label.Append($"Block {block.StartAddress:X8}");
        
        // Mark entry block
        if (block.IsEntryBlock)
        {
            label.Append(" (Entry)");
        }
        
        // Add instruction count
        label.Append($"\n{block.Instructions.Count} instructions");
        
        // Add last instruction type if available
        if (block.LastControlFlowInstruction is not null)
        {
            label.AppendLine();
            label.AppendLine(block.ToString());
        }
        
        return label.ToString();
    }
    
    private static string GetEdgeStyle(ControlFlowJumpType jumpType)
    {
        return jumpType switch
        {
            ControlFlowJumpType.Taken => "==>",      // Dashed arrow for taken jumps
            ControlFlowJumpType.Fallthrough => "-->", // Solid arrow for fallthrough
            _ => "-->"
        };
    }
    
    private static string GetEdgeLabel(ControlFlowJumpType jumpType)
    {
        return jumpType switch
        {
            ControlFlowJumpType.Taken => "taken",
            ControlFlowJumpType.Fallthrough => "fallthrough",
            _ => ""
        };
    }
}
