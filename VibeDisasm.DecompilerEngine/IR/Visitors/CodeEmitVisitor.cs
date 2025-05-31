using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public class CodeEmitVisitor : BaseIRNodeReturningVisitor<string>
{
    public static CodeEmitVisitor Instance = new();

    private CodeEmitVisitor() : base(node => $"{nameof(CodeEmitVisitor)}: Unsupported IR node: {node.GetType().Name}")
    {
    }

    public override string? VisitAdd(IRAddExpr expr) => $"{Visit(expr.Left)} + {Visit(expr.Right)}";

    public override string? VisitCompare(IRCompareExpr expr) => $"{Visit(expr.Left)} {expr.Comparison.ToLangString()} {Visit(expr.Right)}";

    public override string? VisitConstant(IRConstantExpr expr) => expr.Value switch
    {
        byte i => NumberFormatter.FormatNumber(i),
        short i => NumberFormatter.FormatNumber(i),
        ushort u => NumberFormatter.FormatNumber(u),
        int i => NumberFormatter.FormatNumber(i),
        uint u => NumberFormatter.FormatNumber(u),
        long l => NumberFormatter.FormatNumber(l),
        ulong ul => NumberFormatter.FormatNumber(ul),
        bool b => b
            ? "true"
            : "false",
        _ => $"!unknown type constant {expr.Value.GetType().Name}!"
    };

    public override string? VisitDeref(IRDerefExpr expr) => $"*({Visit(expr.Address)})";

    public override string? VisitFlag(IRFlagExpr expr) => expr.Flag.ToString();

    public override string? VisitLogical(IRLogicalExpr expr) => $"{Visit(expr.Left)} {expr.Operation.ToLangString()} {Visit(expr.Right)}";

    public override string? VisitMemory(IRMemoryExpr expr) => $"[{expr.Address}]";

    public override string? VisitMul(IRMulExpr expr) => $"{Visit(expr.Left)} * {Visit(expr.Right)}";

    public override string? VisitNot(IRNotExpr expr) => $"~{Visit(expr.Value)}";

    public override string? VisitRegister(IRRegisterExpr expr) => $"{expr.Register:G}";

    public override string? VisitSegment(IRSegmentExpr expr) => $"{expr.Segment}";

    public override string? VisitSub(IRSubExpr expr) => $"{Visit(expr.Left)} - {Visit(expr.Right)}";

    public override string? VisitXor(IRXorExpr expr) => $"{Visit(expr.Left)} ^ {Visit(expr.Right)}";

    public override string? VisitType(IRType type) => $"{type.Name}";

    public override string? VisitBlock(IRBlock block) => $"// Block {block.Id}\n" + string.Join("\n", block.Instructions.Select(Visit));

    public override string? VisitFunction(IRFunction function) => $"{Visit(function.ReturnType)} {function.Name}({string.Join(", ", function.Parameters.Select(Visit))})\n" + string.Join("\n\n", function.Blocks.Select(Visit));

    // TODO: flag here is a string constant, not an IRFlagExpr
    public override string? VisitAdc(IRAdcInstruction instr) => $"{Visit(instr.Left)} += {Visit(instr.Right)} + CF";

    public override string? VisitAdd(IRAddInstruction instr) => $"{Visit(instr.Destination)} += {Visit(instr.Source)}";

    public override string? VisitAnd(IRAndInstruction instr) => $"{Visit(instr.Left)} &= {Visit(instr.Right)}";

    public override string? VisitCall(IRCallInstruction instr) => $"call {Visit(instr.Target)}";

    public override string? VisitCmp(IRCmpInstruction instr) => $"Compare({Visit(instr.Left)}, {Visit(instr.Right)})";

    public override string? VisitDec(IRDecInstruction instr) => $"{Visit(instr.Target)}--";

    public override string? VisitDiv(IRDivInstruction instr) => $"{Visit(instr.DestQuotient)} = {Visit(instr.Dividend)} / {Visit(instr.Divisor)}; {Visit(instr.DestRemainder)} = {Visit(instr.Dividend)} % {Visit(instr.Divisor)}";

    public override string? VisitIDiv(IRIDivInstruction instr) => $"{Visit(instr.DestQuotient)} = {Visit(instr.Dividend)} / {Visit(instr.Divisor)}; {Visit(instr.DestRemainder)} = {Visit(instr.Dividend)} % {Visit(instr.Divisor)}";

    public override string? VisitInc(IRIncInstruction instr) => $"{Visit(instr.Target)}++";

    public override string? VisitJump(IRJumpInstruction instr) => (instr.Condition is null
        ? $"jump -> {Visit(instr.Target)}"
        : $"jump_if {Visit(instr.Condition)} -> {Visit(instr.Target)}");

    public override string? VisitLea(IRLeaInstruction instr) => $"{Visit(instr.Target)} = &{Visit(instr.Address)}";

    public override string? VisitMove(IRMoveInstruction instr) => $"{Visit(instr.Destination)} = {Visit(instr.Source)}";

    public override string? VisitMul(IRMulInstruction instr) => $"{Visit(instr.Left)} *= {Visit(instr.Right)}";

    public override string? VisitNeg(IRNegInstruction instr) => $"{Visit(instr.Target)} = -{Visit(instr.Target)}";

    public override string? VisitNot(IRNotInstruction instr) => $"{Visit(instr.Operand)} = ~{Visit(instr.Operand)}";

    public override string? VisitOr(IROrInstruction instr) => $"{Visit(instr.Left)} |= {Visit(instr.Right)}";

    public override string? VisitPop(IRPopInstruction instr) => $"pop({Visit(instr.Target)})";
    public override string? VisitPush(IRPushInstruction instr) => $"push({Visit(instr.Value)})";

    public override string? VisitReturn(IRReturnInstruction instr) => instr.Value is null
        ? "return"
        : $"return {Visit(instr.Value)}";

    // TODO: flag here is a string constant, not an IRFlagExpr
    public override string? VisitSbb(IRSbbInstruction instr) => $"{Visit(instr.Left)} -= {Visit(instr.Right)} - CF";

    public override string? VisitSub(IRSubInstruction instr) => $"{Visit(instr.Destination)} -= {Visit(instr.Source)}";

    public override string? VisitTest(IRTestInstruction instr) => $"Test({Visit(instr.Left)}, {Visit(instr.Right)})";

    public override string? VisitXor(IRXorInstruction instr) => $"{Visit(instr.Left)} ^= {Visit(instr.Right)}";

    public override string? VisitStub(StubIRInstruction instr) => $"StubIRInstruction - {instr.InstructionType:G} Not implemented";

    public override string? VisitWiredJump(IRWiredJumpInstruction instr) => instr.WrappedInstruction.Condition == null
        ? $"jump to {Visit(instr.WrappedInstruction.Target)}" // Unconditional jump
        : Visit(instr.WrappedInstruction);

    public override string? VisitUnflaggedJump(IRUnflaggedJumpInstruction instr) => $"if ({Visit(instr.Condition)}) goto {Visit(instr.WrappedInstruction.Target)}";
}
