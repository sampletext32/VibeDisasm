using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;
using System.Collections.Generic;
using System.Linq;

namespace VibeDisasm.DecompilerEngine;

/// <summary>
/// Represents a directed edge in the control flow graph
/// </summary>
public class BlockEdge
{
    /// <summary>
    /// The source block address
    /// </summary>
    public uint SourceBlockAddress { get; set; }
    
    /// <summary>
    /// The target block address
    /// </summary>
    public uint TargetBlockAddress { get; set; }
    
    /// <summary>
    /// The type of edge (fallthrough, conditional jump, unconditional jump, etc.)
    /// </summary>
    public EdgeType EdgeType { get; set; }
    
    /// <summary>
    /// Additional information about the edge (e.g., jump condition)
    /// </summary>
    public string? Condition { get; set; }
    
    public override string ToString()
    {
        return $"{SourceBlockAddress:X8} -> {TargetBlockAddress:X8} [{EdgeType}{(Condition != null ? $", {Condition}" : "")}]";
    }
}

/// <summary>
/// Represents the type of edge in the control flow graph
/// </summary>
public enum EdgeType
{
    /// <summary>
    /// Natural flow from one block to the next (no jump)
    /// </summary>
    Fallthrough,
    
    /// <summary>
    /// Conditional jump that was taken
    /// </summary>
    ConditionalJump,
    
    /// <summary>
    /// Unconditional jump
    /// </summary>
    UnconditionalJump,
    
    /// <summary>
    /// Function call (may return)
    /// </summary>
    Call,
    
    /// <summary>
    /// Return from function
    /// </summary>
    Return
}

/// <summary>
/// Represents a control flow graph of instruction blocks
/// </summary>
public class BlockGraph
{
    /// <summary>
    /// The blocks in the graph, keyed by their starting address
    /// </summary>
    public Dictionary<uint, InstructionBlock> Blocks { get; set; } = new();
    
    /// <summary>
    /// The edges in the graph
    /// </summary>
    public List<BlockEdge> Edges { get; set; } = new();
    
    /// <summary>
    /// The entry points to the graph (block addresses)
    /// </summary>
    public HashSet<uint> EntryPoints { get; set; } = new();
    
    /// <summary>
    /// The exit points from the graph (block addresses)
    /// </summary>
    public HashSet<uint> ExitPoints { get; set; } = new();
    
    /// <summary>
    /// Gets all blocks that have edges coming into the specified block
    /// </summary>
    public List<InstructionBlock> GetPredecessors(uint blockAddress)
    {
        return Edges.Where(e => e.TargetBlockAddress == blockAddress)
                   .Select(e => Blocks[e.SourceBlockAddress])
                   .ToList();
    }
    
    /// <summary>
    /// Gets all blocks that have edges going out from the specified block
    /// </summary>
    public List<InstructionBlock> GetSuccessors(uint blockAddress)
    {
        return Edges.Where(e => e.SourceBlockAddress == blockAddress)
                   .Select(e => Blocks[e.TargetBlockAddress])
                   .ToList();
    }
    
    /// <summary>
    /// Gets all edges coming into the specified block
    /// </summary>
    public List<BlockEdge> GetIncomingEdges(uint blockAddress)
    {
        return Edges.Where(e => e.TargetBlockAddress == blockAddress).ToList();
    }
    
    /// <summary>
    /// Gets all edges going out from the specified block
    /// </summary>
    public List<BlockEdge> GetOutgoingEdges(uint blockAddress)
    {
        return Edges.Where(e => e.SourceBlockAddress == blockAddress).ToList();
    }
}

