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
        if (oldTool != null && oldTool is IOverlay overlay)
            Surface.SadComponents.Remove(overlay.Overlay);

        if (newTool is IOverlay)
            Surface.SadComponents.Add(((IOverlay)newTool).Overlay);
    }
}
