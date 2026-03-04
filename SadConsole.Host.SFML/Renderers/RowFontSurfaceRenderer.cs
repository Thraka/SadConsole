using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="RowFontSurface"/>.
/// </summary>
/// <remarks>
/// This renderer handles surfaces where each row can have a different font with different glyph dimensions.
/// Unlike <see cref="ScreenSurfaceRenderer"/>, this renderer computes destination rectangles on the fly
/// because cached rectangles cannot be used with variable row heights.
/// </remarks>
public class RowFontSurfaceRenderer : ScreenSurfaceRenderer
{
    /// <summary>
    /// Creates a new instance of this renderer with the default steps.
    /// </summary>
    public RowFontSurfaceRenderer() : base()
    {
        // Clear default steps and add RowFontSurface-specific steps
        Steps.Clear();
        Steps.Add(new RowFontSurfaceRenderStep());
        Steps.Add(new OutputSurfaceRenderStep());
        Steps.Add(new TintSurfaceRenderStep());
        Steps.Sort(RenderStepComparer.Instance);
    }

    /// <summary>
    /// Adds the render steps this renderer uses.
    /// </summary>
    protected override void AddDefaultSteps()
    {
        // Override to prevent base class from adding default steps
        // Steps are added in the constructor
    }
}
