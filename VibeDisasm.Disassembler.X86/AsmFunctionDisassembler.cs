using System.Diagnostics;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86;

public static class AsmFunctionDisassembler
{
    /// <summary>
    /// Disassembles an assembly function from a starting position in the fileBuffer.
    /// </summary>
    public static AsmFunction DisassembleFunction(byte[] fileBuffer, uint startPosition)
    {
        var decoder = new InstructionDecoder(fileBuffer, fileBuffer.Length);

        Queue<uint> offsetQueue = [];

        Dictionary<uint, AsmBlock> instructionBlocks = [];

        offsetQueue.Enqueue(startPosition);

        // while we have some blocks to disassemble
        while (offsetQueue.Count > 0)
        {
            var position = offsetQueue.Dequeue();

            // if this jump was already disassembled
            if (instructionBlocks.ContainsKey(position))
            {
                Debug.WriteLine($"AsmFunctionDisassembler stepped onto already visited block {position:X8}");
                continue;
            }

            // if this jump was already disassembled
            var existingBlock = instructionBlocks.Values.FirstOrDefault(x => x.Instructions.Any(y => y.Address == position));
            if (existingBlock is not null)
            {
                Debug.WriteLine($"AsmFunctionDisassembler at {position:X8} appeared in the middle of already existing block starting at {existingBlock.StartAddress:X8}. Splitting!");

                var firstInstructionOfNestedBlockIndex = existingBlock.Instructions.FindIndex(x => x.Address == position);

                if (firstInstructionOfNestedBlockIndex < 0)
                {
                    throw new InvalidOperationException("Unexpectedly didn't find an index in a nested block");
                }

                var innerBlock = new AsmBlock(position);
                innerBlock.Instructions = existingBlock.Instructions.Skip(firstInstructionOfNestedBlockIndex)
                    .ToList();

                existingBlock.Instructions = existingBlock.Instructions.Take(firstInstructionOfNestedBlockIndex).ToList();

                instructionBlocks[position] = innerBlock;

                Debug.WriteLine($"Split block at {existingBlock.StartAddress:X8}. Created block {innerBlock.StartAddress:X8}.");

                continue;
            }

            // only if we haven't been in this block - go disassemble it
            var block = new AsmBlock(position);

            List<AsmInstruction> blockInstructions = [];

            // jump to the block
            decoder.SetPosition(position);

            Debug.WriteLine($"Start disassembling block at {position:X8}");
            // disassemble while we can
            while (true)
            {
                var instruction = decoder.DecodeInstruction();

                if (instruction is null)
                {
                    throw new InvalidOperationException($"Failed disassembling instruction at {decoder.GetPosition():X8}");
                }

                var controlFlowInstruction = new AsmInstruction(instruction);

                blockInstructions.Add(controlFlowInstruction);

                if (instruction.Type.IsRet())
                {
                    block.Instructions = blockInstructions;
                    instructionBlocks[position] = block;

                    Debug.WriteLine($"Block at {position:X8} reached RET");
                    break;
                }
                else if (instruction.Type.IsConditionalJump())
                {
                    // finish current block
                    block.Instructions = blockInstructions;
                    instructionBlocks[position] = block;

                    // enqueue next position (in case a jump condition was not performed)
                    offsetQueue.Enqueue(decoder.GetPosition());

                    // enqueue the address to which we would jump if the condition was performed
                    if (instruction.StructuredOperands[0] is RelativeOffsetOperand roo)
                    {
                        offsetQueue.Enqueue(roo.TargetAddress);

                        Debug.WriteLine($"Block at {position:X8} reached CondJump {instruction.Type}. Enqueued {decoder.GetPosition():X8} continuation and {roo.TargetAddress:X8} jump");
                    }
                    else
                    {
                        throw new InvalidOperationException($"AsmFunctionDisassembler failed to determine jump target of: {instruction}");
                    }

                    break;
                }
                else if (instruction.Type.IsUnconditionalJump())
                {
                    // finish current block
                    block.Instructions = blockInstructions;
                    instructionBlocks[position] = block;

                    // enqueue address to which we jumped
                    if (instruction.StructuredOperands[0] is RelativeOffsetOperand roo)
                    {
                        offsetQueue.Enqueue(roo.TargetAddress);

                        Debug.WriteLine($"Block at {position:X8} reached UncondJump {instruction.Type}. Enqueued {roo.TargetAddress:X8} jump");
                    }
                    else
                    {
                        throw new InvalidOperationException($"AsmFunctionDisassembler failed to determine jump target of: {instruction}");
                    }

                    break;
                }
                else if (instructionBlocks.ContainsKey(decoder.GetPosition()))
                {
                    // reached another block start

                    block.Instructions = blockInstructions;
                    instructionBlocks[position] = block;
                    Debug.WriteLine($"Block at {position:X8} reached another block at {decoder.GetPosition():X8}.");
                    break;
                }
            }

            Debug.WriteLine($"Finished disassembling asm block at {block.StartAddress:X8}.");
        }

        // explicitly set the starting block as an entry block
        instructionBlocks[startPosition].IsEntryBlock = true;

        return new AsmFunction()
        {
            Blocks = instructionBlocks
        };
    }
}
