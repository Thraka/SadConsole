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
        private IRenderStep _defaultRenderStep;

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
        /// Default render step for drawing the surface.
        /// </summary>
        public IRenderStep DefaultRenderStep
        {
            get => _defaultRenderStep;
            set
            {
                if (_defaultRenderStep == value) return;

                if (_defaultRenderStep != null)
                    RemoveRenderStep(_defaultRenderStep);

                if (value == null) return;

                _defaultRenderStep = value;

                AddRenderStep(value);
            }
        }

        /// <summary>
        /// Creates the renderer with the <see cref="SurfaceRenderStep"/> step.
        /// </summary>
        public ScreenSurfaceRenderer() =>
            DefaultRenderStep = new SurfaceRenderStep();

        ///  <inheritdoc/>
        public virtual void Attach(IScreenSurface screen)
        {
            _screen = screen;

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].OnSurfaceChanged(this, screen);
        }

        ///  <inheritdoc/>
        public virtual void Detatch(IScreenSurface screen)
        {
            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].OnSurfaceChanged(this, null);

            _screen = null;
        }

        ///  <inheritdoc/>
        public virtual void Render(IScreenSurface screen)
        {
            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RenderStart();

            // If tint is visible, draw it
            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToSFMLColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToIntRect(), screen.Font.SolidGlyphRectangle.ToIntRect()));

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RenderEnd();
        }

        ///  <inheritdoc/>
        public virtual void Refresh(IScreenSurface screen, bool force = false)
        {
            IsForced = force;

            // Update cached drawing rectangles if something is out of size.
            if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
            {
                CachedRenderRects = new IntRect[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < CachedRenderRects.Length; i++)
                {
                    var position = Point.FromIndex(i, screen.Surface.View.Width);
                    CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToIntRect();
                }

                IsForced = true;
            }

            for (int i = 0; i < RenderSteps.Count; i++)
                IsForced |= RenderSteps[i].RefreshPreStart();

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].Refresh();

            screen.IsDirty = false;
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
