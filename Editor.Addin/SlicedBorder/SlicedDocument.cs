using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Addin.SlicedBorder;

public partial class SlicedDocument : Document
{
    public global::SadConsole.SlicedBorder Border { get; set; }

    public SlicedDocument(CellSurface surface, global::SadConsole.SlicedBorder border)
    {
        Border = border;

        ScreenSurface screenSurface = new(surface);
        EditingSurface = screenSurface;
        Resync();
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers()
    {
        throw new NotImplementedException();
    }
}
