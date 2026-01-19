using System.Linq.Expressions;
using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;
using SadConsole.Renderers;

namespace SadConsole.Editor.Tools;

internal class Text : ITool
{
    private bool _isTyping = false;
    private readonly Components.Cursor _cursorVisual;
    private Components.Cursor? _cursorActual;

    public string Title => "\ueb69 Text";

    public string Description =>
        """

        """;

    public Text()
    {
        _cursorVisual = new() { AutomaticallyShiftRowsUp = false };
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGuiSC.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        int glyph = SharedToolSettings.Tip.Glyph;
        ImGuiTypes.Mirror mirror = MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);

        if (SettingsTable.BeginTable("textsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
        {

            SettingsTable.DrawCommonSettings(true, true, true, false, true,
                ref foreground, document.EditingSurface.Surface.DefaultForeground.ToVector4(),
                ref background, document.EditingSurface.Surface.DefaultBackground.ToVector4(),
                ref mirror,
                ref glyph, document.EditingSurfaceFont, ImGuiCore.Renderer);

            SettingsTable.EndTable();
        }

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();
        SharedToolSettings.Tip.Mirror = MirrorConverter.ToSadConsoleMirror(mirror);

        ImGuiSC.EndGroupPanel();
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (_isTyping)
        {
            if (isHovered)
                ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

            // Cancelled
            if (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape))
            {
                ClearState(document);
            }

            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            {
                _cursorActual!.Position =
                _cursorVisual.Position = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
            }

            else
            {
                Game.Instance.Keyboard.Update(Game.Instance.UpdateFrameDelta);
                //document.VisualTool.ProcessKeyboard(Game.Instance.Keyboard);
                ((Components.IComponent)_cursorActual!).ProcessKeyboard(null!, Game.Instance.Keyboard, out bool didSomething);
                ((Components.IComponent)_cursorActual).Update(null, Game.Instance.UpdateFrameDelta);
                //document.VisualTool.Update(Game.Instance.UpdateFrameDelta);
                //document.VisualTool.IsDirty = true;
                _cursorVisual.Position = _cursorActual.Position;

                if (didSomething)
                    document.EditingSurface.IsDirty = true;
            }

            return;
        }

        if (!isHovered) return;

        ImGuiRenderer theRenderer = ImGuiCore.Renderer;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
        {
            document.VisualTool.SadComponents.Add(_cursorVisual);

            _cursorVisual.Position = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
            _cursorActual = new(document.EditingSurface.Surface.GetSubSurface(new Rectangle(document.EditingSurface.Surface.ViewPosition.X,
                                                                                            document.EditingSurface.Surface.ViewPosition.Y,
                                                                                            document.EditingSurface.Surface.ViewWidth,
                                                                                            document.EditingSurface.Surface.ViewHeight)));
            _cursorActual.PrintAppearance = SharedToolSettings.Tip;
            _cursorActual.PrintAppearanceMatchesHost = false;
            _cursorActual.AutomaticallyShiftRowsUp = false;
            _cursorActual.Position = _cursorVisual.Position;
            _isTyping = true;
            document.Options.DisableScrolling = true;
        }
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) =>
        ClearState(document);

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public void ClearState(Document document)
    {
        _isTyping = false;
        document.VisualTool.SadComponents.Remove(_cursorVisual);
        document.VisualTool.Surface.Clear();
        _cursorActual?.Dispose();
        _cursorActual = null;
        document.Options.DisableScrolling = false;
    }

    public override string ToString() =>
        Title;
}
