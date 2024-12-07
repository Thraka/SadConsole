using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

internal class ComponentEditorCursor : ISadComponentPanel
{
    private Cursor _stateComponent;
    private IScreenObject _screenObject;

    private bool _isEnabled;
    private bool _isVisible;
    private bool _printAppearanceMatchesHost;
    private bool _printOnlyCharData;
    private bool _printDisableAutoLineFeed;
    private bool _disableWordBreak;

    private ColoredGlyphBase _cursorAppearance;

    private int _positionX;
    private int _positionY;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component)
    {
        // Capture the state information of the component as it is
        if (_screenObject != state.Object || _stateComponent != state.Object.SadComponents[state.ComponentsSelectedItem])
        {
            _screenObject = state.Object;
            _stateComponent = (Cursor)state.Object.SadComponents[state.ComponentsSelectedItem];

            _printAppearanceMatchesHost = _stateComponent.PrintAppearanceMatchesHost;
            _printOnlyCharData = _stateComponent.PrintOnlyCharacterData;
            _printDisableAutoLineFeed = _stateComponent.DisablePrintAutomaticLineFeed;
            _disableWordBreak = _stateComponent.DisableWordBreak;
            _isEnabled = _stateComponent.IsEnabled;
            _isVisible = _stateComponent.IsVisible;

            // Copy the render state of the cursor
            _cursorAppearance = new SadConsole.ColoredGlyph();
            _stateComponent.CursorRenderCell.CopyAppearanceTo(_cursorAppearance);

            // Position
            _positionX = _stateComponent.Position.X;
            _positionY = _stateComponent.Position.Y;
        }

        // Draw the ImGUI interface
        ImGui.BeginGroup();
        {
            // Print appearance
            ImGui.AlignTextToFramePadding();
            if (ImGui.CollapsingHeader("Render Appearance"))
            {
                SettingsTable.BeginTable("cur_rend_appearance");
                ColoredGlyphReference glyphRef = _cursorAppearance;
                SettingsTable.DrawCommonSettings("settings", true, true, true, true, true, ref glyphRef,
                                                                                           SadRogue.Primitives.Color.White.ToVector4(),
                                                                                           SadRogue.Primitives.Color.Black.ToVector4(),
                                                                                           ((IScreenSurface)_screenObject).Font, Debugger.Renderer);
                // Something changed
                if (glyphRef != _cursorAppearance)
                {
                    _cursorAppearance.Foreground = glyphRef.Foreground.ToColor();
                    _cursorAppearance.Background = glyphRef.Background.ToColor();
                    _cursorAppearance.Mirror = glyphRef.Mirror;
                    _cursorAppearance.Glyph = glyphRef.Glyph;
                    _stateComponent.CursorRenderCell.CopyAppearanceFrom(_cursorAppearance);
                }
                SettingsTable.EndTable();
            }

            if (ImGui.CollapsingHeader("Printing Behavior"))
            {
                if (ImGui.Checkbox("Print appearance matches host?", ref _printAppearanceMatchesHost))
                    _stateComponent.PrintAppearanceMatchesHost = _printAppearanceMatchesHost;

                ImGui.BeginDisabled(_printAppearanceMatchesHost);
                SettingsTable.BeginTable("cur_print_appearance");

                ColoredGlyphReference glyphRef = _stateComponent.PrintAppearance;
                SettingsTable.DrawCommonSettings("settings", true, true, true, false, true, ref glyphRef,
                                                                                           SadRogue.Primitives.Color.White.ToVector4(),
                                                                                           SadRogue.Primitives.Color.Black.ToVector4(),
                                                                                           ((IScreenSurface)_screenObject).Font, Debugger.Renderer);
                // Something changed
                if (glyphRef != _cursorAppearance)
                {
                    _stateComponent.PrintAppearance.Foreground = glyphRef.Foreground.ToColor();
                    _stateComponent.PrintAppearance.Background = glyphRef.Background.ToColor();
                    _stateComponent.PrintAppearance.Mirror = glyphRef.Mirror;
                }
                SettingsTable.EndTable();
                ImGui.EndDisabled();

                if (ImGui.Checkbox("Print only character data?", ref _printOnlyCharData))
                    _stateComponent.PrintOnlyCharacterData = _printOnlyCharData;
                if (ImGui.Checkbox("Disable printing automatic line feed?", ref _printDisableAutoLineFeed))
                    _stateComponent.DisablePrintAutomaticLineFeed = _printDisableAutoLineFeed;
                if (ImGui.Checkbox("Disable word break?", ref _disableWordBreak))
                    _stateComponent.DisableWordBreak = _disableWordBreak;
                if (ImGui.Checkbox("Is Enabled", ref _isEnabled))
                    _stateComponent.IsEnabled = _isEnabled;
                if (ImGui.Checkbox("Is Visible", ref _isVisible))
                    _stateComponent.IsVisible = _isVisible;
            }

            if (ImGui.CollapsingHeader("Settings", ImGuiTreeNodeFlags.DefaultOpen))
            {
                // Position
                ImGui.AlignTextToFramePadding();
                ImGui.BulletText($"Position X: {_stateComponent.Position.X} Y: {_stateComponent.Position.Y}");
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);
                if (ImGui.Button("Change"))
                    ImGui.OpenPopup("comp_cur_edit_position");
            }

            // Popup for settings
            if (ImGui2.XYEditPopup("comp_cur_edit_position", ref _positionX, ref _positionY, "X", "Y"))
                _stateComponent.Position = (_positionX, _positionY);

            //else if (!ImGuiP.IsPopupOpen("comp_cur_edit_position"))
            //{
            //    _positionX = _stateComponent.Position.X;
            //    _positionY = _stateComponent.Position.Y;
            //}
        }
        ImGui.EndGroup();

    }
}
