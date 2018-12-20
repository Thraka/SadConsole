using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.DrawCalls;

namespace SadConsole
{
    public partial class ScreenObject
    {
        private Color _tint = Color.Transparent;
        private bool _isDirty = true;
        private DrawCallScreenObject _drawCall;

        protected Font _font;
        
        /// <summary>
        /// The renderer used to draw this surface.
        /// </summary>
        public Renderers.IRenderer Renderer { get; protected set; }

        /// <summary>
        /// Font used with rendering.
        /// </summary>
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                SetRenderCells();
                OnFontChanged();
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

        /// <summary>
        /// The last texture render pass for this surface.
        /// </summary>
        public RenderTarget2D LastRenderResult { get; protected set; }
        
        /// <summary>
        /// Disposes <see cref="LastRenderResult"/>.
        /// </summary>
        ~ScreenObject() => LastRenderResult?.Dispose();
        
        /// <summary>
        /// Draws all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        public virtual void Draw(TimeSpan timeElapsed)
        {
            if (!IsVisible) return;

            if (LastRenderResult != null)
            {
                Renderer.Render(this);

                _drawCall.Position = UsePixelPositioning ? CalculatedPosition.ToVector2() : Font.GetWorldPosition(CalculatedPosition).ToVector2();

                Global.DrawCalls.Add(_drawCall);
            }

            var copyList = new List<ScreenObject>(Children);

            foreach (var child in copyList)
                child.Draw(timeElapsed);
        }

        /// <summary>
        /// Updates all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsPaused"/> is <see langword="false"/>.</remarks>
        public virtual void Update(TimeSpan timeElapsed)
        {
            if (IsPaused) return;

            Effects.UpdateEffects(timeElapsed.TotalSeconds);

            var copyList = new List<ScreenObject>(Children);

            foreach (var child in copyList)
                child.Update(timeElapsed);
        }

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

            if (LastRenderResult.Bounds.Width != AbsoluteArea.Width || LastRenderResult.Bounds.Height != AbsoluteArea.Height)
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        ///// <summary>
        ///// Creates the initial <see cref="Cells"/> array and sets each cell to a new instance.
        ///// </summary>
        //protected virtual void InitializeCells()
        //{
        //    Cells = new Cell[Width * Height];

        //    for (int i = 0; i < Cells.Length; i++)
        //        Cells[i] = new Cell(this.DefaultForeground, this.DefaultBackground, 0);

        //    RenderCells = (Cell[])Cells.Clone();
        //    RenderRects = new Rectangle[Cells.Length];
        //}

        /// <summary>
        /// Called when the <see cref="Font"/> property changes.
        /// </summary>
        protected virtual void OnFontChanged() { }
    }
}