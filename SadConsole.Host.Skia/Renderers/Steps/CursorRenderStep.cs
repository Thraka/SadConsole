using System;
using SadConsole.Host;
using SkiaSharp;

namespace SadConsole.Renderers;

/// <summary>
/// Composes cursor components over a rendered surface.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Cursor")]
public class CursorRenderStep : IRenderStep
{
    private Components.Cursor? _cursor;

    /// <inheritdoc />
    public string Name => Constants.RenderStepNames.Cursor;

    /// <inheritdoc />
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Cursor;

    /// <inheritdoc />
    public void SetData(object data)
    {
        _cursor = data as Components.Cursor
            ?? throw new ArgumentException($"{nameof(CursorRenderStep)} must have a {nameof(Components.Cursor)} passed to {nameof(SetData)}.", nameof(data));
    }

    /// <inheritdoc />
    public void Reset() => _cursor = null;

    /// <inheritdoc />
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        foreach (Components.Cursor cursor in screenObject.GetSadComponents<Components.Cursor>())
            if (cursor.CursorRenderCellActiveState.IsDirty)
                return true;

        return false;
    }

    /// <inheritdoc />
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        var skiaRenderer = (ScreenSurfaceRenderer)renderer;
        SKCanvas canvas = skiaRenderer.CompositionCanvas
            ?? throw new InvalidOperationException("The renderer is not composing.");
        var fontTexture = (GameTexture)screenObject.Font.Image;

        foreach (Components.Cursor cursor in screenObject.GetSadComponents<Components.Cursor>())
        {
            if (!cursor.IsVisible ||
                !screenObject.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) ||
                !screenObject.Surface.View.Contains(cursor.Position))
                continue;

            int viewX = cursor.Position.X - screenObject.Surface.ViewPosition.X;
            int viewY = cursor.Position.Y - screenObject.Surface.ViewPosition.Y;
            SKRect destination = SKRect.Create(
                viewX * screenObject.FontSize.X,
                viewY * screenObject.FontSize.Y,
                screenObject.FontSize.X,
                screenObject.FontSize.Y);
            ColoredGlyphBase cell = cursor.CursorRenderCellActiveState;

            SkiaRendererHelpers.DrawGlyph(
                canvas,
                fontTexture,
                screenObject.Font.GetGlyphSourceRectangle(cell.Glyph),
                destination,
                cell.Foreground,
                cell.Mirror);
        }
    }

    /// <inheritdoc />
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    /// <inheritdoc />
    public void Dispose() => Reset();
}
