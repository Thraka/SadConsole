using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole
{
    public partial class Console : ICellSurface
    {
        /// <inheritdoc/>
        public ColoredGlyph this[int index] => Surface[index];

        /// <inheritdoc/>
        public ColoredGlyph this[int x, int y] => Surface[x, y];

        /// <inheritdoc />
        public ColoredGlyph this[Point position] => Surface[position.ToIndex(BufferWidth)];

        /// <inheritdoc/>
        public int TimesShiftedDown { get => Surface.TimesShiftedDown; set => Surface.TimesShiftedDown = value; }
        /// <inheritdoc/>
        public int TimesShiftedRight { get => Surface.TimesShiftedRight; set => Surface.TimesShiftedRight = value; }
        /// <inheritdoc/>
        public int TimesShiftedLeft { get => Surface.TimesShiftedLeft; set => Surface.TimesShiftedLeft = value; }
        /// <inheritdoc/>
        public int TimesShiftedUp { get => Surface.TimesShiftedUp; set => Surface.TimesShiftedUp = value; }
        /// <inheritdoc/>
        public bool UsePrintProcessor { get => Surface.UsePrintProcessor; set => Surface.UsePrintProcessor = value; }

        /// <inheritdoc/>
        public EffectsManager Effects => Surface.Effects;

        /// <inheritdoc/>
        public Rectangle Buffer => Surface.Buffer;

        /// <inheritdoc/>
        public int BufferHeight => Surface.BufferHeight;

        /// <inheritdoc/>
        public int BufferWidth => Surface.BufferWidth;

        /// <inheritdoc/>
        public ColoredGlyph[] Cells => Surface.Cells;

        /// <inheritdoc/>
        public Color DefaultBackground { get => Surface.DefaultBackground; set => Surface.DefaultBackground = value; }
        /// <inheritdoc/>
        public Color DefaultForeground { get => Surface.DefaultForeground; set => Surface.DefaultForeground = value; }
        /// <inheritdoc/>
        public int DefaultGlyph { get => Surface.DefaultGlyph; set => Surface.DefaultGlyph = value; }

        /// <inheritdoc/>
        public bool IsScrollable => Surface.IsScrollable;

        /// <inheritdoc/>
        public Rectangle View { get => Surface.View; set => Surface.View = value; }
        /// <inheritdoc/>
        public int ViewHeight { get => Surface.ViewHeight; set => Surface.ViewHeight = value; }
        /// <inheritdoc/>
        public Point ViewPosition { get => Surface.ViewPosition; set => Surface.ViewPosition = value; }
        /// <inheritdoc/>
        public int ViewWidth { get => Surface.ViewWidth; set => Surface.ViewWidth = value; }

        /// <inheritdoc/>
        public event EventHandler IsDirtyChanged
        {
            add
            {
                Surface.IsDirtyChanged += value;
            }

            remove
            {
                Surface.IsDirtyChanged -= value;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<ColoredGlyph> GetEnumerator()
        {
            return Surface.GetEnumerator();
        }

        public ICellSurface GetSubSurface(Rectangle view) => Surface.GetSubSurface(view);

        public void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear) => Surface.Resize(width, height, bufferWidth, bufferHeight, clear);

        public void SetSurface(in ICellSurface surface, Rectangle view = default) => Surface.SetSurface(surface, view);

        public void SetSurface(in ColoredGlyph[] cells, int width, int height, int bufferWidth, int bufferHeight) => Surface.SetSurface(cells, width, height, bufferWidth, bufferHeight);
    }
}
