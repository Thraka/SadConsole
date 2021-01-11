using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using SadConsole.Components;
using SadConsole.Renderers;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace SadConsole
{
    /// <summary>
    /// An object that renders a <see cref="ICellSurface"/>.
    /// </summary>
    [DataContract]
    public partial class ScreenSurface : ScreenObject, IDisposable, IScreenSurface
    {
        [DataMember(Name = "Font")]
        private Font _font;
        [DataMember(Name = "FontSize")]
        private Point _fontSize;
        [DataMember(Name = "Tint")]
        private Color _tint = Color.Transparent;
        [DataMember(Name = "UsePixelPositioning")]
        private bool _usePixelPositioning;
        [DataMember(Name = "Surface")]
        private ICellSurface _surface;

        private Renderers.IRenderer _renderer;

        /// <inheritdoc/>
        public bool ForceRendererRefresh { get; set; }

        /// <inheritdoc/>
        public virtual string DefaultRendererName { get; } = Renderers.Constants.RendererNames.Default;

        /// <inheritdoc/>
        public Renderers.IRenderer Renderer
        {
            get => _renderer;
            protected set
            {
                if (_renderer == value) return;

                _renderer = value;

                OnRendererChanged();
                IsDirty = true;
            }
        }

        /// <inheritdoc/>
        public SortedSet<IRenderStep> RenderSteps { get; } = new SortedSet<IRenderStep>(new RenderStepComparer());

        /// <summary>
        /// The surface this screen object represents.
        /// </summary>
        public ICellSurface Surface
        {
            get => _surface;
            protected set
            {
                ICellSurface old = _surface;

                _surface = value ?? throw new NullReferenceException("Surface cannot be set to null.");

                if (old != null)
                    old.IsDirtyChanged -= _isDirtyChangedEventHadler;

                _surface.IsDirtyChanged += _isDirtyChangedEventHadler;

                OnSurfaceChanged(old);
            }
        }

        /// <summary>
        /// When <see langword="true"/>, indicates that the <see cref="Surface"/> needs to be redrawn; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsDirty
        {
            get => _surface.IsDirty;
            set => _surface.IsDirty = value;
        }

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
        public Rectangle AbsoluteArea => new Rectangle(AbsolutePosition.X, AbsolutePosition.Y, WidthPixels, HeightPixels);

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
        public int WidthPixels => Surface.View.Width * FontSize.X;


        /// <inheritdoc/>
        public int HeightPixels => Surface.View.Height * FontSize.Y;

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
        /// <param name="viewWidth">The visible width of the surface in cells.</param>
        /// <param name="viewHeight">The visible height of the surface in cells.</param>
        /// <param name="totalWidth">The total width of the surface in cells.</param>
        /// <param name="totalHeight">The total height of the surface in cells.</param>
        public ScreenSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight) : this(viewWidth, viewHeight, totalWidth, totalHeight, null)
        {

        }

        /// <summary>
        /// Creates a new surface from a grid view. The cells between the this object and the grid view are shared.
        /// </summary>
        /// <param name="surface">The surface to use as the source of cells.</param>
        /// <param name="visibleWidth">Optional view width. If <c>0</c>, the view width matches the width of the surface.</param>
        /// <param name="visibleHeight">Optional view height. If <c>0</c>, the view width matches the height of the surface.</param>
        public ScreenSurface(IGridView<ColoredGlyph> surface, int visibleWidth = 0, int visibleHeight = 0): this(new CellSurface(surface, visibleWidth, visibleHeight))
        {
            
        }

        /// <summary>
        /// Creates a new screen object wrapping an existing surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="font">The font to use with the surface.</param>
        /// <param name="fontSize">The font size.</param>
        [JsonConstructor]
        public ScreenSurface(ICellSurface surface, Font font = null, Point? fontSize = null)
        {
            Surface = surface;
            Font = font ?? GameHost.Instance.DefaultFont;
            FontSize = fontSize ?? Font?.GetFontSize(GameHost.Instance.DefaultFontSize) ?? new Point(1, 1);

            // Note, we keep the hardcoded "default" renderer because it requires no setup. If a
            // derived class uses a different render, and that renderer needs the derived class
            // already configured, ready to accept the new renderer, they should call
            //      Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
            //
            Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Surface));
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Output));
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Tint));
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="viewWidth">The width of the surface in cells to show.</param>
        /// <param name="viewHeight">The height of the surface in cells to show.</param>
        /// <param name="totalWidth">The total width of the surface in cells.</param>
        /// <param name="totalHeight">The total height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public ScreenSurface(int viewWidth, int viewHeight, int totalWidth, int totalHeight, ColoredGlyph[] initialCells)
        {
            Surface = new CellSurface(viewWidth, viewHeight, totalWidth, totalHeight, initialCells);
            Font = GameHost.Instance.DefaultFont;
            FontSize = Font?.GetFontSize(GameHost.Instance.DefaultFontSize) ?? new Point(1, 1);

            // See note in other ctor.
            Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Surface));
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Output));
            RenderSteps.Add(GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Tint));
        }

        /// <inheritdoc />
        public override void UpdateAbsolutePosition()
        {
            if (UsePixelPositioning)
                AbsolutePosition = Position + (Parent?.AbsolutePosition ?? new Point(0, 0));
            else
                AbsolutePosition = (FontSize * Position) + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <summary>
        /// Draws the <see cref="Surface"/> and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last call.</param>
        /// <remarks>Only processes if <see cref="ScreenObject.IsVisible"/> is <see langword="true"/>.</remarks>
        public override void Render(TimeSpan delta)
        {
            if (!IsVisible) return;

            if (_renderer != null)
            {
                _renderer.Refresh(this, ForceRendererRefresh);
                _renderer.Render(this);
                ForceRendererRefresh = false;
            }

            foreach (IComponent component in ComponentsRender.ToArray())
                component.Render(this, delta);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Render(delta);
        }

        /// <summary>
        /// Updates the <see cref="Surface"/> effects and all <see cref="ScreenObject.SadComponents"/> and <see cref="ScreenObject.Children"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        /// <remarks>Only processes if <see cref="ScreenObject.IsEnabled"/> is <see langword="true"/>.</remarks>
        public override void Update(TimeSpan delta)
        {
            if (!IsEnabled) return;

            Surface.Effects.UpdateEffects(delta.TotalSeconds);

            foreach (IComponent component in ComponentsUpdate.ToArray())
                component.Update(this, delta);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Update(delta);
        }

        private void _isDirtyChangedEventHadler(object sender, EventArgs e) =>
            OnIsDirtyChanged();

        /// <summary>
        /// Called when the <see cref="IsDirty"/> property changes.
        /// </summary>
        protected virtual void OnIsDirtyChanged() { }

        /// <summary>
        /// Called when the <see cref="Font"/> or <see cref="FontSize"/> property changes.
        /// </summary>
        /// <param name="oldFont">The font prior to the change.</param>
        /// <param name="oldFontSize">The font size prior to the change.</param>
        protected void OnFontChanged(Font oldFont, Point oldFontSize) { }


        /// <summary>
        /// Called when the <see cref="Surface"/> property is changed.
        /// </summary>
        /// <param name="oldSurface">The previous surface.</param>
        protected void OnSurfaceChanged(ICellSurface oldSurface) { }

        /// <summary>
        /// Called when the <see cref="Renderer"/> property is changed.
        /// </summary>
        protected void OnRendererChanged() { }

        /// <summary>
        /// Returns the value "ScreenSurface".
        /// </summary>
        /// <returns>The string "ScreenSurface".</returns>
        public override string ToString() =>
            "ScreenSurface";

        /// <inheritdoc/>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
            UpdateAbsolutePosition();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
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
