using System.Numerics;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Text : ITool
{
    private bool _isWriting;
    private Point _cursorPosition;
    private Cursor? _cursor;
    private Input.Keyboard _keyboard = new();

    public string Name => "Text";

    public string Description => """
        Write text to a surface.
        
        Use the left-mouse button to start writing or move the text cursor.
        
        The right-mouse button or the ESC key turns off the cursor.
        """;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGuiWidgets.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        int glyph = SharedToolSettings.Tip.Glyph;
        Mirror mirror = SharedToolSettings.Tip.Mirror;
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().Surface;

        SettingsTable.BeginTable("matchtable");

        SettingsTable.DrawColor("Foreground", "##fore", ref foreground, surface.Surface.DefaultForeground.ToVector4(), out bool rightClicked);

        if (rightClicked)
            (background, foreground) = (foreground, background);

        SettingsTable.DrawColor("Background", "##back", ref background, surface.Surface.DefaultBackground.ToVector4(), out rightClicked);

        if (rightClicked)
            (background, foreground) = (foreground, background);

        SettingsTable.EndTable();

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        if (_isWriting)
        {
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnDeselected();
            }
        }
        else
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                surface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            }
        }

        if (!isActive) return;

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            _isWriting = true;
            _cursorPosition = hoveredCellPosition - surface.Surface.ViewPosition;
            if (_cursor is null)
            {
                _cursor = new() { IsVisible = true, IsEnabled = true };
                surface.SadComponents.Add(_cursor);
            }
            _cursor.Position = _cursorPosition;
            _cursor.PrintAppearance = SharedToolSettings.Tip;
            _cursor.PrintOnlyCharacterData = false;
            _cursor.PrintAppearanceMatchesHost = false;
            ImGuiCore.State.GetOpenDocument().Options.DisableScrolling = true;
        }
    }

    public void OnSelected() { }

    public void OnDeselected()
    {
        _keyboard.Clear();
        _isWriting = false;

        Document doc = ImGuiCore.State.GetOpenDocument();

        if (_cursor != null)
        {
            doc.Surface.SadComponents.Remove(_cursor);
            _cursor = null;
        }

        doc.Options.DisableScrolling = false;
    }

    public void DocumentViewChanged() { }

    public void DrawOverDocument()
    {
        if (_isWriting && _cursor != null)
        {
            _keyboard.Update(Game.Instance.UpdateFrameDelta);
            ((Components.IComponent)_cursor).ProcessKeyboard(ImGuiCore.State.GetOpenDocument().Surface, _keyboard, out bool handled);
        }
    }
}
