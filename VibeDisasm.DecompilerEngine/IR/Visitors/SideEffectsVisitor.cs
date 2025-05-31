using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public class SideEffectsVisitor : BaseIRNodeReturningVisitor<IReadOnlyList<IRFlagEffect>>
{
    public static readonly SideEffectsVisitor Instance = new();

    private SideEffectsVisitor() : base([])
    {
    }

    public override IReadOnlyList<IRFlagEffect>? VisitAdc(IRAdcInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitAdd(IRAddInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitAnd(IRAndInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Parity)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitCmp(IRCmpInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitDec(IRDecInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitDiv(IRDivInstruction instr) =>
    [
        new(IRFlag.Carry),
        new(IRFlag.Overflow)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitIDiv(IRIDivInstruction instr) =>
    [
        new(IRFlag.Carry),
        new(IRFlag.Overflow)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitInc(IRIncInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitMul(IRMulInstruction instr) =>
    [
        new(IRFlag.Carry),
        new(IRFlag.Overflow)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitNeg(IRNegInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitNot(IRNotInstruction instr) =>
    [
        new(IRFlag.Overflow),
        new(IRFlag.Carry),
        new(IRFlag.Sign),
        new(IRFlag.Zero),
        new(IRFlag.Parity)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitOr(IROrInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Parity)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitSbb(IRSbbInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitSub(IRSubInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitTest(IRTestInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override IReadOnlyList<IRFlagEffect>? VisitXor(IRXorInstruction instr) =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Parity)
    ];
}