/// <summary>
/// Builds a control flow graph from a collection of instruction blocks
/// </summary>
public class InstructionBlockGraphBuilder
{
    /// <summary>
    /// Builds a control flow graph from a dictionary of instruction blocks
    /// </summary>
    /// <param name="blocks">Dictionary of instruction blocks keyed by their starting address</param>
    /// <param name="entryPointAddress">The entry point address to mark as the primary entry point</param>
    /// <returns>A control flow graph representing the relationships between blocks</returns>
    public static BlockGraph BuildGraph(Dictionary<uint, InstructionBlock> blocks, uint entryPointAddress)
    {
        var graph = new BlockGraph();
        
        // Add all blocks to the graph
        foreach (var block in blocks)
        {
            graph.Blocks.Add(block.Key, block.Value);
        }
        
        // Mark the entry point
        if (graph.Blocks.ContainsKey(entryPointAddress))
        {
            graph.EntryPoints.Add(entryPointAddress);
        }
        
        // Analyze each block to build edges
        foreach (var block in blocks)
        {
            uint blockAddress = block.Key;
            InstructionBlock instructionBlock = block.Value;
            
            // Skip empty blocks
            if (instructionBlock.Instructions.Count == 0)
            {
                continue;
            }
            
            // Get the last instruction in the block
            Instruction lastInstruction = instructionBlock.Instructions[^1];
            
            // Handle different types of control flow instructions
            if (lastInstruction.Type.IsRet())
            {
                // Mark as exit point
                graph.ExitPoints.Add(blockAddress);
            }
            else if (lastInstruction.Type.IsConditionalJump())
            {
                // For conditional jumps, we have two possible paths: taken and not taken
                
                // Handle the taken path (if we can determine the target)
                if (lastInstruction.StructuredOperands[0] is RelativeOffsetOperand jumpOperand)
                {
                    uint targetAddress = jumpOperand.TargetAddress;
                    
                    // Add edge for the taken path
                    if (graph.Blocks.ContainsKey(targetAddress))
                    {
                        var edge = new BlockEdge
                        {
                            SourceBlockAddress = blockAddress,
                            TargetBlockAddress = targetAddress,
                            EdgeType = EdgeType.ConditionalJump,
                            Condition = lastInstruction.Type.GetJumpConditionDescription()
                        };
                        
                        graph.Edges.Add(edge);
                    }
                }
                
                // Handle the not-taken path (fallthrough)
                uint fallthroughAddress = (uint)(lastInstruction.Address + lastInstruction.Length);
                if (graph.Blocks.ContainsKey(fallthroughAddress))
                {
                    var edge = new BlockEdge
                    {
                        SourceBlockAddress = blockAddress,
                        TargetBlockAddress = fallthroughAddress,
                        EdgeType = EdgeType.Fallthrough,
                        Condition = $"!{lastInstruction.Type.GetJumpConditionDescription()}"
                    };
                    
                    graph.Edges.Add(edge);
                }
            }
            else if (lastInstruction.Type.IsUnconditionalJump())
            {
                // For unconditional jumps, we have only one path
                if (lastInstruction.StructuredOperands[0] is RelativeOffsetOperand jumpOperand)
                {
                    uint targetAddress = jumpOperand.TargetAddress;
                    
                    // Add edge for the jump
                    if (graph.Blocks.ContainsKey(targetAddress))
                    {
                        var edge = new BlockEdge
                        {
                            SourceBlockAddress = blockAddress,
                            TargetBlockAddress = targetAddress,
                            EdgeType = EdgeType.UnconditionalJump
                        };
                        
                        graph.Edges.Add(edge);
                    }
                }
                else
                {
                    // Indirect jump (e.g., jmp eax) - can't determine target statically
                    // Mark as exit point for now
                    graph.ExitPoints.Add(blockAddress);
                }
            }
            else
            {
                // For any other instruction at the end of a block (likely a call followed by fallthrough)
                // Add fallthrough edge to the next block if it exists
                uint nextBlockAddress = (uint)(lastInstruction.Address + lastInstruction.Length);
                
                if (graph.Blocks.ContainsKey(nextBlockAddress))
                {
                    var edge = new BlockEdge
                    {
                        SourceBlockAddress = blockAddress,
                        TargetBlockAddress = nextBlockAddress,
                        EdgeType = EdgeType.Fallthrough
                    };
                    
                    graph.Edges.Add(edge);
                }
                
                // If the last instruction is a call, also add a call edge
                if (lastInstruction.Type.IsCall())
                {
                    if (lastInstruction.StructuredOperands[0] is RelativeOffsetOperand callOperand)
                    {
                        uint targetAddress = callOperand.TargetAddress;
                        
                        // Add edge for the call target if it exists in our blocks
                        if (graph.Blocks.ContainsKey(targetAddress))
                        {
                            var edge = new BlockEdge
                            {
                                SourceBlockAddress = blockAddress,
                                TargetBlockAddress = targetAddress,
                                EdgeType = EdgeType.Call
                            };
                            
                            graph.Edges.Add(edge);
                        }
                    }
                }
            }
        }
        
        // Find orphaned blocks (no incoming edges except entry points)
        foreach (var block in graph.Blocks)
        {
            uint blockAddress = block.Key;
            
            // Skip entry points
            if (graph.EntryPoints.Contains(blockAddress))
            {
                continue;
            }
            
            // If no incoming edges, mark as an entry point
            if (!graph.Edges.Any(e => e.TargetBlockAddress == blockAddress))
            {
                graph.EntryPoints.Add(blockAddress);
            }
        }
        
        return graph;
    }
    
    /// <summary>
    /// Visualizes the control flow graph as a DOT graph for visualization with tools like Graphviz
    /// </summary>
    public static string GenerateDotGraph(BlockGraph graph)
    {
        var dotGraph = new System.Text.StringBuilder();
        
        dotGraph.AppendLine("digraph ControlFlowGraph {");
        dotGraph.AppendLine("  node [shape=box, fontname=\"Consolas\", fontsize=10];");
        
        // Add nodes (blocks)
        foreach (var block in graph.Blocks)
        {
            uint blockAddress = block.Key;
            InstructionBlock instructionBlock = block.Value;
            
            string nodeStyle = "";
            if (graph.EntryPoints.Contains(blockAddress))
            {
                nodeStyle = ", style=filled, fillcolor=lightblue";
            }
            else if (graph.ExitPoints.Contains(blockAddress))
            {
                nodeStyle = ", style=filled, fillcolor=lightcoral";
            }
            
            // Create label with instructions
            var label = new System.Text.StringBuilder();
            label.AppendLine($"Block_{blockAddress:X8}");
            
            foreach (var instruction in instructionBlock.Instructions)
            {
                label.AppendLine($"{instruction.Address:X8}: {instruction}");
            }
            
            // Escape special characters
            string escapedLabel = label.ToString()
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n");
            
            dotGraph.AppendLine($"  \"Block_{blockAddress:X8}\" [label=\"{escapedLabel}\"{nodeStyle}];");
        }
        
        // Add edges
        foreach (var edge in graph.Edges)
        {
            string edgeStyle = edge.EdgeType switch
            {
                EdgeType.Fallthrough => "[color=black]",
                EdgeType.ConditionalJump => "[color=blue, label=\"Taken\"]",
                EdgeType.UnconditionalJump => "[color=green]",
                EdgeType.Call => "[color=purple, style=dashed]",
                EdgeType.Return => "[color=red, style=dotted]",
                _ => ""
            };
            
            dotGraph.AppendLine($"  \"Block_{edge.SourceBlockAddress:X8}\" -> \"Block_{edge.TargetBlockAddress:X8}\" {edgeStyle};");
        }
        
        dotGraph.AppendLine("}");
        
        return dotGraph.ToString();
    }
    
}