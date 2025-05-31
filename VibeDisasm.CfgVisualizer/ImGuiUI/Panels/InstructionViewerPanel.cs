using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying assembly instructions
/// </summary>
public class InstructionViewerPanel : IImGuiPanel
{
    private readonly InstructionViewerViewModel _viewModel;
    private string _addressInput = string.Empty;

    // Column widths
    private const float AddressColumnWidth = 120f;
    private const float OpcodeColumnWidth = 100f;

    // Tracking for auto-scroll
    private int _lastSelectedIndex = -1;
    private bool _programmaticSelection = false;

    // Table flags
    private const ImGuiTableFlags TableFlags =
        ImGuiTableFlags.ScrollY |
        ImGuiTableFlags.RowBg |
        ImGuiTableFlags.BordersOuter |
        ImGuiTableFlags.BordersV;

    /// <summary>
    /// Constructor
    /// </summary>
    public InstructionViewerPanel(InstructionViewerViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    /// <summary>
    /// Renders the instruction viewer panel
    /// </summary>
    public unsafe void OnImGuiRender()
    {
        if (!ImGui.Begin("Assembly Instructions"))
        {
            ImGui.End();
            return;
        }

        RenderNavigationControls();

        if (_viewModel.Instructions.Count == 0)
        {
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No instructions available");
            ImGui.End();
            return;
        }

        RenderInstructionsTable();

        ImGui.End();
    }

    /// <summary>
    /// Renders the navigation controls
    /// </summary>
    private void RenderNavigationControls()
    {
        ImGui.SetNextItemWidth(150);
        var enterPressed = ImGui.InputText("Address", ref _addressInput, 20,
            ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.EnterReturnsTrue);

        ImGui.SameLine();
        var jumpClicked = ImGui.Button("Jump");

        if (enterPressed || jumpClicked)
        {
            _programmaticSelection = true;
            TryJumpToAddress();
        }

        ImGui.SameLine();
        if (ImGui.Button("Previous"))
        {
            _programmaticSelection = true;
            _viewModel.PreviousInstruction();
        }

        ImGui.SameLine();
        if (ImGui.Button("Next"))
        {
            _programmaticSelection = true;
            _viewModel.NextInstruction();
        }

        ImGui.Separator();
    }

    /// <summary>
    /// Try to jump to the address entered in the input field
    /// </summary>
    private void TryJumpToAddress()
    {
        if (ulong.TryParse(_addressInput, System.Globalization.NumberStyles.HexNumber, null, out var address))
        {
            _viewModel.JumpToAddress(address);
        }
    }

    /// <summary>
    /// Renders the table of instructions with virtual scrolling
    /// </summary>
    private unsafe void RenderInstructionsTable()
    {
        if (!ImGui.BeginTable("InstructionTable", 3, TableFlags))
        {
            return;
        }

        // Setup columns
        ImGui.TableSetupColumn("Address", ImGuiTableColumnFlags.WidthFixed, AddressColumnWidth);
        ImGui.TableSetupColumn("Opcode", ImGuiTableColumnFlags.WidthFixed, OpcodeColumnWidth);
        ImGui.TableSetupColumn("Operands", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupScrollFreeze(0, 1); // Freeze header row
        ImGui.TableHeadersRow();

        // Virtual scrolling with clipper
        ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        clipper.Begin(_viewModel.Instructions.Count);

        while (clipper.Step())
        {
            _viewModel.UpdateVisibleRange(clipper.DisplayStart, clipper.DisplayEnd);

            for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
            {
                if (i < _viewModel.Instructions.Count)
                {
                    RenderInstructionRow(i);
                }
            }
        }

        // Only auto-scroll for programmatic selection changes (not user clicks)
        if (_programmaticSelection && _viewModel.SelectedInstructionIndex >= 0)
        {
            // Reset flag
            _programmaticSelection = false;
            _lastSelectedIndex = _viewModel.SelectedInstructionIndex;

            // Calculate position and scroll
            var scrollPos = ImGui.GetCursorStartPos().Y +
                _viewModel.SelectedInstructionIndex * ImGui.GetTextLineHeightWithSpacing();
            ImGui.SetScrollFromPosY(scrollPos);
        }
        else if (_lastSelectedIndex != _viewModel.SelectedInstructionIndex)
        {
            // Just update the last selected index without scrolling
            _lastSelectedIndex = _viewModel.SelectedInstructionIndex;
        }

        ImGui.EndTable();
    }

    /// <summary>
    /// Renders a single instruction row
    /// </summary>
    private void RenderInstructionRow(int index)
    {
        var instruction = _viewModel.Instructions[index];
        var isSelected = index == _viewModel.SelectedInstructionIndex;

        ImGui.TableNextRow();

        // Highlight selected row
        if (isSelected)
        {
            ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0,
                ImGui.GetColorU32(new Vector4(0.2f, 0.4f, 0.8f, 0.7f)));
        }

        // Address column
        ImGui.TableNextColumn();
        var addressText = $"0x{instruction.Address:X}";

        if (ImGui.Selectable(addressText, isSelected, ImGuiSelectableFlags.SpanAllColumns))
        {
            _viewModel.SelectInstruction(index);
        }

        // Opcode column
        ImGui.TableNextColumn();
        ImGui.Text(instruction.Opcode);

        // Operands column
        ImGui.TableNextColumn();
        ImGui.Text(instruction.Operands);
    }
}
