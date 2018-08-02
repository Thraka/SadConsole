using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using Newtonsoft.Json;
using SadConsole.DrawCalls;
using SadConsole.Effects;
using SadConsole.SerializedTypes;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// The base class for a text surface. Provides code for the view port and basic cell access.
    /// </summary>
    [DataContract]
    public abstract partial class SurfaceBase : ScreenObject, IEnumerable<Cell>
    {
        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> is set to true.
        /// </summary>
        public event EventHandler DirtyChanged;

        private Font _font;
        private Color _tint = Color.Transparent;
        private bool _isDirty = true;

        /// <summary>
        /// Sets the viewport without triggering <see cref="SetRenderCells"/>.
        /// </summary>
        protected Rectangle ViewPortRectangle;

        /// <summary>
        /// The renderer used to draw this surface.
        /// </summary>
        public Renderers.IRenderer Renderer { get; protected set; }

        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        public Color DefaultForeground { get; set; } = Color.White;

        /// <summary>
        /// The default background for glyphs on this surface.
        /// </summary>
        public Color DefaultBackground { get; set; } = Color.Transparent;

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        public Cell[] Cells { get; protected set; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get => Cells[y * Width + x];
            protected set => Cells[y * Width + x] = value;
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get => Cells[index];
            protected set => Cells[index] = value;
        }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                OnFontChanged();
            }
        }

        /// <summary>
        /// Pixel area of the render cells.
        /// </summary>
        public Rectangle AbsoluteArea { get; protected set; }

        /// <summary>
        /// Destination rectangles for rendering.
        /// </summary>
        public Rectangle[] RenderRects { get; protected set; }

        /// <summary>
        /// Cells that will be rendered.
        /// </summary>
        public Cell[] RenderCells { get; protected set; }

        /// <summary>
        /// A tint used in rendering.
        /// </summary>
        public Color Tint
        {
            get => _tint;
            set
            {
                _tint = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Sets the area of the text surface that should be rendered.
        /// </summary>
        public Rectangle ViewPort
        {
            get => ViewPortRectangle;
            set
            {
                ViewPortRectangle = value;

                if (ViewPortRectangle == null)
                    ViewPortRectangle = new Rectangle(0, 0, Width, Height);
                if (ViewPortRectangle.Width > Width)
                    ViewPortRectangle.Width = Width;
                if (ViewPortRectangle.Height > Height)
                    ViewPortRectangle.Height = Height;

                if (ViewPortRectangle.X < 0)
                    ViewPortRectangle.X = 0;
                if (ViewPortRectangle.Y < 0)
                    ViewPortRectangle.Y = 0;

                if (ViewPortRectangle.X + ViewPortRectangle.Width > Width)
                    ViewPortRectangle.X = Width - ViewPortRectangle.Width;
                if (ViewPortRectangle.Y + ViewPortRectangle.Height > Height)
                    ViewPortRectangle.Y = Height - ViewPortRectangle.Height;

                IsDirty = true;
                SetRenderCells();
            }
        }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                var old = _isDirty;
                _isDirty = value;
                if (value && !old)
                    DirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The last texture render pass for this surface.
        /// </summary>
        public RenderTarget2D LastRenderResult { get; set; }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="viewPort">Initial value for the <see cref="ViewPort"/> view.</param>
        protected SurfaceBase(int width, int height, Font font, Rectangle viewPort, Cell[] initialCells)
        {
            Effects = new Effects.EffectsManager(this);
            ViewPortRectangle = viewPort;
            Width = width;
            Height = height;

            if (font != null)
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, viewPort.Width * font.Size.X, viewPort.Height * font.Size.Y);

            if (initialCells == null)
                InitializeCells();

            else if (initialCells.Length != width * height)
                throw new ArgumentOutOfRangeException(nameof(initialCells), "initialCells length must equal width * height");
            else
            {
                Cells = initialCells;
                RenderCells = new Cell[Cells.Length];
                RenderRects = new Rectangle[Cells.Length];
            }

            _font = font;
            if (font != null)
                SetRenderCells();

            Renderer = new Renderers.Basic();
        }

        /// <summary>
        /// Initialization is left to the derived class.
        /// </summary>
        protected SurfaceBase() { }


        /// <summary>
        /// Disposes <see cref="LastRenderResult"/>.
        /// </summary>
        ~SurfaceBase() => LastRenderResult?.Dispose();


        /// <summary>
        /// Sets <see cref="RenderCells"/> to <see cref="Cells"/>.
        /// </summary>
        protected virtual void InitializeCells()
        {
            Cells = new Cell[Width * Height];

            for (int i = 0; i < Cells.Length; i++)
                Cells[i] = new Cell(this.DefaultForeground, this.DefaultBackground, 0);

            RenderCells = (Cell[])Cells.Clone();
            RenderRects = new Rectangle[Cells.Length];
        }

        /// <summary>
        /// Calculates which cells to draw based on <see cref="ViewPort"/>.
        /// </summary>
        public virtual void SetRenderCells()
        {
            if (RenderCells.Length != ViewPortRectangle.Width * ViewPortRectangle.Height)
            {
                RenderRects = new Rectangle[ViewPortRectangle.Width * ViewPortRectangle.Height];
                RenderCells = new Cell[ViewPortRectangle.Width * ViewPortRectangle.Height];
            }

            int index = 0;

            for (int y = 0; y < ViewPortRectangle.Height; y++)
            {
                for (int x = 0; x < ViewPortRectangle.Width; x++)
                {
                    RenderRects[index] = _font.GetRenderRect(x, y);
                    RenderCells[index] = Cells[(y + ViewPortRectangle.Top) * Width + (x + ViewPortRectangle.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, ViewPortRectangle.Width * _font.Size.X, ViewPortRectangle.Height * _font.Size.Y);

            if (LastRenderResult.Bounds.Width != AbsoluteArea.Width || LastRenderResult.Bounds.Height != AbsoluteArea.Height)
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                if (LastRenderResult != null)
                {
                    Renderer.Render(this);
                    Global.DrawCalls.Add(new DrawCallSurface(this, CalculatedPosition, false));
                }

                base.Draw(timeElapsed);
            }
        }

        /// <summary>
        /// Updates the effects of the surface.
        /// </summary>
        /// <inheritdoc />
        public override void Update(TimeSpan timeElapsed)
        {
            if (!IsPaused)
            {
                Effects.UpdateEffects(timeElapsed.TotalSeconds);

                base.Update(timeElapsed);
            }
        }

        /// <summary>
        /// Called when the <see cref="Font"/> property changes.
        /// </summary>
        protected virtual void OnFontChanged() => SetRenderCells();

        /// <summary>
        /// Remaps the cells of this surface to a view of the <paramref name="surface"/>.
        /// </summary>
        /// <typeparam name="T">The surface type.</typeparam>
        /// <param name="view">A view rectangle of the target surface.</param>
        /// <param name="surface">The target surface to map cells from.</param>
        public void SetViewFromSurface<T>(Rectangle view, in T surface) where T : SurfaceBase
        {
            //TODO Should SetViewFromSurface just go back to being a SurfaceView (or BasicView) type?
            if (!new Rectangle(0, 0, surface.Width, surface.Height).Contains(view))
                throw new ArgumentOutOfRangeException(nameof(view), "The view is outside the bounds of the surface.");

            Width = view.Width;
            Height = view.Height;
            Cells = new Cell[view.Width * view.Height];
            ViewPortRectangle = new Rectangle(0, 0, view.Width, view.Height);

            var index = 0;

            for (var y = 0; y < view.Height; y++)
            {
                for (var x = 0; x < view.Width; x++)
                {
                    Cells[index] = surface.Cells[(y + view.Top) * surface.Width + (x + view.Left)];
                    index++;
                }
            }

            Font = surface.Font;
        }

        /// <summary>
        /// Resizes the surface to the specified width and height.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="clear">When true, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
        public void Resize(int width, int height, bool clear, Rectangle viewPort = default)
        {
            if (viewPort == default)
                viewPort = new Rectangle(0, 0, width, height);

            var newCells = new Cell[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (IsValidCell(x, y))
                    {
                        newCells[new Point(x, y).ToIndex(width)] = this[x, y];

                        if (clear)
                        {
                            newCells[new Point(x, y).ToIndex(width)].Foreground = DefaultForeground;
                            newCells[new Point(x, y).ToIndex(width)].Background = DefaultBackground;
                            newCells[new Point(x, y).ToIndex(width)].Glyph = 0;
                            newCells[new Point(x, y).ToIndex(width)].ClearState();
                        }
                    }
                    else
                        newCells[new Point(x, y).ToIndex(width)] = new Cell(DefaultForeground, DefaultBackground, 0);
                }
            }

            Cells = newCells;
            Width = width;
            Height = height;
            ViewPortRectangle = viewPort;
            RenderRects = new Rectangle[Cells.Length];
            RenderCells = new Cell[Cells.Length];
            SetRenderCells();
            Effects = new EffectsManager(this);
        }

        /// <summary>
        /// Returns a new surface instance from the current instance based on the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">An area of the surface to create a view of.</param>
        /// <returns>A new surface</returns>
        public SurfaceBase GetViewSurface(Rectangle view)
        {
            return new Basic(view.Width, view.Height, Font, new Rectangle(0, 0, view.Width, view.Height), GetCells(view));
        }

        //TODO Should ChangeCells exist?
        //public void ChangeCells(Surfaces.SurfaceBase surface)
        //{
        //    Cells = surface.Cells;
        //    RenderCells = surface.RenderCells;
        //    Tint = surface.Tint;
        //    ViewPortRectangle = surface.ViewPort;
        //    Font = surface.Font;
        //}

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        public IEnumerator<Cell> GetEnumerator() => ((IEnumerable<Cell>)Cells).GetEnumerator();

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Cells.GetEnumerator();
    }
}
