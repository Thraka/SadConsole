using System;
using System.Numerics;
using Raylib_cs;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;
using Rectangle = SadRogue.Primitives.Rectangle;
using HostColor = Raylib_cs.Color;
using HostRectangle = Raylib_cs.Rectangle;
using SadConsole.UI.Controls;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="SadConsole.UI.ControlHost"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Control host")]
public class ControlHostRenderStep : IRenderStep, IRenderStepTexture
{
    private SadConsole.UI.ControlHost _controlsHost;
    private Host.GameTexture _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.ControlHost;

    /// <summary>
    /// The cached texture of the drawn surface.
    /// </summary>
    public RenderTexture2D BackingTexture { get; private set; }

    /// <inheritdoc/>
    public ITexture CachedTexture => _cachedTexture;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.ControlHost;

    /// <summary>
    /// Sets the <see cref="UI.ControlHost"/>.
    /// </summary>
    /// <param name="data">A <see cref="UI.ControlHost"/> object.</param>
    public void SetData(object data)
    {
        if (data is UI.ControlHost host)
            _controlsHost = host;
        else
            throw new Exception($"{nameof(ControlHostRenderStep)} must have a {nameof(UI.ControlHost)} passed to the {nameof(SetData)} method");
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        if (Raylib.IsRenderTextureValid(BackingTexture))
            Raylib.UnloadRenderTexture(BackingTexture);

        _cachedTexture?.Dispose();
        _cachedTexture = null;
        _controlsHost = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool result = false;

        // Update texture if something is out of size.
        if (backingTextureChanged || !Raylib.IsRenderTextureValid(BackingTexture) || screenObject.AbsoluteArea.Width != (int)BackingTexture.Texture.Width || screenObject.AbsoluteArea.Height != (int)BackingTexture.Texture.Height)
        {
            if (Raylib.IsRenderTextureValid(BackingTexture))
                Raylib.UnloadRenderTexture(BackingTexture);

            BackingTexture = Raylib.LoadRenderTexture(screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height);
            _cachedTexture?.Dispose();
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            result = true;
        }

        if (result || _controlsHost.IsDirty || isForced)
        {
            Raylib.BeginTextureMode(BackingTexture);
            Raylib.ClearBackground(Color.Transparent.ToHostColor());
            Raylib.BeginBlendMode(((ScreenSurfaceRenderer)renderer).BlendState);

            ProcessContainer(_controlsHost, ((ScreenSurfaceRenderer)renderer), screenObject);

            Raylib.EndBlendMode();
            Raylib.EndTextureMode();

            result = true;
            _controlsHost.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Raylib.DrawTexture(BackingTexture.Texture, 0, 0, HostColor.White);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
    }

    /// <summary>
    /// Processes a container from the control host.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="renderer">The renderer used with this step.</param>
    /// <param name="screenObject">The screen surface with font information.</param>
    private void ProcessContainer(UI.Controls.IContainer container, ScreenSurfaceRenderer renderer, IScreenSurface screenObject)
    {
        /*
         * Temp code to clip drawing controls to the containers region. Prevents controls bleeding outside their container.
         * However, this code is useless until the mouse input is updated to account for the container. So probably
         * IContainer needs to be improved to cache this information some how. Then the control's input can query its
         * container to interset control.MouseBounds.

        Rectangle clipRect;

        if (container is ControlBase containerAsControl)
        {
            SadRogue.Primitives.Point position = container.AbsolutePosition + containerAsControl.Surface.View.Position;
            SadRogue.Primitives.Point size = containerAsControl.Surface.Area.Size;
            clipRect = new(position.X, position.Y, size.X, size.Y);
        }
        else
            clipRect = screenObject.Surface.View;

        // clipRect would be passed to RenderControlCells
        */

        for (int i = 0; i < container.Count; i++)
        {
            ControlBase control = container[i];

            if (!control.IsVisible)
                continue;

            RenderControlCells(control, renderer, screenObject.Font, screenObject.FontSize, screenObject.Surface.View);

            if (control is UI.Controls.IContainer childContainer)
                ProcessContainer(childContainer, renderer, screenObject);

            control.IsDirty = false;
        }
    }

    /// <summary>
    /// Renders the cells of a control.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="renderer">The renderer used with this step.</param>
    /// <param name="font">The font to render the cells with.</param>
    /// <param name="fontSize">The size of a cell in pixels.</param>
    /// <param name="parentViewRect">The view of the parent to cull cells from.</param>
    protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, ScreenSurfaceRenderer renderer, IFont font, SadRogue.Primitives.Point fontSize, Rectangle parentViewRect)
    {
        font = control.AlternateFont ?? font;

        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
        ColoredGlyphBase cell;

        for (int y = 0; y < control.Surface.View.Height; y++)
        {
            int i = ((y + control.Surface.ViewPosition.Y) * control.Surface.Width) + control.Surface.ViewPosition.X;

            for (int x = 0; x < control.Surface.View.Width; x++)
            {
                cell = control.Surface[i];
                cell.IsDirty = false;

                if (cell.IsVisible)
                {
                    Point cellRenderPosition = control.AbsolutePosition + (x, y);

                    if (!parentViewRect.Contains(cellRenderPosition)) continue;
                    //if (!parentViewRect.Contains(cellRenderPosition) || !clipRect.Contains(cellRenderPosition)) continue;

                    HostRectangle renderRect = renderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(parentViewRect.Width)];

                    if (cell.Background != Color.Transparent)
                        Raylib.DrawTexturePro(fontImage, font.SolidGlyphRectangle.ToHostRectangle(), renderRect, Vector2.Zero, 0f, cell.Background.ToHostColor());

                    if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent && cell.Foreground != cell.Background)
                        Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Glyph).ToHostRectangle(cell.Mirror), renderRect, Vector2.Zero, 0f, cell.Foreground.ToHostColor());

                    if (cell.Decorators != null)
                        for (int d = 0; d < cell.Decorators.Count; d++)
                            if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                                Raylib.DrawTexturePro(fontImage, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToHostRectangle(cell.Decorators[d].Mirror), renderRect, Vector2.Zero, 0f, cell.Decorators[d].Color.ToHostColor());
                }

                i++;
            }
        }
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
    protected void Dispose(bool disposing) =>
        Reset();

    ///  <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes the object for collection.
    /// </summary>
    ~ControlHostRenderStep() =>
        Dispose(false);
}
