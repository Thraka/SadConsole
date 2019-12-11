using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// Represents an individual piece of a <see cref="CellSurface"/> containing a glyph, foreground color, background color, and a mirror effect.
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(SerializedTypes.ColoredGlyphJsonConverter))]
    public class ColoredGlyph : IEquatable<ColoredGlyph>
    {
        private Color _foreground;
        private Color _background;
        private Mirror _mirror;
        private int _glyph;

        /// <summary>
        /// Modifies the look of a cell with additional character. 
        /// </summary>
        public CellDecorator[] Decorators { get; set; } = Array.Empty<CellDecorator>();

        /// <summary>
        /// The foreground color of this cell.
        /// </summary>
        public Color Foreground
        {
            get => _foreground;
            set { _foreground = value; IsDirty = true; }
        }

        /// <summary>
        /// The background color of this cell.
        /// </summary>
        public Color Background
        {
            get => _background;
            set { _background = value; IsDirty = true; }
        }

        /// <summary>
        /// The glyph index from a font for this cell.
        /// </summary>
        public int Glyph
        {
            get => _glyph;
            set { _glyph = value; IsDirty = true; }
        }

        /// <summary>
        /// The mirror effect for this cell.
        /// </summary>
        public Mirror Mirror
        {
            get => _mirror;
            set { _mirror = value; IsDirty = true; }
        }

        /// <summary>
        /// The glyph.
        /// </summary>
        public char GlyphCharacter
        {
            get => (char)Glyph;
            set => Glyph = value;
        }

        /// <summary>
        /// <see langword="true"/> when this cell should be drawn; otherwise, <see langword="false"/>.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// <see langword="true"/> when this cell needs to be redrawn; otherwise, <see langword="false"/>.
        /// </summary>
        public bool IsDirty { get; set; } = true;

        /// <summary>
        /// Creates a cell with a white foreground, black background, glyph 0, and no mirror effect.
        /// </summary>
        public ColoredGlyph() : this(Color.White, Color.Black, 0, Mirror.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, black background, glyph 0, and no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        public ColoredGlyph(Color foreground) : this(foreground, Color.Black, 0, Mirror.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, specified background, glyph 0, and no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        public ColoredGlyph(Color foreground, Color background) : this(foreground, background, 0, Mirror.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, background, and glyph, with no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        public ColoredGlyph(Color foreground, Color background, int glyph) : this(foreground, background, glyph, Mirror.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, background, glyph, and mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="mirror">The mirror effect.</param>
        public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
        }

        /// <summary>
        /// Creates a cell with the specified foreground, background, glyph, mirror, and visibility.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="mirror">The mirror effect.</param>
        /// <param name="isVisible">The visiblity of the glyph.</param>
        public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
            IsVisible = isVisible;
        }

        /// <summary>
        /// Creates a cell with the specified foreground, background, glyph, and mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="mirror">The mirror effect.</param>
        /// <param name="isVisible">The visiblity of the glyph.</param>
        /// <param name="decorators">Decorators for the cell.</param>
        public ColoredGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, CellDecorator[] decorators)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
            IsVisible = isVisible;
            Decorators = decorators;
        }

        /// <summary>
        /// Copies the visual appearance to the specified cell. This includes foreground, background, glyph, and mirror effect.
        /// </summary>
        /// <param name="cell">The target cell to copy to.</param>
        public void CopyAppearanceTo(ColoredGlyph cell)
        {
            cell.Foreground = Foreground;
            cell.Background = Background;
            cell.Glyph = Glyph;
            cell.Mirror = Mirror;
            cell.Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>();
        }

        /// <summary>
        /// Sets the foreground, background, glyph, and mirror effect to the same as the specified cell.
        /// </summary>
        /// <param name="cell">The target cell to copy from.</param>
        public void CopyAppearanceFrom(ColoredGlyph cell)
        {
            Foreground = cell.Foreground;
            Background = cell.Background;
            Glyph = cell.Glyph;
            Mirror = cell.Mirror;
            Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : Array.Empty<CellDecorator>();
        }

        /// <summary>
        /// Resets the foreground, background, glyph, and mirror effect.
        /// </summary>
        public void Clear()
        {
            Foreground = Color.White;
            Background = Color.Black;
            Glyph = 0;
            Mirror = Mirror.None;
            Decorators = Array.Empty<CellDecorator>();
        }

        /* TODO: Move this to extension methods in the actual Renderers library (monogame, gdi+, etc)
        /// <summary>
        /// Draws a single cell using the specified SpriteBatch.
        /// </summary>
        /// <param name="batch">Renderers batch.</param>
        /// <param name="position">Pixel position on the screen to render.</param>
        /// <param name="size">Renderers size of the cell.</param>
        /// <param name="font">Font used to draw the cell.</param>
        public void Draw(SpriteBatch batch, Point position, Point size, Font font) => Draw(batch, new Rectangle(position.X, position.Y, size.X, size.Y), font);

        /// <summary>
        /// Draws a single cell using the specified SpriteBatch.
        /// </summary>
        /// <param name="batch">Renderers batch.</param>
        /// <param name="drawingRectangle">Where on the sreen to draw the cell, in pixels.</param>
        /// <param name="font">Font used to draw the cell.</param>
        public void Draw(SpriteBatch batch, Rectangle drawingRectangle, Font font)
        {
            if (Background != Color.Transparent)
            {
                batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[font.SolidGlyphIndex], Background, 0f, Vector2.Zero, Mirror.None, 0.3f);
            }

            if (Foreground != Color.Transparent)
            {
                batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[Glyph], Foreground, 0f, Vector2.Zero, Mirror, 0.4f);
            }

            foreach (ColoredGlyphDecorator decorator in Decorators)
            {
                if (decorator.Color != Color.Transparent)
                {
                    batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
                }
            }
        }
        */

        /// <summary>
        /// Returns a new cell with the same properties as this one.
        /// </summary>
        /// <returns>The new cell.</returns>
        public ColoredGlyph Clone() =>
            new ColoredGlyph(Foreground, Background, Glyph, Mirror)
            {
                IsVisible = IsVisible,
                Decorators = Decorators.Length != 0 ? Decorators.ToArray() : Array.Empty<CellDecorator>()
            };

        public override bool Equals(object obj)
        {
            return Equals(obj as ColoredGlyph);
        }

        public bool Equals(ColoredGlyph other)
        {
            return other != null &&
                   EqualityComparer<CellDecorator[]>.Default.Equals(Decorators, other.Decorators) &&
                   Foreground.Equals(other.Foreground) &&
                   Background.Equals(other.Background) &&
                   Glyph == other.Glyph &&
                   EqualityComparer<Mirror>.Default.Equals(Mirror, other.Mirror) &&
                   IsVisible == other.IsVisible;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Decorators, Foreground, Background, Glyph, Mirror, IsVisible, IsDirty);
        }

        public static bool operator ==(ColoredGlyph left, ColoredGlyph right)
        {
            return EqualityComparer<ColoredGlyph>.Default.Equals(left, right);
        }

        public static bool operator !=(ColoredGlyph left, ColoredGlyph right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Creates an array of colored glyphs.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ColoredGlyph[] CreateArray(int size)
        {
            ColoredGlyph[] cells = new ColoredGlyph[size];

            for (int i = 0; i < size; i++)
                cells[i] = new ColoredGlyph();

            return cells;
        }
    }
}
