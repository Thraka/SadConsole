using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Components;
using SadConsole.DrawCalls;
using SadConsole.FontEditing;
using SadConsole.Input;
using SadConsole.UI;

namespace SadConsole.Examples;

internal class DemoFontManipulation : IDemo
{
    public string Title => "Font manipulation";

    public string Description => "Something";

    public string CodeFile => "DemoFontManipulation.cs";

    public IScreenSurface CreateDemoScreen() =>
        new FontEditingScreen();

    public override string ToString() =>
        Title;
}

internal class FontEditingScreen : ControlsConsole
{
    Rectangle sourceFontArea;
    Rectangle targetFontArea;
    DrawCallGlyph glyphDrawCall;
    ColoredGlyph _selectedGlyph;
    Point glyphDrawPosition;

    ColoredGlyph _targetMouseGlyph;
    Point _targetMousePosition;
    bool _targetMousePositionInUse;
    SadConsole.SadFont newFont;

    public FontEditingScreen() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Cursor.IsVisible = false;
        Cursor.IsEnabled = false;

        sourceFontArea = new Rectangle(2, 1, 16, 16);
        targetFontArea = new Rectangle(Width - 18, 1, 16, 16);

        int counter = 0;
        foreach (Point position in sourceFontArea.Positions())
        {
            Surface[position].Glyph = counter;
            counter++;
        }

        Surface.DrawBox(sourceFontArea.Expand(1, 1), ShapeParameters.CreateStyledBoxThin(Color.Purple));
        Surface.DrawBox(targetFontArea.Expand(1, 1), ShapeParameters.CreateStyledBoxThin(Color.Purple));

        Colors ansiColors = Colors.CreateAnsi();

        Surface.Print(sourceFontArea.MaxExtentX + 3, sourceFontArea.Y, "Selected Glyph", ansiColors.Title);

        // Transform the top-left position we want to draw the blown up glyph to a pixel location
        glyphDrawPosition = new Point(sourceFontArea.MaxExtentX + 3, sourceFontArea.Y).SurfaceLocationToPixel(FontSize);
        _selectedGlyph = new(Color.Yellow, Color.Black, 1);

#if MONOGAME
        glyphDrawCall = new DrawCallGlyph(_selectedGlyph, new Rectangle(0, 0, Font.GlyphWidth * 10, Font.GlyphHeight * 10).ToMonoRectangle(), Font, false);
#elif SFML
        glyphDrawCall = new DrawCallGlyph(_selectedGlyph, new Rectangle(0, 0, Font.GlyphWidth * 10, Font.GlyphHeight * 10).ToIntRect(), Font, false);
#endif

        UI.Controls.ColorBar barR = new(15) { Position = (sourceFontArea.MaxExtentX + 3, sourceFontArea.Y + 9), StartingColor = Color.Black, EndingColor = new Color(255,0,0), SelectedColor = _selectedGlyph.Foreground.RedOnly() };
        UI.Controls.ColorBar barG = new(15) { Position = (sourceFontArea.MaxExtentX + 3, sourceFontArea.Y + 11), StartingColor = Color.Black, EndingColor = new Color(0, 255, 0), SelectedColor = _selectedGlyph.Foreground.GreenOnly() };
        UI.Controls.ColorBar barB = new(15) { Position = (sourceFontArea.MaxExtentX + 3, sourceFontArea.Y + 13), StartingColor = Color.Black, EndingColor = new Color(0, 0, 255), SelectedColor = _selectedGlyph.Foreground.BlueOnly() };

        //UI.Windows.ColorPickerPopup
        

        barR.ColorChanged += BarR_ColorChanged;
        barG.ColorChanged += BarG_ColorChanged;
        barB.ColorChanged += BarB_ColorChanged;

        Controls.Add(barR);
        Controls.Add(barB);
        Controls.Add(barG);


        
    }

    private void BarR_ColorChanged(object? sender, EventArgs e)
    {
        _selectedGlyph.Foreground = _selectedGlyph.Foreground.SetRed(((UI.Controls.ColorBar)sender!).SelectedColor.R);
    }
    private void BarG_ColorChanged(object? sender, EventArgs e)
    {
        _selectedGlyph.Foreground = _selectedGlyph.Foreground.SetGreen(((UI.Controls.ColorBar)sender!).SelectedColor.G);
    }
    private void BarB_ColorChanged(object? sender, EventArgs e)
    {
        _selectedGlyph.Foreground = _selectedGlyph.Foreground.SetBlue(((UI.Controls.ColorBar)sender!).SelectedColor.B);
    }

    public override void Render(TimeSpan delta)
    {
        // Copied from base class to add in custom render step
        // Components first
        IComponent[] components = ComponentsRender.ToArray();
        int count = components.Length;
        for (int i = 0; i < count; i++)
            components[i].Render(this, delta);

        // This object second
        if (Renderer != null)
        {
            Renderer.Refresh(this, ForceRendererRefresh);
            Renderer.Render(this);
            glyphDrawCall.TargetRect = new(AbsolutePosition.X + glyphDrawPosition.X, AbsolutePosition.Y + glyphDrawPosition.Y, glyphDrawCall.TargetRect.Width, glyphDrawCall.TargetRect.Height);
            Game.Instance.DrawCalls.Enqueue(glyphDrawCall);
            ForceRendererRefresh = false;
        }

        // Children last
        IScreenObject[] children = Children.ToArray();
        count = children.Length;
        for (int i = 0; i < count; i++)
            if (children[i].IsVisible)
                children[i].Render(delta);
    }

    protected override void OnMouseMove(MouseScreenObjectState state)
    {
        base.OnMouseMove(state);

        if (targetFontArea.Contains(state.CellPosition))
        {

        }
        else if (_targetMousePositionInUse)
        {
            // Mouse moved out of area, restore glyph
            
        }
    }

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
