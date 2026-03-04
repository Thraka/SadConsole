using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
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
[System.Diagnostics.DebuggerDisplay("RowFontSurface")]
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

    ///  <inheritdoc/>
    public override void Refresh(IScreenSurface screen, bool force = false)
    {
        bool backingTextureChanged = false;
        IsForced = force;

        // Update texture if something is out of size.
        if (_backingTexture == null || screen.AbsoluteArea.Width != _backingTexture.Width || screen.AbsoluteArea.Height != _backingTexture.Height)
        {
            IsForced = true;
            backingTextureChanged = true;
            _backingTexture?.Dispose();
            _backingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, BackingTextureUsageMode);
            _renderTexture?.Dispose();
            _renderTexture = new Host.GameTexture(_backingTexture);
            // Note: BackingTextureRecreated event is raised by base class
        }

        // NOTE: RowFontSurface does NOT use CachedRenderRects because each row has different dimensions
        // Destination rectangles are computed on the fly in RowFontSurfaceRenderStep

        bool composeRequested = IsForced;

        // Let everything refresh before compose.
        foreach (IRenderStep step in Steps)
            composeRequested |= step.Refresh(this, screen, backingTextureChanged, IsForced);

        // If any step (or IsForced) requests a compose, process them.
        if (composeRequested)
        {
            // Setup spritebatch for compose
            Host.Global.GraphicsDevice.SetRenderTarget(_backingTexture);
            Host.Global.GraphicsDevice.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            // Compose each step
            foreach (IRenderStep step in Steps)
                step.Composing(this, screen);

            // End sprite batch
            Host.Global.SharedSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
