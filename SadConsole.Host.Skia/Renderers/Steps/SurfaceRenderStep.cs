using System;
using SadConsole.Host;
using SadRogue.Primitives;
using SkiaSharp;
using static SadConsole.Readers.REXPaintImage;
using Color = SadRogue.Primitives.Color;

namespace SadConsole.Renderers;

/// <summary>
/// Draws an <see cref="IScreenSurface"/> into a cached texture.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Surface")]
public class SurfaceRenderStep : IRenderStep, IRenderStepTexture
{
    private GameTexture? _cachedTexture;
    private IScreenSurface? _screenSurface;

    /// <summary>
    /// Gets the bitmap containing the rendered surface cells.
    /// </summary>
    public GameTexture? BackingTexture => _cachedTexture;

    /// <inheritdoc />
    public ITexture? CachedTexture => _cachedTexture;

    /// <inheritdoc />
    public string Name => Constants.RenderStepNames.Surface;

    /// <inheritdoc />
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Surface;

    /// <summary>
    /// Gets or sets the color applied when composing this step.
    /// </summary>
    public Color ComposeTint { get; set; } = Color.White;

    /// <inheritdoc />
    public void SetData(object data)
    {
        if (data is null)
            _screenSurface = null;
        else if (data is IScreenSurface surface)
            _screenSurface = surface;
        else
            throw new InvalidCastException($"Data must be {nameof(IScreenSurface)}");
    }

    /// <inheritdoc />
    public void Reset()
    {
        _cachedTexture?.Dispose();
        _cachedTexture = null;
    }

    /// <inheritdoc />
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        screenObject = _screenSurface ?? screenObject;
        bool textureChanged = false;

        if (_cachedTexture is null ||
            _cachedTexture.Width != screenObject.AbsoluteArea.Width ||
            _cachedTexture.Height != screenObject.AbsoluteArea.Height)
        {
            Reset();
            _cachedTexture = new GameTexture(screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height);
            textureChanged = true;
        }

        if (!textureChanged && !screenObject.IsDirty && !isForced)
            return false;

        Rasterize((ScreenSurfaceRenderer)renderer, screenObject, _cachedTexture);
        screenObject.IsDirty = false;
        return true;
    }

    /// <inheritdoc />
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        if (_cachedTexture is null)
            return;

        var skiaRenderer = (ScreenSurfaceRenderer)renderer;
        SKCanvas canvas = skiaRenderer.CompositionCanvas
            ?? throw new InvalidOperationException("The renderer is not composing.");
        SKRect destination = SKRect.Create(_cachedTexture.Width, _cachedTexture.Height);
        SkiaRendererHelpers.DrawTexture(canvas, _cachedTexture, destination, ComposeTint);
    }

    /// <inheritdoc />
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    /// <inheritdoc />
    public void Dispose() => Reset();

    private static void Rasterize(ScreenSurfaceRenderer renderer, IScreenSurface screen, GameTexture target)
    {
        SKCanvas canvas = target.BeginDraw();
        canvas.Clear(GameTexture.ToSkColor(screen.Surface.DefaultBackground));

        ICellSurface surface = screen.Surface;
        IFont font = screen.Font;
        var fontTexture = (GameTexture)font.Image;
        int rectIndex = 0;

        if (screen.Surface.Width > 15)
            System.Diagnostics.Debug.WriteLine("Refreshing main screen");

        Global.SKPaint_SrcOver.Color = new SKColor(0, 0, 0, 255);
        for (int viewY = 0; viewY < surface.View.Height; viewY++)
        {
            int surfaceY = viewY + surface.ViewPosition.Y;
            int surfaceIndex = (surfaceY * surface.Width) + surface.ViewPosition.X;

            for (int viewX = 0; viewX < surface.View.Width; viewX++)
            {
                ColoredGlyphBase cell = surface[surfaceIndex++];
                cell.IsDirty = false;
                SKRect destination = renderer.CachedRenderRects[rectIndex++];

                if (!cell.IsVisible)
                    continue;

                if (cell.Background.A != 0 && cell.Background != surface.DefaultBackground)
                {
                    //backgroundPaint.Color = GameTexture.ToSkColor(cell.Background);
                    Global.SKPaint_SrcOver.Color = GameTexture.ToSkColor(cell.Background);
                    canvas.DrawRect(destination, Global.SKPaint_SrcOver);
                }

                if (cell.Glyph != 0 && cell.Foreground.A != 0 && cell.Foreground != cell.Background)
                {
                    SkiaRendererHelpers.DrawGlyph(
                        canvas,
                        fontTexture,
                        font.GetGlyphSourceRectangle(cell.Glyph),
                        destination,
                        cell.Foreground,
                        cell.Mirror);
                }

                if (cell.Decorators is not null)
                {
                    foreach (CellDecorator decorator in cell.Decorators)
                    {
                        SkiaRendererHelpers.DrawGlyph(
                            canvas,
                            fontTexture,
                            font.GetGlyphSourceRectangle(decorator.Glyph),
                            destination,
                            decorator.Color,
                            decorator.Mirror);
                    }
                }
            }
        }

        target.EndDraw();
    }
}
