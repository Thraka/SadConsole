#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using Newtonsoft.Json;
    using SadConsole.Effects;

    /// <summary>
    /// A <see cref="Console"/> that implements <see cref="IConsoleViewPort"/> to provide a scrolling capability.
    /// </summary>
    [JsonConverter(typeof(SerializedTypes.ScrollingConsoleJsonConverter))]
    [System.Diagnostics.DebuggerDisplay("Console (Scrolling)")]
    public class ScrollingConsole : Console, IConsoleViewPort
    {
        /// <summary>
        /// Sets the viewport without triggering <see cref="SetRenderCells"/>.
        /// </summary>
        protected Rectangle ViewPortRectangle;

        /// <summary>
        /// Sets the area of the text surface that should be rendered.
        /// </summary>
        public Rectangle ViewPort
        {
            get => ViewPortRectangle;
            set
            {
                ViewPortRectangle = value;

                if (ViewPortRectangle == default)
                {
                    ViewPortRectangle = new Rectangle(0, 0, Width, Height);
                }

                if (ViewPortRectangle.Width > Width)
                {
                    ViewPortRectangle.Width = Width;
                }

                if (ViewPortRectangle.Height > Height)
                {
                    ViewPortRectangle.Height = Height;
                }

                if (ViewPortRectangle.X < 0)
                {
                    ViewPortRectangle.X = 0;
                }

                if (ViewPortRectangle.Y < 0)
                {
                    ViewPortRectangle.Y = 0;
                }

                if (ViewPortRectangle.X + ViewPortRectangle.Width > Width)
                {
                    ViewPortRectangle.X = Width - ViewPortRectangle.Width;
                }

                if (ViewPortRectangle.Y + ViewPortRectangle.Height > Height)
                {
                    ViewPortRectangle.Y = Height - ViewPortRectangle.Height;
                }

                IsDirty = true;
                SetRenderCells();
                OnViewPortChanged();
            }
        }

        /// <summary>
        /// Creates a new scrolling console with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        public ScrollingConsole(int width, int height) : this(width, height, Global.FontDefault, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new scrolling console with the specified width, height, and font.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public ScrollingConsole(int width, int height, Font font) : this(width, height, font, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new scrolling console with the specified width, height, and a view port.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="viewPort">Initial value for the viewport if this console will scroll.</param>
        public ScrollingConsole(int width, int height, Rectangle viewPort) : this(width, height, Global.FontDefault, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new scrolling console with the specified width, height, font, and a view port.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="viewPort">Initial value for the viewport if this console will scroll.</param>
        public ScrollingConsole(int width, int height, Font font, Rectangle viewPort) : this(width, height, font, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new scrolling console with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="viewPort">Initial value for the viewport if this console will scroll.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        public ScrollingConsole(int width, int height, Font font, Rectangle viewPort, Cell[] initialCells) : base(width, height, initialCells, font, true)
        {
            ViewPortRectangle = viewPort;

            if (ViewPortRectangle == default)
            {
                ViewPortRectangle = new Rectangle(0, 0, Width, Height);
            }

            if (ViewPortRectangle.Width > Width)
            {
                ViewPortRectangle.Width = Width;
            }

            if (ViewPortRectangle.Height > Height)
            {
                ViewPortRectangle.Height = Height;
            }

            if (ViewPortRectangle.X < 0)
            {
                ViewPortRectangle.X = 0;
            }

            if (ViewPortRectangle.Y < 0)
            {
                ViewPortRectangle.Y = 0;
            }

            if (ViewPortRectangle.X + ViewPortRectangle.Width > Width)
            {
                ViewPortRectangle.X = Width - ViewPortRectangle.Width;
            }

            if (ViewPortRectangle.Y + ViewPortRectangle.Height > Height)
            {
                ViewPortRectangle.Y = Height - ViewPortRectangle.Height;
            }

            RenderRects = new Rectangle[ViewPortRectangle.Width * ViewPortRectangle.Height];
            RenderCells = new Cell[ViewPortRectangle.Width * ViewPortRectangle.Height];

            int index = 0;

            for (int y = 0; y < ViewPortRectangle.Height; y++)
            {
                for (int x = 0; x < ViewPortRectangle.Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = Cells[(y + ViewPortRectangle.Top) * Width + (x + ViewPortRectangle.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, ViewPortRectangle.Width * Font.Size.X, ViewPortRectangle.Height * Font.Size.Y);
            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        /// <summary>
        /// Calculates which cells to draw based on <see cref="ViewPort"/>.
        /// </summary>
        public override void SetRenderCells()
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
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = Cells[(y + ViewPortRectangle.Top) * Width + (x + ViewPortRectangle.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(CalculatedPosition.X, CalculatedPosition.Y, ViewPortRectangle.Width * Font.Size.X, ViewPortRectangle.Height * Font.Size.Y);

            if (LastRenderResult != null && (LastRenderResult.Bounds.Width != AbsoluteArea.Width || LastRenderResult.Bounds.Height != AbsoluteArea.Height))
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        /// <inheritdoc />
        protected override void OnCellsReset()
        {
            // Resize the viewport to make sure it's OK with the new cells
            ViewPort = ViewPortRectangle;

            base.OnCellsReset();
        }

        /// <summary>
        /// Resizes the surface to the specified width and height.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="clear">When true, resets every cell to the <see cref="CellSurface.DefaultForeground"/>, <see cref="CellSurface.DefaultBackground"/> and glyph 0.</param>
        /// <param name="viewPort">The view port to apply after resizing.</param>
        public void Resize(int width, int height, bool clear, Rectangle viewPort)
        {
            var newCells = new Cell[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
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
                    {
                        newCells[new Point(x, y).ToIndex(width)] = new Cell(DefaultForeground, DefaultBackground, 0);
                    }
                }
            }

            Cells = newCells;
            Width = width;
            Height = height;
            Effects = new EffectsManager(this);
            ViewPortRectangle = viewPort;
            OnCellsReset();
        }

        /// <summary>
        /// Called when the <see cref="ViewPort"/> property changes.
        /// </summary>
        protected virtual void OnViewPortChanged() { }

        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in rendering.</param>
        protected override void OnBeforeRender(SpriteBatch batch)
        {
            if (Cursor.IsVisible && ViewPort.Contains(Cursor.Position))
            {
                Cursor.Render(batch, Font, Font.GetRenderRect(Cursor.Position.X - ViewPort.Location.X, Cursor.Position.Y - ViewPort.Location.Y));
            }
        }

        /// <summary>
        /// Creates a new console from an existing surface.
        /// </summary>
        /// <param name="surface">The source console to convert to a scrolling console.</param>
        /// <param name="viewPort">The view port to apply to the new scrolling console.</param>
        /// <returns>A new scrolling console.</returns>
        public static ScrollingConsole FromSurface(Console surface, Rectangle viewPort) => new ScrollingConsole(surface.Width, surface.Height, surface.Font, new Rectangle(0, 0, surface.Width, surface.Height), surface.Cells);

        /// <summary>
        /// Creates a new console from an existing surface.
        /// </summary>
        /// <param name="surface">The source console to convert to a scrolling console.</param>
        /// <param name="font">The font to associate with the new console.</param>
        /// <returns>A new scrolling console.</returns>
        public static new ScrollingConsole FromSurface(CellSurface surface, Font font) => new ScrollingConsole(surface.Width, surface.Height, font, new Rectangle(0, 0, surface.Width, surface.Height), surface.Cells);
    }
}
