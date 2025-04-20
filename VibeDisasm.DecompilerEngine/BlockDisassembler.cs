using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine;

public class BlockDisassembler
{
    public static Dictionary<uint, InstructionBlock> DisassembleBlock(byte[] fileBuffer, uint startPosition)
    {
        var decoder = new InstructionDecoder(fileBuffer, fileBuffer.Length);

        Queue<uint> offsetQueue = [];

        Dictionary<uint, InstructionBlock> instructionBlocks = [];
        
        offsetQueue.Enqueue(startPosition);

        // while we have some blocks to disassemble
        while (offsetQueue.Count > 0)
        {
            var position = offsetQueue.Dequeue();
            
            // if this jump was already disassembled
            if (instructionBlocks.ContainsKey(position))
            {
                Console.WriteLine($"BlockDisassembler stepped onto already visited block {position:X8}");
                continue;
            }
            
            // if this jump was already disassembled
            var existingBlock = instructionBlocks.Values.FirstOrDefault(x => x.Instructions.Any(y => y.Address == position));
            if (existingBlock is not null)
            {
                Console.WriteLine($"BlockDisassembler at {position:X8} appeared in the middle of already existing block starting at {existingBlock.StartAddress:X8}. Splitting!");

                var firstInstructionOfNestedBlockIndex = existingBlock.Instructions.FindIndex(x => x.Address == position);

                if (firstInstructionOfNestedBlockIndex < 0)
                {
                    throw new InvalidOperationException("Unexpectedly didn't find an index in a nested block");
                }

                var innerBlock = new InstructionBlock();
                innerBlock.StartAddress = position;
                innerBlock.Instructions = existingBlock.Instructions.Skip(firstInstructionOfNestedBlockIndex)
                    .ToList();

                existingBlock.Instructions = existingBlock.Instructions.Take(firstInstructionOfNestedBlockIndex).ToList();

                instructionBlocks[position] = innerBlock;
                
                continue;
            }

            // only if we haven't been in this block - go disassemble it
            var block = new InstructionBlock();
            block.StartAddress = position;

            List<Instruction> blockInstructions = [];

            // jump to the block
            decoder.SetPosition(position);

            // disassemble while we can
            while (true)
            {
                var instruction = decoder.DecodeInstruction();

                if (instruction is null)
                {
                    throw new InvalidOperationException($"Failed disassembling instruction at {decoder.GetPosition()}");
                }

                blockInstructions.Add(instruction);

                if (instruction.Type.IsRet())
                {
                    block.Instructions = blockInstructions;
                    instructionBlocks[position] = block;
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
                    }
                    else
                    {
                        throw new InvalidOperationException($"BlockDisassembler failed to determine jump target of: {instruction}");
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
                    }
                    else
                    {
                        throw new InvalidOperationException($"BlockDisassembler failed to determine jump target of: {instruction}");
                    }
                    break;
                }
            }
        }

        return instructionBlocks;
    }
}