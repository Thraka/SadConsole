using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    public partial class Console
    {
        private Color _tint = Color.Transparent;

        private Font _font;
        private Renderers.IRenderer _renderer;
        private bool _isCursorDisabled;

        ///// <summary>
        ///// When <see langword="true"/>, indicates that the <see cref="Cursor"/> cannot be used on this console; otherwise, <see langword="false"/>.
        ///// </summary>
        ///// <remarks>
        ///// This property should only be used to indicate that this object can never use the <see cref="Cursor"/>. To simply disable or enable the <see cref="Cursor"/>, use <see cref="Cursor.IsEnabled"/> and <see cref="Cursor.IsVisible"/>.
        ///// </remarks>
        //public bool IsCursorDisabled
        //{
        //    get => _isCursorDisabled;
        //    set
        //    {
        //        _isCursorDisabled = value;

        //        if (value)
        //        {
        //            if (_renderer != null && _renderer.BeforeRenderTintCallback == null)
        //            {
        //                _renderer.BeforeRenderTintCallback = OnBeforeRender;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// The renderer used to draw this surface.
        /// </summary>
        public Renderers.IRenderer Renderer
        {
            get => _renderer;
            set
            {
                //if (_renderer?.BeforeRenderTintCallback == OnBeforeRender)
                //{
                //    _renderer.BeforeRenderTintCallback = null;
                //}

                //_renderer = value;

                //if (!IsCursorDisabled)
                //{
                //    _renderer.BeforeRenderTintCallback = OnBeforeRender;
                //}

                _renderer?.Detatch(this);
                _renderer = value;
                _renderer?.Attach(this);
            }
        }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                if (_font == value)
                {
                    return;
                }

                _font = value;
                OnFontChanged();
                SetRenderCells();
                IsDirty = true;
            }
        }

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
        /// Pixel area of the render cells.
        /// </summary>
        public Rectangle AbsoluteArea { get; protected set; }

        ///// <summary>
        ///// The private virtual cursor reference.
        ///// </summary>
        //public Cursor Cursor { get; }

        /// <summary>
        /// Toggles the VirtualCursor as visible\hidden when the console if focused\unfocused.
        /// </summary>
        public bool AutoCursorOnFocus { get; set; }

        /// <summary>
        /// Disposes <see cref="LastRenderResult"/>.
        /// </summary>
        ~Console() => _renderer?.Dispose();

        /// <summary>
        /// Draws all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        public virtual void Draw(TimeSpan timeElapsed)
        {
            if (!IsVisible) return;

            if (_renderer != null)
            {
                if (IsDirty)
                {
                    _renderer.Refresh(this);
                    IsDirty = false;
                }
                _renderer.Render();
                //Global.DrawCalls.Add(new DrawCalls.DrawCallScreenObject(this, CalculatedPosition, true));
            }

            foreach (Components.IConsoleComponent component in ComponentsDraw.ToArray())
                component.Draw(this, timeElapsed);

            foreach (Console child in new List<Console>(Children))
                child.Draw(timeElapsed);
        }

        /// <summary>
        /// Updates all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsPaused"/> is <see langword="false"/>.</remarks>
        public virtual void Update(TimeSpan timeElapsed)
        {
            if (IsPaused)
            {
                return;
            }

            //Effects.UpdateEffects(timeElapsed.TotalSeconds);

            //if (!IsCursorDisabled && Cursor.IsVisible)
            //{
            //    Cursor.Update(timeElapsed);
            //}

            foreach (Components.IConsoleComponent component in ComponentsUpdate.ToArray())
                component.Update(this, timeElapsed);

            foreach (Console child in new List<Console>(Children))
                child.Update(timeElapsed);
        }


        // Extension method added by the monogame library

        ///// <summary>
        ///// Changes the glyph of a specified cell to a glyph definition from the font.
        ///// </summary>
        ///// <param name="x">The x location of the cell.</param>
        ///// <param name="y">The y location of the cell.</param>
        ///// <param name="glyphName">The name of the glyph definition to use. If invalid, sets it to 0.</param>
        //public void SetGlyph(int x, int y, string glyphName)
        //{
        //    if (!IsValidCell(x, y, out int index))
        //    {
        //        return;
        //    }

        //    FontMaster.GlyphDefinition definition = Font.Master.GetGlyphDefinition(glyphName);

        //    Cells[index].Glyph = definition.Glyph == -1 ? 0 : definition.Glyph;
        //    IsDirty = true;
        //}

        /// <summary>
        /// Configures <see cref="RenderCells"/>, <see cref="RenderRects"/>, and <see cref="LastRenderResult"/> for rendering.
        /// </summary>
        public virtual void SetRenderCells()
        {
            if (RenderCells.Length != Width * Height)
            {
                RenderRects = new Rectangle[Width * Height];
                RenderCells = new Cell[Width * Height];
            }

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

            _renderer?.Attach(this);

            IsDirty = true;
        }

        /// <summary>
        /// Called when the <see cref="Font"/> property changes.
        /// </summary>
        protected virtual void OnFontChanged() { }

        ///// <summary>
        ///// Creates a new console from an existing surface.
        ///// </summary>
        ///// <param name="surface"></param>
        ///// <param name="font">The font to associate with the new console.</param>
        ///// <returns>A new console.</returns>
        //public static Console FromSurface(CellSurface surface, Font font) => new Console(surface.Width, surface.Height, font, surface.Cells);
    }
}
