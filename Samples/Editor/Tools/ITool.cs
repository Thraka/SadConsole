using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

public interface ITool
{
    string Name { get; }

    void BuildSettingsPanel(ImGuiRenderer renderer);

    void MouseOver(IScreenSurface surface, SadRogue.Primitives.Point hoveredCellPosition, ImGuiRenderer renderer);
}
