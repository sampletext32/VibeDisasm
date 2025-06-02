using VibeDisasm.Core;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Visitors;

public class CodeEmitVisitor : BaseIRNodeReturningVisitor<string>
{
    public static CodeEmitVisitor Instance = new();

    private string _indent = "";

    public CodeEmitVisitor Indent()
    {
        _indent += "  ";
        return this;
    }
    public CodeEmitVisitor Unindent()
    {
        if (_indent.Length >= 2)
        {
            _indent = _indent[..^2];
        }
        else
        {
            _indent = "";
        }

        return this;
    }

    public string Nop() => "";

    public void Reset()
    {
        _indent = "";
    }

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

    // TODO: flag here is a string constant, not an IRFlagExpr
    public override string? VisitAdc(IRAdcInstruction instr) => $"{_indent}{Visit(instr.Left)} += {Visit(instr.Right)} + CF";

    public override string? VisitAdd(IRAddInstruction instr) => $"{_indent}{Visit(instr.Destination)} += {Visit(instr.Source)}";

    public override string? VisitAnd(IRAndInstruction instr) => $"{_indent}{Visit(instr.Left)} &= {Visit(instr.Right)}";

    public override string? VisitCall(IRCallInstruction instr) => $"{_indent}call {Visit(instr.Target)}";

    public override string? VisitCmp(IRCmpInstruction instr) => $"{_indent}Compare({Visit(instr.Left)}, {Visit(instr.Right)})";

    public override string? VisitDec(IRDecInstruction instr) => $"{_indent}{Visit(instr.Target)}--";

    public override string? VisitDiv(IRDivInstruction instr) => $"{_indent}{Visit(instr.DestQuotient)} = {Visit(instr.Dividend)} / {Visit(instr.Divisor)}; {Visit(instr.DestRemainder)} = {Visit(instr.Dividend)} % {Visit(instr.Divisor)}";

    public override string? VisitIDiv(IRIDivInstruction instr) => $"{_indent}{Visit(instr.DestQuotient)} = {Visit(instr.Dividend)} / {Visit(instr.Divisor)}; {Visit(instr.DestRemainder)} = {Visit(instr.Dividend)} % {Visit(instr.Divisor)}";

    public override string? VisitInc(IRIncInstruction instr) => $"{_indent}{Visit(instr.Target)}++";

    public override string? VisitJump(IRJumpInstruction instr) => $"{_indent}jump -> {Visit(instr.Target)}";

    public override string? VisitConditionalJump(IRConditionalJumpInstruction instr) => $"{_indent}jump_if {Visit(instr.Condition)} -> {Visit(instr.Target)}";

    public override string? VisitLea(IRLeaInstruction instr) => $"{_indent}{Visit(instr.Target)} = &{Visit(instr.Address)}";

    public override string? VisitMove(IRMoveInstruction instr) => $"{_indent}{Visit(instr.Destination)} = {Visit(instr.Source)}";

    public override string? VisitMul(IRMulInstruction instr) => $"{_indent}{Visit(instr.Left)} *= {Visit(instr.Right)}";

    public override string? VisitNeg(IRNegInstruction instr) => $"{_indent}{Visit(instr.Target)} = -{Visit(instr.Target)}";

    public override string? VisitNot(IRNotInstruction instr) => $"{_indent}{Visit(instr.Operand)} = ~{Visit(instr.Operand)}";

    public override string? VisitOr(IROrInstruction instr) => $"{_indent}{Visit(instr.Left)} |= {Visit(instr.Right)}";

    public override string? VisitPop(IRPopInstruction instr) => $"{_indent}pop({Visit(instr.Target)})";
    public override string? VisitPush(IRPushInstruction instr) => $"{_indent}push({Visit(instr.Value)})";

    public override string? VisitReturn(IRReturnInstruction instr) => instr.Value is null
        ? $"{_indent}return"
        : $"{_indent}return {Visit(instr.Value)}";

    // TODO: flag here is a string constant, not an IRFlagExpr
    public override string? VisitSbb(IRSbbInstruction instr) => $"{_indent}{Visit(instr.Left)} -= {Visit(instr.Right)} - CF";

    public override string? VisitSub(IRSubInstruction instr) => $"{_indent}{Visit(instr.Destination)} -= {Visit(instr.Source)}";

    public override string? VisitTest(IRTestInstruction instr) => $"{_indent}Test({Visit(instr.Left)}, {Visit(instr.Right)})";

    public override string? VisitXor(IRXorInstruction instr) => $"{_indent}{Visit(instr.Left)} ^= {Visit(instr.Right)}";

    public override string? VisitStub(StubIRInstruction instr) => $"{_indent}StubIRInstruction - {instr.InstructionType:G} Not implemented";

    public override string? VisitWiredJump(IRWiredJumpInstruction instr) => instr.WrappedInstruction.Condition == null
        ? $"{_indent}jump to {Visit(instr.WrappedInstruction.Target)}" // Unconditional jump
        : Visit(instr.WrappedInstruction);

    public override string? VisitSemanticIfJump(IRSemanticIfJumpInstruction instr) => $"{_indent}if ({Visit(instr.Condition)}) goto {Visit(instr.WrappedInstruction.Target)}";

    public override string? VisitFld(IRFldInstruction instr) => $"{_indent}ST(0) = fld({Visit(instr.Source)}) /* push FPU stack */";

    public override string? VisitFstp(IRFstpInstruction instr) => $"{_indent}{Visit(instr.Destination)} = ST(0); /* pop FPU stack */"; // Store and pop

    public override string? VisitFadd(IRFaddInstruction instr) => $"{_indent}ST(0) += {Visit(instr.Source)}"; // Add to ST(0)

    public override string? VisitShl(IRShlInstruction instr) => $"{_indent}{Visit(instr.Value)} <<= {Visit(instr.ShiftCount)}";

    public override string? VisitShr(IRShrInstruction instr) => $"{_indent}{Visit(instr.Value)} >>= {Visit(instr.ShiftCount)}";

    public override string? VisitMovzx(IRMovzxInstruction instr) => $"{_indent}{Visit(instr.Destination)} = (uint){Visit(instr.Source)}";

    public override string? VisitBlock(IRBlock block)
    {
        return $"{_indent}{{\n" +
               $"{Indent()._indent}// {block.Address:X8}\n" +
               string.Join($"{_indent}\n", block.Instructions.Select(Visit)) +
               $"\n" +
               $"{Unindent()._indent}}}";
    }

    public override string? VisitIfThen(IRIfThenNode node)
    {
        return $"{_indent}if ({Visit(node.Condition)})\n" +
               $"{Visit(node.ThenBlock)}";
    }

    public override string? VisitFunction(IRFunction function) => $"{Visit(function.ReturnType)} {function.Name}({string.Join(", ", function.Parameters.Select(Visit))})\n{Visit(function.Body)}";
    public override string? VisitSequence(IRSequenceNode node) => $"{string.Join($"\n{_indent}", node.Nodes.Select(Visit))}";
}
