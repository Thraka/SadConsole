using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Components;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

internal class ComponentEditorCursor : ISadComponentPanel
{
    private Cursor _stateComponent;
    private IScreenObject _screenObject;
    
    private ColoredGlyphBase _cursorAppearance;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component)
    {
        // Capture the state information of the component as it is
        if (_screenObject != state.Object || _stateComponent != state.Object.SadComponents[state.ComponentsSelectedItem])
        {
            _screenObject = state.Object;
            _stateComponent = (Cursor)state.Object.SadComponents[state.ComponentsSelectedItem];

            // Copy the render state of the cursor
            _cursorAppearance = new SadConsole.ColoredGlyph();
            _stateComponent.CursorRenderCell.CopyAppearanceTo(_cursorAppearance);
            
        }

        bool printAppearanceMatchesHost = _stateComponent.PrintAppearanceMatchesHost;
        bool printOnlyCharData = _stateComponent.PrintOnlyCharacterData;
        bool printDisableAutoLineFeed = _stateComponent.DisablePrintAutomaticLineFeed;
        bool disableWordBreak = _stateComponent.DisableWordBreak;
        bool isEnabled = _stateComponent.IsEnabled;
        bool isVisible = _stateComponent.IsVisible;

        int positionX = _stateComponent.Position.X;
        int positionY = _stateComponent.Position.Y;

        // Draw the ImGUI interface
        ImGui.BeginGroup();
        {
            // Print appearance
            ImGui.AlignTextToFramePadding();
            if (ImGui.CollapsingHeader("Render Appearance"u8))
            {
                if (SettingsTable.BeginTable("cur_rend_appearance"))
                {
                    ImGuiTypes.ColoredGlyphReference glyphRef = _cursorAppearance;
                    SettingsTable.DrawCommonSettings(true, true, true, true, true, ref glyphRef,
                        SadRogue.Primitives.Color.White.ToVector4(),
                        SadRogue.Primitives.Color.Black.ToVector4(),
                        ((IScreenSurface)_screenObject).Font, ImGuiCore.Renderer);

                    // Something changed
                    if (glyphRef != _cursorAppearance)
                    {
                        _cursorAppearance.Foreground = glyphRef.Foreground.ToColor();
                        _cursorAppearance.Background = glyphRef.Background.ToColor();
                        _cursorAppearance.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(glyphRef.Mirror);
                        _cursorAppearance.Glyph = glyphRef.Glyph;
                        _stateComponent.CursorRenderCell.CopyAppearanceFrom(_cursorAppearance);
                    }

                    SettingsTable.EndTable();
                }

                bool useRenderEffect = _stateComponent.ApplyCursorEffect;
                if (ImGui.Checkbox("Apply Cursor Effect", ref useRenderEffect))
                    _stateComponent.ApplyCursorEffect = useRenderEffect;
            }

            if (ImGui.CollapsingHeader("Printing Behavior"u8))
            {
                if (ImGui.Checkbox("Print appearance matches host?"u8, ref printAppearanceMatchesHost))
                    _stateComponent.PrintAppearanceMatchesHost = printAppearanceMatchesHost;

                ImGui.BeginDisabled(printAppearanceMatchesHost);
                if (SettingsTable.BeginTable("cur_print_appearance"))
                {
                    ImGuiTypes.ColoredGlyphReference glyphRef = _stateComponent.PrintAppearance;
                    SettingsTable.DrawCommonSettings(true, true, true, false, true, ref glyphRef,
                        SadRogue.Primitives.Color.White.ToVector4(),
                        SadRogue.Primitives.Color.Black.ToVector4(),
                        ((IScreenSurface)_screenObject).Font, ImGuiCore.Renderer);
                    // Something changed
                    if (glyphRef != _cursorAppearance)
                    {
                        _stateComponent.PrintAppearance.Foreground = glyphRef.Foreground.ToColor();
                        _stateComponent.PrintAppearance.Background = glyphRef.Background.ToColor();
                        _stateComponent.PrintAppearance.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(glyphRef.Mirror);
                    }

                    SettingsTable.EndTable();
                }

                ImGui.EndDisabled();

                if (ImGui.Checkbox("Print only character data?"u8, ref printOnlyCharData))
                    _stateComponent.PrintOnlyCharacterData = printOnlyCharData;
                if (ImGui.Checkbox("Disable printing automatic line feed?"u8, ref printDisableAutoLineFeed))
                    _stateComponent.DisablePrintAutomaticLineFeed = printDisableAutoLineFeed;
                if (ImGui.Checkbox("Disable word break?"u8, ref disableWordBreak))
                    _stateComponent.DisableWordBreak = disableWordBreak;
                if (ImGui.Checkbox("Is Enabled"u8, ref isEnabled))
                    _stateComponent.IsEnabled = isEnabled;
                if (ImGui.Checkbox("Is Visible"u8, ref isVisible))
                    _stateComponent.IsVisible = isVisible;
            }

            if (ImGui.CollapsingHeader("Settings"u8, ImGuiTreeNodeFlags.DefaultOpen))
            {
                // Position
                ImGui.AlignTextToFramePadding();
                ImGui.BulletText($"Position X: {_stateComponent.Position.X} Y: {_stateComponent.Position.Y}");
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10);
                if (ImGui.Button("Change"u8))
                    ImGui.OpenPopup("comp_cur_edit_position"u8);
            }

            // Popup for settings
            if (ImGuiSC.XYEditPopup("comp_cur_edit_position", ref positionX, ref positionY, "X", "Y"))
                _stateComponent.Position = (positionX, positionY);

            //else if (!ImGuiP.IsPopupOpen("comp_cur_edit_position"))
            //{
            //    _positionX = _stateComponent.Position.X;
            //    _positionY = _stateComponent.Position.Y;
            //}
        }
        ImGui.EndGroup();

    }
}
