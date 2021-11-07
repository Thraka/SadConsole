using SadRogue.Primitives.GridViews;
using Diag = System.Diagnostics;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        private void PrintSurfaceGlyphs(ICellSurface surface, string header, int fieldSize = 2)
        {
            Diag.Debug.WriteLine(header);
            Diag.Debug.WriteLine(surface.ExtendToString(fieldSize, elementStringifier: g => g.Glyph.ToString()));
        }
    }
}
