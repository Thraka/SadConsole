using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem.Rendering;
using SadRogue.Primitives;

namespace SadConsole.Editor.Addin.SlicedBorder;

public partial class SlicedDocument : Document
{
    public SadConsole.SlicedBorder Border { get; set; }

    public SlicedDocument(SadConsole.SlicedBorder border)
    {
        Border = border;

        ScreenSurface screenSurface = new(border.Surface);
        EditingSurface = screenSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;

        // Scene draws itself with custom rendering
        Options.UseToolsWindow = true;
        Options.ToolsWindowShowToolsList = false;
        Options.DisableScrolling = false;
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        
    }

    public override void ImGuiDrawSurfaceTextureAfter(ImGuiRenderer renderer, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers()
    {
        throw new NotImplementedException();
    }
}
