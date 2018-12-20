using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.DrawCalls;
using SadConsole.Effects;
using SadConsole.SerializedTypes;

namespace SadConsole
{
    /// <summary>
    /// An object that can be positioned on the screen and receive updates.
    /// </summary>
    public partial class ScreenObject: CellSurface
    {
        private ScreenObject _parentScreen;
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
        public ScreenObjectCollection Children { get; }

        /// <summary>
        /// The parent object that this instance is a child of.
        /// </summary>
        public ScreenObject Parent
        {
            get => _parentScreen;
            set
            {
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
        public ScreenObject(int width, int height): this(width, height, SadConsole.Global.FontDefault)
        {
            
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and font.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public ScreenObject(int width, int height, Font font): base(width, height)
        {
            Children = new ScreenObjectCollection(this);
            _font = font;
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Basic();
            _drawCall = new DrawCallScreenObject(this, Point.Zero, false);

            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    RenderRects[index] = _font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * _font.Size.X, Height * _font.Size.Y);
            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="cells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        public ScreenObject(int width, int height, Font font, Cell[] cells) : base(width, height, cells)
        {
            Children = new ScreenObjectCollection(this);
            _font = font;
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Basic();
            _drawCall = new DrawCallScreenObject(this, Point.Zero, false);

            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    RenderRects[index] = _font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * _font.Size.X, Height * _font.Size.Y);
            LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }

        internal ScreenObject(): base(1, 1)
        {
            Children = new ScreenObjectCollection(this);
            _font = Global.FontDefault;
            RenderCells = new Cell[Cells.Length];
            RenderRects = new Rectangle[Cells.Length];
            Renderer = new Renderers.Basic();
            _drawCall = new DrawCallScreenObject(this, Point.Zero, false);

            var index = 0;

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    RenderRects[index] = _font.GetRenderRect(x, y);
                    RenderCells[index] = this[x, y];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, Width * _font.Size.X, Height * _font.Size.Y);
        }
        
        ///// <summary>
        ///// Initialization is left to the derived class.
        ///// </summary>
        //protected ScreenObject() { }

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
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentChanged(ScreenObject oldParent, ScreenObject newParent) { }

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
        /// Called when this console's focus has been lost.
        /// </summary>
        protected virtual void OnFocusLost() { }

        /// <summary>
        /// Called when this console is focused.
        /// </summary>
        protected virtual void OnFocused() { }
    }
}
