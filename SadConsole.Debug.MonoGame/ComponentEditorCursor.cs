using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Debug.MonoGame.ImGuiTypes;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame;

internal class ComponentEditorCursor : ImGuiObjectBase
{
    private Cursor _stateComponent;
    private IScreenObject _screenObject;

    private bool _isEnabled;
    private bool _isVisible;
    private ImGuiColoredGlyphEditor _printEffectEditor;
    private bool _printAppearanceMatchesHost;

    private ImGuiColoredGlyphEditor _cursorAppearanceEditor;
    private SadConsole.ColoredGlyph _cursorAppearance;

    private int _positionX;
    private int _positionY;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_screenObject != GuiState._selectedScreenObjectState.Object || _stateComponent != GuiState._selectedScreenObjectState.Object.SadComponents[GuiState._selectedScreenObjectState.ComponentsSelectedItem])
        {
            _screenObject = GuiState._selectedScreenObjectState.Object;
            _stateComponent = (Cursor)GuiState._selectedScreenObjectState.Object.SadComponents[GuiState._selectedScreenObjectState.ComponentsSelectedItem];

            _printAppearanceMatchesHost = _stateComponent.PrintAppearanceMatchesHost;
            _isEnabled = _stateComponent.IsEnabled;
            _isVisible = _stateComponent.IsVisible;

            _printEffectEditor = new ImGuiColoredGlyphEditor();
            _cursorAppearanceEditor = new ImGuiColoredGlyphEditor();

            // Copy the render state of the cursor
            _cursorAppearance = new ColoredGlyph();
            _stateComponent.CursorRenderCell.RestoreState(ref _cursorAppearance);

            // Position
            _positionX = _stateComponent.Position.X;
            _positionY = _stateComponent.Position.Y;
        }

        ImGui.BeginGroup();
        {
            
            // Print appearance
            ImGui.AlignTextToFramePadding();
            ImGui.Bullet();
            if (ImGui.Checkbox("Print appearance matches host?", ref _printAppearanceMatchesHost))
                _stateComponent.PrintAppearanceMatchesHost = _printAppearanceMatchesHost;

            // Print appearance details
            if (!_printAppearanceMatchesHost)
            {
                ImGui.Indent();
                _printEffectEditor.BuildUI("comp_cur_edit_printappearance", renderer, _stateComponent.PrintAppearance, ((IScreenSurface)_screenObject).Font, ImGuiColoredGlyphEditor.Modes.Fore | ImGuiColoredGlyphEditor.Modes.Back);
                ImGui.Unindent();
            }

            // Cursor render cell
            ImGui.BulletText("Render Appearance");
            ImGui.Indent();
            if (_cursorAppearanceEditor.BuildUI("comp_cur_edit_renderappearance", renderer, _cursorAppearance, ((IScreenSurface)_screenObject).Font, ImGuiColoredGlyphEditor.Modes.Glyph | ImGuiColoredGlyphEditor.Modes.Fore | ImGuiColoredGlyphEditor.Modes.Back))
            {
                // was edited, apply it back to cursor
                _stateComponent.CursorRenderCell = _cursorAppearance.ToState();
            }
            ImGui.Unindent();

            // Position
            ImGui.AlignTextToFramePadding();
            ImGui.BulletText($"Position X: {_stateComponent.Position.X} Y: {_stateComponent.Position.Y}");
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);
            if (ImGui.Button("Change"))
                ImGui.OpenPopup("comp_cur_edit_position");

            if (XYPopup.BuildUI("comp_cur_edit_position", renderer, ref _positionX, ref _positionY, "X", "Y"))
            {
                _stateComponent.Position = (_positionX, _positionY);
            }
            else
            {
                if (!ImGui.IsPopupOpen("comp_cur_edit_position"))
                {
                    _positionX = _stateComponent.Position.X;
                    _positionY = _stateComponent.Position.Y;
                }
            }
        }
        ImGui.EndGroup();
        
    }
}
