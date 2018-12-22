using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.DrawCalls;
using SadConsole.Effects;
using SadConsole.SerializedTypes;
using Console = SadConsole.Console;

namespace SadConsole
{
    /// <summary>
    /// A <see cref="CellSurface"/> that has a font and can be drawn to the screen.
    /// </summary>
    [JsonConverter(typeof(SerializedTypes.ConsoleJsonConverter))]
    public partial class Console : CellSurface
    {
        private Console _parentScreen;
        private Point _position;
        private bool _isVisible = true;
        private bool _isPaused;

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
                var oldPosition = _position;
                _position = value;
                OnCalculateRenderPosition();
                OnPositionChanged(oldPosition);
            }
        }

        /// <summary>
        /// A position that is based on the <see cref="Parent"/> position.
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
                if (value == this) throw new Exception("Cannot set parent to itself.");
                if (_parentScreen == value) return;
                if (_parentScreen == null)
                {
                    _parentScreen = value;
                    _parentScreen.Children.Add(this);
                    OnCalculateRenderPosition();
                    OnParentChanged(null, _parentScreen);
                }
                else
                {
                    var oldParent = _parentScreen;
                    _parentScreen = value;

                    oldParent.Children.Remove(this);

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
                if (_isVisible == value) return;
                _isVisible = value;
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
                if (_isPaused == value) return;
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
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

                        OnFocused();
                    }

                    else if (value && Global.FocusedConsoles.Console == this)
                        OnFocused();

                    else if (!value)
                    {
                        if (Global.FocusedConsoles.Console == this)
                            Global.FocusedConsoles.Pop(this);

                        OnFocusLost();
                    }
                }
                else
                {
                    if (value)
                    {
                        if (FocusedMode == ActiveBehavior.Push)
                            Global.FocusedConsoles.Push(this);
                        else
                            Global.FocusedConsoles.Set(this);

                        OnFocused();
                    }
                    else
                        OnFocusLost();
                }
            }
        }
        
        /// <summary>
        /// Creates a new text surface with the specified width and height. Uses <see cref="Global.FontDefault"/> as the font.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        public Console(int width, int height): this(width, height, SadConsole.Global.FontDefault)
        {
            
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and font.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public Console(int width, int height, Font font): this(width, height, font, null)
        {
            
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="cells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        public Console(int width, int height, Font font, Cell[] cells) : base(width, height, cells)
        {
            Children = new ConsoleCollection(this);
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Console();
            _font = font;
            _drawCall = new DrawCallScreenObject(this, Point.Zero, false);

            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
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

        internal Console(): base(1, 1)
        {
            IsCursorDisabled = true;
            Children = new ConsoleCollection(this);
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Console();
            _font = Global.FontDefault;
            _drawCall = new DrawCallScreenObject(this, Point.Zero, false);

            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * Font.Size.X, Height * Font.Size.Y);
        }
        
        protected override void OnCellsReset()
        {
            SetRenderCells();
        }

        /// <summary>
        /// Sets a value for <see cref="CalculatedPosition"/> based on the <see cref="Position"/> of this instance and the <see cref="Parent"/> instance.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            CalculatedPosition = Position + (Parent?.CalculatedPosition ?? Point.Zero);

            foreach (var child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Called when this console's focus has been lost. Hides the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        protected virtual void OnFocusLost()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = false;
        }

        /// <summary>
        /// Called when this console is focused. Shows the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
        /// </summary>
        protected virtual void OnFocused()
        {
            if (AutoCursorOnFocus)
                Cursor.IsVisible = true;
        }
        
        /// <summary>
        /// Called when the renderer renders the text view.
        /// </summary>
        /// <param name="batch">The batch used in rendering.</param>
        protected virtual void OnBeforeRender(SpriteBatch batch)
        {
            if (Cursor.IsVisible && IsValidCell(Cursor.Position.X, Cursor.Position.Y))
                Cursor.Render(batch, Font, Font.GetRenderRect(Cursor.Position.X, Cursor.Position.Y));
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
