using System.Collections.Generic;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.State;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the assembly instruction viewer panel
/// </summary>
public class InstructionViewerViewModel : IViewModel
{
    private readonly AppState _state;
    
    // Collection of instructions to display
    public IReadOnlyList<InstructionInfo> Instructions { get; private set; } = [];
    
    // Currently selected instruction index
    public int SelectedInstructionIndex { get; private set; } = -1;
    
    // Mapping from address to index for quick lookups
    private Dictionary<ulong, int> _addressToIndexMap = [];
    
    // Current visible range for optimization
    public (int StartIndex, int EndIndex) VisibleRange { get; private set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public InstructionViewerViewModel(AppState state)
    {
        _state = state;
        state.FileLoaded += OnFileLoaded;
        state.InstructionSelected += OnInstructionSelected;
        state.EntryPointSelected += OnEntryPointSelected;
    }

    private void OnEntryPointSelected(EntryPointInfo entryPoint)
    {
        if (_state.OpenedFile == null) return;
        
        // Use the disassembler to get the function's instructions
        var function = AsmFunctionDisassembler.DisassembleFunction(_state.OpenedFile.FileData, entryPoint.FileOffset);
        
        // Convert the instructions to our view model format
        var instructions = new List<InstructionInfo>();
        
        // Process all blocks in the function
        foreach (var block in function.Blocks)
        {
            foreach (var instruction in block.Value.Instructions)
            {
                instructions.Add(new InstructionInfo(
                    instruction.Address,
                    instruction.Type.ToString("G"),
                    string.Join(", ", instruction.RawInstruction.StructuredOperands),
                    _state.OpenedFile.FileData.Skip((int) instruction.RawInstruction.Address).Take(instruction.Length).ToArray()
                ));
            }
        }
        
        // Set the instructions in the view model
        SetInstructions(instructions);
        
        // Select the first instruction if available
        if (instructions.Count > 0)
        {
            SelectInstruction(0);
        }
    }

    private void OnFileLoaded(PeFileState obj)
    {
        // Reset state when a new file is loaded
        ClearInstructions();
    }
    
    private void OnInstructionSelected(InstructionInfo instruction)
    {
        // Jump to the selected instruction if we have it
        if (_addressToIndexMap.TryGetValue(instruction.Address, out int index))
        {
            SelectInstruction(index);
        }
    }
    
    /// <summary>
    /// Sets the instructions to display
    /// </summary>
    public void SetInstructions(IReadOnlyList<InstructionInfo> instructions)
    {
        Instructions = instructions;
        SelectedInstructionIndex = -1;
        
        // Rebuild the address to index map
        _addressToIndexMap = [];
        for (int i = 0; i < instructions.Count; i++)
        {
            _addressToIndexMap[instructions[i].Address] = i;
        }
    }
    
    /// <summary>
    /// Clears all instructions
    /// </summary>
    public void ClearInstructions()
    {
        Instructions = [];
        SelectedInstructionIndex = -1;
        _addressToIndexMap = [];
    }
    
    // Flag to prevent recursion between SelectInstruction and OnInstructionSelected
    private bool _isSelectingInstruction = false;
    
    /// <summary>
    /// Selects an instruction by index
    /// </summary>
    public void SelectInstruction(int index)
    {
        if (index < 0 || index >= Instructions.Count) return;
        
        // Skip if already selected to avoid unnecessary work
        if (SelectedInstructionIndex == index) return;
        
        // Set selection
        SelectedInstructionIndex = index;
        
        // Prevent recursion by checking the flag
        if (!_isSelectingInstruction)
        {
            try
            {
                _isSelectingInstruction = true;
                _state.OnInstructionSelected(Instructions[index]);
            }
            finally
            {
                _isSelectingInstruction = false;
            }
        }
    }
    
    /// <summary>
    /// Updates the currently visible range of instructions
    /// </summary>
    public void UpdateVisibleRange(int startIndex, int endIndex)
    {
        VisibleRange = (startIndex, endIndex);
    }
    
    /// <summary>
    /// Jumps to a specific instruction by address
    /// </summary>
    public bool JumpToAddress(ulong address)
    {
        if (_addressToIndexMap.TryGetValue(address, out int index))
        {
            SelectInstruction(index);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Navigate to the next instruction
    /// </summary>
    public void NextInstruction()
    {
        if (SelectedInstructionIndex < Instructions.Count - 1)
        {
            SelectInstruction(SelectedInstructionIndex + 1);
        }
    }
    
    /// <summary>
    /// Navigate to the previous instruction
    /// </summary>
    public void PreviousInstruction()
    {
        if (SelectedInstructionIndex > 0)
        {
            SelectInstruction(SelectedInstructionIndex - 1);
        }
    }
}
