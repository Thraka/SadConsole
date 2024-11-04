using ImGuiNET;
using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal partial class AnimationDocument : IDocumentTools
{
    IDocumentToolsState IDocumentTools.State { get; } = new IDocumentToolsState();

    bool IDocumentTools.ShowToolsList { get; set; }

    void IDocumentTools.BuildUI(ImGuiRenderer renderer)
    {
    }

    void IDocumentTools.ToolChanged(ITool? oldTool, ITool newTool)
    {
        VisualToolLayerLower.Surface!.Clear();
        VisualToolLayerLower.Surface!.Surface.DefaultBackground = Color.Transparent;
        VisualToolLayerLower.Surface.Surface.DefaultForeground = Color.White;
        VisualToolLayerLower.Surface.Surface.DefaultGlyph = 0;
        VisualToolLayerUpper.Surface!.Clear();
        VisualToolLayerUpper.Surface!.Surface.DefaultBackground = Color.Transparent;
        VisualToolLayerUpper.Surface.Surface.DefaultForeground = Color.White;
        VisualToolLayerUpper.Surface.Surface.DefaultGlyph = 0;
    }
}
