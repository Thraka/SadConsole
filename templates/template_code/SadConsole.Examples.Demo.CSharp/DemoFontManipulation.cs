using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.FontEditing;

namespace SadConsole.Examples;
internal class DemoFontManipulation
{
    /*
    public void test()
    {
        // Some surface that's 1x1 and you've composed and done at least one render pass on it
        // You must use the original font size as you'll be using it pixel-by-pixel
        SadConsole.ScreenSurface surfaceObject = new(1, 1, MyCustomFont);

        // You want transparent background for the rendering
        surfaceObject.Surface.DefaultBackground = Color.Transparent;
        surfaceObject.Surface.Clear();

        // Base cell, you always want transparent background
        surfaceObject.SetGlyph(0, 0, 22, Color.Yellow, Color.Transparent, Mirror.None, SomeDecorators...);

        // Render the surface, and you want to force a render no matter what. These flags are probably overkill
        // but I don't know how you'll be working with your objects...
        surfaceObject.ForceRendererRefresh = true;
        surfaceObject.Render(TimeSpan.Zero);

        // Find a font you want to expand with a new glyph
        SadFont font = YourFontToExpand;

        // This is the last glyph index in the font currently, Cols * Rows
        int lastGlyphIndex = font.TotalGlyphs;

        // Add a new row of glyphs to the font, so first index will be what font.TotalGlyphs was
        font.Edit_AddRows(1);


        // Get the pixel data from the custom glyph after your surface was rendered. This is where it was rendered to.
        Color[] pixels = ((SadConsole.Renderers.ScreenSurfaceRenderer)surfaceObject.Renderer).Output.GetPixels();
        Color[]? cachedPixelsUnused = null;

        // Add the custom glyph to the first glyph of the new row
        font.Edit_SetGlyph_Pixel(lastGlyphIndex, pixels, true, ref cachedPixelsUnused!);
    }
    */
}
