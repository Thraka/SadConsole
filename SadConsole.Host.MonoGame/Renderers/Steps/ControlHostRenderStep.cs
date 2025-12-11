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

            BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
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
    private void ProcessContainer(UI.Controls.IContainer container, ScreenSurfaceRenderer renderer, IScreenSurface screenObject)
    {
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
    protected void RenderControlCells(SadConsole.UI.Controls.ControlBase control, ScreenSurfaceRenderer renderer, IFont font, SadRogue.Primitives.Point fontSize, SadRectangle parentViewRect)
    {
        font = control.AlternateFont ?? font;

        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;

        // Iterate through the control's viewport
        for (int controlViewY = 0; controlViewY < control.Surface.View.Height; controlViewY++)
        {
            for (int controlViewX = 0; controlViewX < control.Surface.View.Width; controlViewX++)
            {
                // Calculate the actual surface position within the control
                int controlSurfaceX = controlViewX + control.Surface.ViewPosition.X;
                int controlSurfaceY = controlViewY + control.Surface.ViewPosition.Y;

                // Get the cell index in the control's surface
                int cellIndex = controlSurfaceY * control.Surface.Width + controlSurfaceX;
                ColoredGlyphBase cell = control.Surface[cellIndex];
                cell.IsDirty = false;

                if (!cell.IsVisible) continue;

                // Calculate where this control cell appears in parent surface coordinates
                // control.AbsolutePosition is in parent surface coordinates
                // controlViewX/Y represent the offset from the control's position
                SadRogue.Primitives.Point positionInParentSurface = control.AbsolutePosition + (controlViewX, controlViewY);

                // Check if this position is within the parent's view rectangle
                if (!parentViewRect.Contains(positionInParentSurface)) continue;

                // Calculate the position within the parent's view (0-based)
                SadRogue.Primitives.Point positionInParentView = positionInParentSurface - parentViewRect.Position;

                // Get the render rectangle from the cached array
                int renderRectIndex = positionInParentView.ToIndex(parentViewRect.Width);
                XnaRectangle renderRect = renderer.CachedRenderRects[renderRectIndex];

                if (cell.Background != SadRogue.Primitives.Color.Transparent)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                if (cell.Glyph != 0 && cell.Foreground != SadRogue.Primitives.Color.Transparent)
                    Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                if (cell.Decorators != null)
                    for (int d = 0; d < cell.Decorators.Count; d++)
                        if (cell.Decorators[d].Color != SadRogue.Primitives.Color.Transparent)
                            Host.Global.SharedSpriteBatch.Draw(fontImage, renderRect, font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(), cell.Decorators[d].Color.ToMonoColor(), 0f, Vector2.Zero, cell.Decorators[d].Mirror.ToMonoGame(), 0.5f);
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
