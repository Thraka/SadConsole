using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a <see cref="UI.ControlHost"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Control host")]
public class ControlHostRenderStep : IRenderStep, IRenderStepTexture
{
    private UI.ControlHost _controlsHost;
    private GameTexture _cachedTexture;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.ControlHost;

    /// <summary>
    /// The cached texture of the drawn controls layer.
    /// </summary>
    public RenderTexture BackingTexture { get; private set; }

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
        if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != (int)BackingTexture.Size.X || screenObject.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
        {
            BackingTexture?.Dispose();
            _cachedTexture?.Dispose();

            BackingTexture = new RenderTexture((uint)screenObject.AbsoluteArea.Width, (uint)screenObject.AbsoluteArea.Height);
            _cachedTexture = new Host.GameTexture(BackingTexture.Texture);
            result = true;
        }

        // Redraw is needed
        if (result || _controlsHost.IsDirty || isForced)
        {
            BackingTexture.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Reset(BackingTexture, ((ScreenSurfaceRenderer)renderer).SFMLBlendState, Transform.Identity);

            ProcessContainer(_controlsHost, ((ScreenSurfaceRenderer)renderer), screenObject);

            Host.Global.SharedSpriteBatch.End();
            BackingTexture.Display();

            result = true;
            _controlsHost.IsDirty = false;
        }

        return result;
    }

    /// <summary>
    /// Processes a container from the control host.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="renderer">The renderer used with this step.</param>
    /// <param name="screenObject">The screen surface with font information.</param>
    private void ProcessContainer(UI.Controls.IContainer container, ScreenSurfaceRenderer renderer, IScreenSurface screenObject)
    {
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

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        IntRect outputArea = new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y);
        Host.Global.SharedSpriteBatch.DrawQuad(outputArea, outputArea, Color.White, BackingTexture.Texture);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
    }

    /// <summary>
    /// Renders the cells of a control.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="renderer">The renderer used with this step.</param>
    /// <param name="font">The font to render the cells with.</param>
    /// <param name="fontSize">The size of a cell in pixels.</param>
    /// <param name="parentViewRect">The view of the parent to cull cells from.</param>
    protected void RenderControlCells(UI.Controls.ControlBase control, ScreenSurfaceRenderer renderer, IFont font, Point fontSize, Rectangle parentViewRect)
    {
        font = control.AlternateFont ?? font;

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
                IntRect renderRect = renderer.CachedRenderRects[renderRectIndex];

                // Draw the cell
                Host.Global.SharedSpriteBatch.DrawCell(cell, renderRect, true, font);
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
