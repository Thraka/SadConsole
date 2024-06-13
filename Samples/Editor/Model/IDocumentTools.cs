using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal interface IDocumentTools
{
    IDocumentToolsState State { get; }

    bool ShowToolsList { get; set; }

    void BuildUI(ImGuiRenderer renderer);

    void ToolChanged(ITool? oldTool, ITool newTool);
}

public class IDocumentToolsState
{
    public int SelectedToolIndex;

    public string[] ToolNames;
    public Tools.ITool[] ToolObjects;

    public Tools.ITool? SelectedTool => SelectedToolIndex != -1 ? ToolObjects[SelectedToolIndex] : null;
}
