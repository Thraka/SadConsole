using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SadConsole.Components;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An object that renders a <see cref="CellSurface"/>.
    /// </summary>
    public partial class ScreenSurface : CellSurface, IDisposable, IScreenSurface
    {
        private Font _font;
        private Point _fontSize;
        private Renderers.IRenderer _renderer;
        private Color _tint = Color.Transparent;
        private bool _usePixelPositioning;

        /// <inheritdoc/>
        public bool ForceRendererRefresh { get; set; }

        /// <inheritdoc/>
        public Renderers.IRenderer Renderer
        {
            get => _renderer;
            set
            {
                _renderer = value;
                IsDirty = true;
            }
        }


        CellSurface IScreenSurface.Surface => this;

        /// <inheritdoc/>
        public Font Font
        {
            get => _font;
            set
            {
                if (_font == value) return;

                _font = value;
                FontSize = _font.GetFontSize(Font.Sizes.One);
                IsDirty = true;
            }
        }

        /// <inheritdoc/>
        public Point FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize == value) return;

                _fontSize = value;
                IsDirty = true;
            }
        }

        /// <inheritdoc/>
        public Color Tint
        {
            get => _tint;
            set
            {
                _tint = value;
                IsDirty = true;
            }
        }

        /// <inheritdoc/>
        public Rectangle AbsoluteArea => new Rectangle(AbsolutePosition.X, AbsolutePosition.Y, View.Width * FontSize.X, View.Height * FontSize.Y);

        /// <inheritdoc/>
        public bool UsePixelPositioning
        {
            get => _usePixelPositioning;
            set
            {
                _usePixelPositioning = value;
                IsDirty = true;
                UpdateAbsolutePosition();
            }
        }

        /// <inheritdoc/>
        public int WidthPixels => View.Width * FontSize.X;


        /// <inheritdoc/>
        public int HeightPixels => View.Height * FontSize.Y;

        /// <summary>
        /// Creates a new screen object that can render a surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        public ScreenSurface(int width, int height) : this(width, height, width, height, null)
        {

        }

        /// <summary>
        /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        /// <param name="initialCells">The initial cells to seed the surface.</param>
        public ScreenSurface(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells)
        {
            
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the surface in cells.</param>
        /// <param name="height">The visible height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        public ScreenSurface(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null)
        {

        }

        /// <summary>
        /// Creates a new screen object using the specified surface's cells.
        /// </summary>
        /// <param name="surface">The surface.</param>
        public ScreenSurface(CellSurface surface) : this(surface.View.Width, surface.View.Height, surface.BufferWidth, surface.BufferHeight, surface.Cells)
        {

        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public ScreenSurface(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells): base (width, height, bufferWidth, bufferHeight, initialCells)
        {
            Font = Global.DefaultFont;
            FontSize = Font?.GetFontSize(Global.DefaultFontSize) ?? new Point(1, 1);
            Renderer = GameHost.Instance.GetDefaultRenderer(this);
            Components = new ObservableCollection<IComponent>();
            ComponentsUpdate = new List<IComponent>();
            ComponentsDraw = new List<IComponent>();
            ComponentsKeyboard = new List<IComponent>();
            ComponentsMouse = new List<IComponent>();
            Components.CollectionChanged += Components_CollectionChanged;
            Children = new ScreenObjectCollection(this);
        }

        /// <inheritdoc />
        public virtual void UpdateAbsolutePosition()
        {
            if (UsePixelPositioning)
                AbsolutePosition = Position + (Parent?.AbsolutePosition ?? new Point(0, 0));
            else
                AbsolutePosition = (FontSize * Position) + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <summary>
        /// Draws all <see cref="Components"/> and <see cref="Children"/>.
        /// </summary>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        public virtual void Draw()
        {
            if (!IsVisible) return;

            if (_renderer != null)
            {
                _renderer.Refresh(this, ForceRendererRefresh);
                _renderer.Render(this);
                ForceRendererRefresh = false;
            }

            foreach (IComponent component in ComponentsDraw.ToArray())
                component.Draw(this);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Draw();
        }

        /// <summary>
        /// Updates all <see cref="Components"/> and <see cref="Children"/>.
        /// </summary>
        /// <remarks>Only processes if <see cref="IsEnabled"/> is <see langword="true"/>.</remarks>
        public virtual void Update()
        {
            if (!IsEnabled) return;

            Effects.UpdateEffects(Global.UpdateFrameDelta.TotalSeconds);

            foreach (IComponent component in ComponentsUpdate.ToArray())
                component.Update(this);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Update();
        }

        /// <summary>
        /// Called when the <see cref="Font"/> or <see cref="FontSize"/> property changes.
        /// </summary>
        /// <param name="oldFont">The font prior to the change.</param>
        /// <param name="oldFontSize">The font size prior to the change.</param>
        protected virtual void OnFontChanged(Font oldFont, Point oldFontSize) { }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                _renderer?.Dispose();
                _renderer = null;

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes <see cref="Renderer"/>.
        /// </summary>
        ~ScreenSurface() =>
            Dispose(false);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
