using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRectangle = SadRogue.Primitives.Rectangle;
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
    public RenderTarget2D BackingTexture { get; private set; }

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
        BackingTexture?.Dispose();
        BackingTexture = null;
        _cachedTexture?.Dispose();
        _cachedTexture = null;
        _controlsHost = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        bool result = false;

        // Update texture if something is out of size.
        if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
        {
            BackingTexture?.Dispose();
            _cachedTexture?.Dispose();

            BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            _cachedTexture = new Host.GameTexture(BackingTexture);
            result = true;
        }

        if (result || _controlsHost.IsDirty || isForced)
        {
            Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            Host.Global.GraphicsDevice.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, ((IRendererMonoGame)renderer).MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            ProcessContainer(_controlsHost, ((ScreenSurfaceRenderer)renderer), screenObject);

            Host.Global.SharedSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);

            result = true;
            _controlsHost.IsDirty = false;
        }

        return result;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, Color.White);
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
    protected void ProcessContainer(UI.Controls.IContainer container, ScreenSurfaceRenderer renderer, IScreenSurface screenObject)
    {
        UI.Controls.ControlBase control;

        /*
         * Temp code to clip drawing controls to the containers region. Prevents controls bleeding outside their container.
         * However, this code is useless until the mouse input is updated to account for the container. So probably
         * IContainer needs to be improved to cache this information some how. Then the control's input can query its
         * container to interset control.MouseBounds.

        SadRectangle clipRect;

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
            control = container[i];

            if (!control.IsVisible) continue;
            RenderControlCells(control, renderer, screenObject.Font, screenObject.FontSize, screenObject.Surface.View);

            if (control is UI.Controls.IContainer childContainer)
                ProcessContainer(childContainer, renderer, screenObject);
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
    protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, ScreenSurfaceRenderer renderer, IFont font, SadRogue.Primitives.Point fontSize, SadRectangle parentViewRect)
    {
        font = control.AlternateFont ?? font;

        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
        ColoredGlyph cell;

        for (int y = 0; y < control.Surface.View.Height; y++)
        {
            int i = ((y + control.Surface.ViewPosition.Y) * control.Surface.Width) + control.Surface.ViewPosition.X;

            for (int x = 0; x < control.Surface.View.Width; x++)
            {
                cell = control.Surface[i];
                cell.IsDirty = false;

                if (cell.IsVisible)
                {
                    SadRogue.Primitives.Point cellRenderPosition = control.AbsolutePosition + (x, y);

                    if (!parentViewRect.Contains(cellRenderPosition)) continue;
                    //if (!parentViewRect.Contains(cellRenderPosition) || !clipRect.Contains(cellRenderPosition)) continue;

                    XnaRectangle renderRect = renderer.CachedRenderRects[(cellRenderPosition - parentViewRect.Position).ToIndex(parentViewRect.Width)];

                    if (cell.Background != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                    if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                    for (int d = 0; d < cell.Decorators.Length; d++)
                        if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
                }

                i++;
            }
        }
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
    protected void Dispose(bool disposing)
    {
        Reset();
    }

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
