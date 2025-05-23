﻿namespace VibeDisasm.DecompilerEngine.ControlFlow;

public class ControlFlowEdge
{
    public uint FromBlockAddress { get; set; }

    public uint ToBlockAddress { get; set; }

    public ControlFlowJumpType JumpType { get; set; }
}