using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal partial class SurfaceDocument : IDocumentTools
{
    IDocumentToolsState IDocumentTools.State { get; } = new IDocumentToolsState();

    bool IDocumentTools.ShowToolsList { get; set; }

    void IDocumentTools.BuildUI(ImGuiRenderer renderer)
    {
        
    }

    void IDocumentTools.ToolChanged(ITool? oldTool, ITool newTool)
    {
        if (oldTool != null)
        {
            if (oldTool is IOverlay)
                Surface.SadComponents.Remove(((IOverlay)oldTool).Overlay);
                
        }

        if (newTool is IOverlay)
            Surface.SadComponents.Add(((IOverlay)newTool).Overlay);
    }
}
