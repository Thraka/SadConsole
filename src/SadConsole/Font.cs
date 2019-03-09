#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a specific font size from a <see cref="FontMaster"/>.
    /// </summary>
    public sealed class Font
    {
        private int solidGlyphIndex;
        private Rectangle solidGlyphRect;

        /// <summary>
        /// The size options of a font.
        /// </summary>
        public enum FontSizes
        {
            /// <summary>
            /// One quater the size of the font. (Original Width and Height * 0.25)
            /// </summary>
            Quarter = 0,

            /// <summary>
            /// Half the size of the font. (Original Width and Height * 0.50)
            /// </summary>
            Half = 1,

            /// <summary>
            /// Exact size of the font. (Original Width and Height * 1.0)
            /// </summary>
            One = 2,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 2.0)
            /// </summary>
            Two = 3,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 3.0)
            /// </summary>
            Three = 4,

            /// <summary>
            /// Two times the size of the font. (Original Width and Height * 4.0)
            /// </summary>
            Four = 5
        }

        /// <summary>
        /// The texture of the font.
        /// </summary>
        public Texture2D FontImage { get; private set; }

        /// <summary>
        /// The width and height of each glyph.
        /// </summary>
        public Point Size { get; private set; }

        /// <summary>
        /// The maximum upper inclusive glyph index of the font.
        /// </summary>
        public int MaxGlyphIndex { get; private set; }

        /// <summary>
        /// Which glyph index is considered completely solid. Used for shading.
        /// </summary>
        public int SolidGlyphIndex { get => solidGlyphIndex;
            set { solidGlyphIndex = value; solidGlyphRect = GlyphRects[value]; } }

        /// <summary>
        /// The rectangle associated with the <see cref="SolidGlyphIndex"/>.
        /// </summary>
        public Rectangle SolidGlyphRectangle => solidGlyphRect;

        /// <summary>
        /// A cached array of rectangles of individual glyphs.
        /// </summary>
        public Rectangle[] GlyphRects { get; private set; }

        /// <summary>
        /// How many columns are in the this font.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// How many rows are in this font.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The size originally used to create the font from a <see cref="FontMaster"/>.
        /// </summary>
        public FontSizes SizeMultiple { get; private set; }

        /// <summary>
        /// The name of the font used when it is registered with the <see cref="Global.Fonts"/> collection.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The <see cref="FontMaster"/> that created this <see cref="Font"/> instance.
        /// </summary>
        public FontMaster Master { get; private set; }

        internal Font() { }

        internal Font(FontMaster masterFont, FontSizes fontMultiple)
        {
            Initialize(masterFont, fontMultiple);
        }

        private void Initialize(FontMaster masterFont, FontSizes fontMultiple)
        {
            Master = masterFont;
            FontImage = masterFont.Image;
            MaxGlyphIndex = masterFont.Rows * masterFont.Columns - 1;

            switch (fontMultiple)
            {
                case FontSizes.Quarter:
                    Size = new Point((int)(masterFont.GlyphWidth * 0.25), (int)(masterFont.GlyphHeight * 0.25));
                    break;
                case FontSizes.Half:
                    Size = new Point((int)(masterFont.GlyphWidth * 0.5), (int)(masterFont.GlyphHeight * 0.5));
                    break;
                case FontSizes.One:
                    Size = new Point(masterFont.GlyphWidth, masterFont.GlyphHeight);
                    break;
                case FontSizes.Two:
                    Size = new Point(masterFont.GlyphWidth * 2, masterFont.GlyphHeight * 2);
                    break;
                case FontSizes.Three:
                    Size = new Point(masterFont.GlyphWidth * 3, masterFont.GlyphHeight * 3);
                    break;
                case FontSizes.Four:
                    Size = new Point(masterFont.GlyphWidth * 4, masterFont.GlyphHeight * 4);
                    break;
                default:
                    break;
            }

            if (Size.X == 0 || Size.Y == 0)
                throw new ArgumentException($"This font cannot use size {fontMultiple.ToString()}, at least one axis is 0.", "fontMultiple");

            SizeMultiple = fontMultiple;
            Name = masterFont.Name;
            GlyphRects = masterFont.GlyphIndexRects;
            SolidGlyphIndex = masterFont.SolidGlyphIndex;
            Rows = masterFont.Rows;
            Columns = masterFont.Columns;
        }

        /// <summary>
        /// Resizes the graphics device manager based on this font's glyph size.
        /// </summary>
        /// <param name="manager">Graphics device manager to resize.</param>
        /// <param name="width">The width glyphs.</param>
        /// <param name="height">The height glyphs.</param>
        /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
        /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
        public void ResizeGraphicsDeviceManager(GraphicsDeviceManager manager, int width, int height, int additionalWidth, int additionalHeight)
        {
            manager.PreferredBackBufferWidth = (Size.X * width) + additionalWidth;
            manager.PreferredBackBufferHeight = (Size.Y * height) + additionalHeight;

            Global.RenderWidth = manager.PreferredBackBufferWidth;
            Global.RenderHeight = manager.PreferredBackBufferHeight;

            manager.ApplyChanges();
        }

        /// <summary>
        /// Returns a rectangle that is positioned and sized based on the font and the cell position specified.
        /// </summary>
        /// <param name="x">The x-axis of the cell position.</param>
        /// <param name="y">The y-axis of the cell position.</param>
        /// <returns>A new rectangle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public Rectangle GetRenderRect(int x, int y)
        {
            return new Rectangle(x * Size.X, y * Size.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Gets the pixel position of a cell position based on the font size.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <returns>A new pixel point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetWorldPosition(int x, int y)
        {
            return GetWorldPosition(new Point(x, y));
        }

        /// <summary>
        /// Gets the pixel position of a cell position based on the font size.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <returns>A new pixel point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetWorldPosition(Point position)
        {
            return new Point(position.X * Size.X, position.Y * Size.Y);
        }
    }
}
