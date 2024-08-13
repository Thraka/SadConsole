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
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().VisualDocument;

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
    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        if (_isWriting)
        {
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnDeselected();
                document.VisualToolLayerUpper.Clear();
            }
        }
        else
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                document.VisualDocument.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            }
        }

        if (!isActive) return;

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            _isWriting = true;
            _cursorPosition = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;
            document.VisualDocument.Surface.Copy(document.VisualDocument.Surface.View, document.VisualToolLayerUpper, 0, 0);
            if (_cursor is null)
            {
                _cursor = new() { IsVisible = true, IsEnabled = true };
                document.VisualToolContainer.SadComponents.Add(_cursor);
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
            doc.VisualDocument.SadComponents.Remove(_cursor);
            _cursor = null;
        }

        doc.Options.DisableScrolling = false;
    }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer)
    {
        if (_isWriting && _cursor != null)
        {
            _keyboard.Update(Game.Instance.UpdateFrameDelta);
            ((Components.IComponent)_cursor).ProcessKeyboard(document.VisualDocument, _keyboard, out bool handled);
        }
    }
}
