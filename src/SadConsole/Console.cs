#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Newtonsoft.Json;
    using SadConsole.Components;

    /// <summary>
    /// A <see cref="CellSurface"/> that has a font and can be drawn to the screen.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Console")]
    [JsonConverter(typeof(SerializedTypes.ConsoleJsonConverter))]
    public partial class Console : CellSurface
    {
        private Console _parentScreen;
        private Point _position;
        private bool _isVisible = true;
        private bool _isPaused;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IConsoleComponent.IsUpdate"/> is <see langword="true"/>.
        /// </summary>
        protected List<IConsoleComponent> ComponentsUpdate;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IConsoleComponent.IsDraw"/> is <see langword="true"/>.
        /// </summary>
        protected List<IConsoleComponent> ComponentsDraw;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IConsoleComponent.IsMouse"/> is <see langword="true"/>.
        /// </summary>
        protected List<IConsoleComponent> ComponentsMouse;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IConsoleComponent.IsKeyboard"/> is <see langword="true"/>.
        /// </summary>
        protected List<IConsoleComponent> ComponentsKeyboard;

        /// <summary>
        /// A filtered list from <see cref="Components"/> that is not set for update, draw, mouse, or keyboard.
        /// </summary>
        protected List<IConsoleComponent> ComponentsEmpty;

        /// <summary>
        /// A collection of components processed by this console.
        /// </summary>
        public ObservableCollection<IConsoleComponent> Components { get; private set; }

        /// <summary>
        /// How the console should handle becoming active.
        /// </summary>
        public ActiveBehavior FocusedMode { get; set; }

        /// <summary>
        /// The position of the screen object.
        /// </summary>
        /// <remarks>This position has no substance.</remarks>
        public Point Position
        {
            get => _position;
            set
            {
                Point oldPosition = _position;
                _position = value;
                OnCalculateRenderPosition();
                OnPositionChanged(oldPosition);
            }
        }

        /// <summary>
        /// A position that is based on the current <see cref="Position"/> and <see cref="Parent"/> position, in pixels.
        /// </summary>
        public Point CalculatedPosition { get; protected set; }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get; set; } = false;

        /// <summary>
        /// The child objects of this instance.
        /// </summary>
        public ConsoleCollection Children { get; }

        /// <summary>
        /// The parent object that this instance is a child of.
        /// </summary>
        public Console Parent
        {
            get => _parentScreen;
            set
            {
                if (value == this)
                {
                    throw new Exception("Cannot set parent to itself.");
                }

                if (_parentScreen == value)
                {
                    return;
                }

                if (_parentScreen == null)
                {
                    _parentScreen = value;
                    _parentScreen.Children.Add(this);
                    OnCalculateRenderPosition();
                    OnParentChanged(null, _parentScreen);
                }
                else
                {
                    Console oldParent = _parentScreen;
                    _parentScreen = null;
                    oldParent.Children.Remove(this);
                    _parentScreen = value;


                    _parentScreen?.Children.Add(this);

                    OnCalculateRenderPosition();
                    OnParentChanged(oldParent, _parentScreen);
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of this object.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;

                if (!value && IsMouseOver)
                {
                    OnMouseExit(new Input.MouseConsoleState(this, Global.MouseState));
                }

                OnVisibleChanged();
            }
        }

        /// <summary>
        /// Gets or sets the paused status of this object.
        /// </summary>
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value)
                {
                    return;
                }

                _isPaused = value;
                OnPausedChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        public bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Gets or sets this console as the focused console for input.
        /// </summary>
        public bool IsFocused
        {
            get => Global.FocusedConsoles.Console == this;
            set
            {
                if (Global.FocusedConsoles.Console != null)
                {
                    if (value && Global.FocusedConsoles.Console != this)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                        {
                            Global.FocusedConsoles.Push(this);
                        }
                        else
                        {
                            Global.FocusedConsoles.Set(this);
                        }
                    }
                    else if (!value && Global.FocusedConsoles.Console == this)
                    {
                        Global.FocusedConsoles.Pop(this);
                    }
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                        {
                            Global.FocusedConsoles.Push(this);
                        }
                        else
                        {
                            Global.FocusedConsoles.Set(this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new console with the specified width and height. Uses <see cref="Global.FontDefault"/> as the font.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        public Console(int width, int height) : this(width, height, Global.FontDefault)
        {

        }

        /// <summary>
        /// Creates a new console with the specified width, height, and the cells backing the console. Uses <see cref="Global.FontDefault"/> as the font.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="cells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        public Console(int width, int height, Cell[] cells) : this(width, height, Global.FontDefault, cells)
        {

        }

        /// <summary>
        /// Creates a new console with the specified width, height, and font.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the conosle.</param>
        /// <param name="font">The font used with rendering.</param>
        public Console(int width, int height, Font font) : this(width, height, font, null)
        {

        }

        /// <summary>
        /// Creates a new console with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="cells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        public Console(int width, int height, Font font, Cell[] cells) : base(width, height, cells)
        {
            Components = new ObservableCollection<IConsoleComponent>();
            Components.CollectionChanged += Components_CollectionChanged;

            ComponentsKeyboard = new List<IConsoleComponent>();
            ComponentsDraw = new List<IConsoleComponent>();
            ComponentsUpdate = new List<IConsoleComponent>();
            ComponentsMouse = new List<IConsoleComponent>();
            ComponentsEmpty = new List<IConsoleComponent>();

            Children = new ConsoleCollection(this);
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Console();
            _font = font;

            int index = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * Font.Size.X, Height * Font.Size.Y);
            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            Cursor = new Cursor(this);
        }

        /// <summary>
        /// Creates a new console. This is a custom constructor that leaves the creation of leaves <see cref="RenderCells"/> and <see cref="LastRenderResult"/> creation up to the the child class.
        /// </summary>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="cells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="skipRenderCreation">Must be set to true.</param>
        protected Console(int width, int height, Cell[] cells, Font font, bool skipRenderCreation) : base(width, height, cells)
        {
            if (skipRenderCreation == false)
            {
                throw new Exception($"{nameof(skipRenderCreation)} must be set to true. This is a special constructor that should not be called unless you know what you're doing.");
            }

            Components = new ObservableCollection<IConsoleComponent>();
            Components.CollectionChanged += Components_CollectionChanged;

            ComponentsKeyboard = new List<IConsoleComponent>();
            ComponentsDraw = new List<IConsoleComponent>();
            ComponentsUpdate = new List<IConsoleComponent>();
            ComponentsMouse = new List<IConsoleComponent>();
            ComponentsEmpty = new List<IConsoleComponent>();

            Children = new ConsoleCollection(this);
            Renderer = new Renderers.Console();
            Cursor = new Cursor(this);
            _font = font;
        }

        internal Console() : base(1, 1)
        {
            IsCursorDisabled = true;

            Components = new ObservableCollection<IConsoleComponent>();
            Components.CollectionChanged += Components_CollectionChanged;

            ComponentsKeyboard = new List<IConsoleComponent>();
            ComponentsDraw = new List<IConsoleComponent>();
            ComponentsUpdate = new List<IConsoleComponent>();
            ComponentsMouse = new List<IConsoleComponent>();
            ComponentsEmpty = new List<IConsoleComponent>();

            Children = new ConsoleCollection(this);
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Console();
            _font = Global.FontDefault;

            int index = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * Font.Size.X, Height * Font.Size.Y);
        }

        /// <summary>
        /// Calls <see cref="SetRenderCells"/>.
        /// </summary>
        protected override void OnCellsReset() => SetRenderCells();

        /// <summary>
        /// Sets a value for <see cref="CalculatedPosition"/> based on the <see cref="Position"/> of this instance and the <see cref="Parent"/> instance.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            if (UsePixelPositioning)
            {
                CalculatedPosition = Position + (Parent?.CalculatedPosition ?? Point.Zero);
            }
            else
            {
                CalculatedPosition = Position.ConsoleLocationToPixel(Font) + (Parent?.CalculatedPosition ?? Point.Zero);
            }

            AbsoluteArea = new Rectangle(CalculatedPosition.X, CalculatedPosition.Y, AbsoluteArea.Width, AbsoluteArea.Height);

            foreach (Console child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Called when this console's focus has been lost. Hides the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus)
            {
                Cursor.IsVisible = false;
            }
        }

        /// <summary>
        /// Called when this console is focused. Shows the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        public virtual void OnFocused()
        {
            if (AutoCursorOnFocus)
            {
                Cursor.IsVisible = true;
            }
        }

        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in rendering.</param>
        protected virtual void OnBeforeRender(SpriteBatch batch)
        {
            if (Cursor.IsVisible && IsValidCell(Cursor.Position.X, Cursor.Position.Y))
            {
                Cursor.Render(batch, Font, Font.GetRenderRect(Cursor.Position.X, Cursor.Position.Y));
            }
        }

        /// <summary>
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentChanged(Console oldParent, Console newParent) { }

        /// <summary>
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }

        /// <summary>
        /// Called when the visibility of the object changes.
        /// </summary>
        protected virtual void OnVisibleChanged() { }

        /// <summary>
        /// Called when the paused status of the object changes.
        /// </summary>
        protected virtual void OnPausedChanged() { }

        /// <summary>
        /// Gets components of the specified types.
        /// </summary>
        /// <typeparam name="TComponent">THe component to find</typeparam>
        /// <returns>The components found.</returns>
        public IEnumerable<IConsoleComponent> GetComponents<TComponent>()
            where TComponent : IConsoleComponent
        {
            foreach (IConsoleComponent component in Components)
            {
                if (component is TComponent)
                {
                    yield return component;
                }
            }
        }

        /// <summary>
        /// Gets the first component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">THe component to find</typeparam>
        /// <returns>The component if found, otherwise null.</returns>
        public IConsoleComponent GetComponent<TComponent>()
            where TComponent : IConsoleComponent
        {
            foreach (IConsoleComponent component in Components)
            {
                if (component is TComponent)
                {
                    return component;
                }
            }

            return null;
        }

        private void Components_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        FilterAddItem((IConsoleComponent)item);
                        ((IConsoleComponent)item).OnAdded(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        FilterRemoveItem((IConsoleComponent)item);
                        ((IConsoleComponent)item).OnRemoved(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.NewItems)
                    {
                        FilterAddItem((IConsoleComponent)item);
                        ((IConsoleComponent)item).OnAdded(this);
                    }
                    foreach (object item in e.OldItems)
                    {
                        FilterRemoveItem((IConsoleComponent)item);
                        ((IConsoleComponent)item).OnRemoved(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Calling Clear in this object is not supported. Use the RemoveAll extension method.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            void FilterAddItem(IConsoleComponent component)
            {
                if (component.IsDraw)
                {
                    if (!ComponentsDraw.Contains(component))
                    {
                        ComponentsDraw.Add(component);
                    }
                }

                if (component.IsUpdate)
                {
                    if (!ComponentsUpdate.Contains(component))
                    {
                        ComponentsUpdate.Add(component);
                    }
                }

                if (component.IsKeyboard)
                {
                    if (!ComponentsKeyboard.Contains(component))
                    {
                        ComponentsKeyboard.Add(component);
                    }
                }

                if (component.IsMouse)
                {
                    if (!ComponentsMouse.Contains(component))
                    {
                        ComponentsMouse.Add(component);
                    }
                }

                if (!component.IsDraw && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                {
                    if (!ComponentsEmpty.Contains(component))
                    {
                        ComponentsEmpty.Add(component);
                    }
                }

                ComponentsDraw.Sort(CompareComponent);
                ComponentsUpdate.Sort(CompareComponent);
                ComponentsKeyboard.Sort(CompareComponent);
                ComponentsMouse.Sort(CompareComponent);
            }

            void FilterRemoveItem(IConsoleComponent component)
            {
                if (component.IsDraw)
                {
                    if (ComponentsDraw.Contains(component))
                    {
                        ComponentsDraw.Remove(component);
                    }
                }

                if (component.IsUpdate)
                {
                    if (ComponentsUpdate.Contains(component))
                    {
                        ComponentsUpdate.Remove(component);
                    }
                }

                if (component.IsKeyboard)
                {
                    if (ComponentsKeyboard.Contains(component))
                    {
                        ComponentsKeyboard.Remove(component);
                    }
                }

                if (component.IsMouse)
                {
                    if (ComponentsMouse.Contains(component))
                    {
                        ComponentsMouse.Remove(component);
                    }
                }

                if (!component.IsDraw && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                {
                    if (!ComponentsEmpty.Contains(component))
                    {
                        ComponentsEmpty.Remove(component);
                    }
                }

                ComponentsDraw.Sort(CompareComponent);
                ComponentsUpdate.Sort(CompareComponent);
                ComponentsKeyboard.Sort(CompareComponent);
                ComponentsMouse.Sort(CompareComponent);
            }

            int CompareComponent(IConsoleComponent left, IConsoleComponent right)
            {
                if (left.SortOrder > right.SortOrder)
                {
                    return 1;
                }

                if (left.SortOrder < right.SortOrder)
                {
                    return -1;
                }

                return 0;
            }
        }

        /// <summary>
        /// Saves the <see cref="Console"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="Console"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static Console Load(string file) => Serializer.Load<Console>(file, Settings.SerializationIsCompressed);
    }
}
