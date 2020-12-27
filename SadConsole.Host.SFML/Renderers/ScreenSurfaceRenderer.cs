using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="IScreenSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class ScreenSurfaceRenderer : IRenderer
    {
        private Host.GameTexture _renderTexture;

        /// <summary>
        /// Quick access to backing texture.
        /// </summary>
        public RenderTexture _backingTexture;

        /// <summary>
        /// <see langword="true"/> when the renderer should create a draw call for <see cref="Output"/>.
        /// </summary>
        public bool DoOutputRender { get; set; } = true;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public ITexture Output => _renderTexture;

        /// <summary>
        /// The screen this renderer is attached to.
        /// </summary>
        protected IScreenSurface _screen { get; set; }

        /// <summary>
        /// Color used with drawing the texture to the screen. Let's a surface become transparent.
        /// </summary>
        public Color _finalDrawColor = SadRogue.Primitives.Color.White.ToSFMLColor();

        /// <summary>
        /// Render steps to process.
        /// </summary>
        protected List<IRenderStep> RenderSteps = new List<IRenderStep>();

        /// <summary>
        /// The blend state used by this renderer.
        /// </summary>
        public BlendMode SFMLBlendState { get; set; } = SadConsole.Host.Settings.SFMLSurfaceBlendMode;

        /// <summary>
        /// A 0 to 255 value represening how transparent the surface is when it's drawn to the screen. 255 represents full visibility.
        /// </summary>
        public byte Opacity
        {
            get => _finalDrawColor.A;
            set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
        }

        /// <inheritdoc/>
        public bool IsForced { get; set; }

        /// <summary>
        /// Cached set of rectangles used in rendering each cell.
        /// </summary>
        public IntRect[] CachedRenderRects;

        /// <summary>
        /// Creates the renderer with the <see cref="SurfaceRenderStep"/> step.
        /// </summary>
        public ScreenSurfaceRenderer()
        {
            AddRenderStep(new SurfaceRenderStep());
            AddRenderStep(new OutputSurfaceRenderStep());
        }

        ///  <inheritdoc/>
        public virtual void SetSurface(IScreenSurface screen)
        {
            _screen = screen;

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].OnSurfaceChanged(this, screen);
        }

        ///  <inheritdoc/>
        public virtual void Refresh(bool force = false)
        {
            bool backingTextureChanged = false;
            IsForced = force;

            // Update texture if something is out of size.
            if (_backingTexture == null || _screen.AbsoluteArea.Width != (int)_backingTexture.Size.X || _screen.AbsoluteArea.Height != (int)_backingTexture.Size.Y)
            {
                IsForced = true;
                _backingTexture?.Dispose();
                _backingTexture = new RenderTexture((uint)_screen.AbsoluteArea.Width, (uint)_screen.AbsoluteArea.Height);
                _renderTexture?.Dispose();
                _renderTexture = new Host.GameTexture(_backingTexture.Texture);
            }

            // Update cached drawing rectangles if something is out of size.
            if (CachedRenderRects == null || CachedRenderRects.Length != _screen.Surface.View.Width * _screen.Surface.View.Height || CachedRenderRects[0].Width != _screen.FontSize.X || CachedRenderRects[0].Height != _screen.FontSize.Y)
            {
                CachedRenderRects = new IntRect[_screen.Surface.View.Width * _screen.Surface.View.Height];

                for (int i = 0; i < CachedRenderRects.Length; i++)
                {
                    var position = Point.FromIndex(i, _screen.Surface.View.Width);
                    CachedRenderRects[i] = _screen.Font.GetRenderRect(position.X, position.Y, _screen.FontSize).ToIntRect();
                }

                IsForced = true;
            }

            bool composeRequested = IsForced;

            // Let everything refresh before compose.
            for (int i = 0; i < RenderSteps.Count; i++)
                composeRequested |= RenderSteps[i].Refresh(this, backingTextureChanged, IsForced);

            // If any step (or IsForced) requests a compose, process them.
            if (composeRequested)
            {
                // Setup spritebatch for compose
                _backingTexture.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Reset(_backingTexture, SFMLBlendState, Transform.Identity);

                // Compose each step
                for (int i = 0; i < RenderSteps.Count; i++)
                    RenderSteps[i].Composing();

                // End sprite batch
                Host.Global.SharedSpriteBatch.End();
                _backingTexture.Display();
            }
        }

        ///  <inheritdoc/>
        public virtual void Render()
        {
            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].Render();
        }

        /// <inheritdoc/>
        public void AddRenderStep(IRenderStep step)
        {
            if (RenderSteps.Contains(step)) throw new Exception("Render step has already been added to renderer");

            RenderSteps.Add(step);
            RenderSteps.Sort(CompareStep);

            step.OnAdded(this, _screen);
        }

        /// <inheritdoc/>
        public void RemoveRenderStep(IRenderStep step)
        {
            RenderSteps.Remove(step);
            step.OnRemoved(this, _screen);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IRenderStep> GetRenderSteps() =>
            RenderSteps.AsReadOnly();

        private static int CompareStep(IRenderStep left, IRenderStep right)
        {
            if (left.SortOrder > right.SortOrder)
                return 1;

            if (left.SortOrder < right.SortOrder)
                return -1;

            return 0;
        }


        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var item in RenderSteps)
                    item.Dispose();

                disposedValue = true;
            }
        }

        ~ScreenSurfaceRenderer() =>
            Dispose(false);

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
