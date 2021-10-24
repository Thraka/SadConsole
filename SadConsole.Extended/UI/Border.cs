using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.UI
{
    /// <summary>
    /// Creates a 3D border around a surface.
    /// </summary>
    public class Border: ScreenSurface
    {
        /// <summary>
        /// The settings to use when creating a <see cref="Border"/>.
        /// </summary>
        public struct BorderParameters
        {
            /// <summary>
            /// When <see langword="true"/>, indicates the <see cref="BorderBox"/> should be used to draw a border.
            /// </summary>
            public bool DrawBorder { get; set; }

            /// <summary>
            /// The border box creation parameters.
            /// </summary>
            public ShapeParameters BorderBox { get; set; }

            /// <summary>
            /// The title to display on the border.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The foreground color of the title.
            /// </summary>
            public Color TitleForeground { get; set; }

            /// <summary>
            /// The background color of the title.
            /// </summary>
            public Color TitleBackground { get; set; }

            /// <summary>
            /// The alignment of the <see cref="Title"/>.
            /// </summary>
            public HorizontalAlignment TitleAlignment { get; set; }

            /// <summary>
            /// When <see langword="true"/>, indicates the 3d shaded area of the border should be drawn.
            /// </summary>
            public bool DrawShadedArea { get; set; }

            /// <summary>
            /// When <see langword="true"/>, indicates the  other shaded properties should be ignored and the default shaded glyphs should be used.
            /// </summary>
            public bool UseDefaultShadedGlyphs { get; set; }


            /// <summary>
            /// When <see langword="true"/>, indicates the other shaded properties should be ignored and the default shaded glyphs should be used.
            /// </summary>
            public bool UseDefaultShadedColors { get; set; }

            /// <summary>
            /// The glyph to use in the shaded area.
            /// </summary>
            public int ShadedGlyph { get; set; }

            /// <summary>
            /// The forground of the <see cref="ShadedGlyph"/>.
            /// </summary>
            public Color ShadedGlyphForeground { get; set; }
 
            /// <summary>
            /// The background of the <see cref="ShadedGlyph"/>.
            /// </summary>
            public Color ShadedGlyphBackground { get; set; }

            /// <summary>
            /// Creates a new instance of this class which contains the settings for drawing a border surface around an existing surface.
            /// </summary>
            /// <param name="drawBorder">When <see langword="true"/>, indicates the <paramref name="borderBox"/> should be used to draw a border.</param>
            /// <param name="borderBox">The box drawn around the surface.</param>
            /// <param name="title">The title to display on the border.</param>
            /// <param name="titleAlignment">The alignment of the <paramref name="title"/>.</param>
            /// <param name="titleForeground">The foreground color of the <paramref name="title"/>.</param>
            /// <param name="titleBackground">The background color of the <paramref name="title"/>.</param>
            /// <param name="drawShadedArea">When <see langword="true"/>, indicates the 3d shaded area of the border should be drawn.</param>
            /// <param name="useDefaultShadedGlyphs">When <see langword="true"/>, indicates the  other shaded properties should be ignored and the default shaded glyphs should be used.</param>
            /// <param name="useDefaultShadedColors">When <see langword="true"/>, indicates the other shaded properties should be ignored and the default shaded glyphs should be used.</param>
            /// <param name="shadedGlyph">The glyph to use in the shaded area.</param>
            /// <param name="shadedGlyphForeground">The forground of the <paramref name="shadedGlyph"/>.</param>
            /// <param name="shadedGlyphBackground">The background of the <paramref name="shadedGlyph"/>.</param>
            public BorderParameters(bool drawBorder, ShapeParameters borderBox, string title, HorizontalAlignment titleAlignment, Color titleForeground, Color titleBackground, bool drawShadedArea, bool useDefaultShadedGlyphs, bool useDefaultShadedColors, int shadedGlyph, Color shadedGlyphForeground, Color shadedGlyphBackground)
            {
                if (!drawBorder && !drawShadedArea) throw new InvalidOperationException("Creation parameters for a border must contain at least a drawable border or a drawable shaded area");

                DrawBorder = drawBorder;
                BorderBox = borderBox;
                Title = title;
                TitleAlignment = titleAlignment;
                TitleForeground = titleForeground;
                TitleBackground = titleBackground;
                DrawShadedArea = drawShadedArea;
                UseDefaultShadedGlyphs = useDefaultShadedGlyphs;
                UseDefaultShadedColors = useDefaultShadedColors;
                ShadedGlyph = shadedGlyph;
                ShadedGlyphForeground = shadedGlyphForeground;
                ShadedGlyphBackground = shadedGlyphBackground;
            }
        }

        public Border(IScreenSurface contents, BorderParameters parameters, IFont font = null) : base(new CellSurface(contents.Surface.Width + GetSize(parameters),
                                                                                                                        contents.Surface.Height + GetSize(parameters)),
                                                                                                        font ?? contents.Font, contents.FontSize)
        {
            if (parameters.DrawBorder)
            {
                Position = (-1, -1);

                var colorGlyph = parameters.BorderBox.BorderGlyph ?? parameters.BorderBox.BoxBorderStyleGlyphs[0];

                Surface.DefaultForeground = parameters.BorderBox.IgnoreBorderForeground ? contents.Surface.DefaultForeground : colorGlyph.Foreground;
                Surface.Clear();
            }
            else
            {
                Surface.DefaultForeground = contents.Surface.DefaultForeground;
                Surface.Clear();
            }

            if (parameters.DrawShadedArea)
            {
                // Draw the box first, in the smaller area, the other area gets the 3d shade.
                if (parameters.DrawBorder)
                    Surface.DrawBox(new Rectangle(0, 0, Surface.Width - 1, Surface.Height - 1), parameters.BorderBox);

                Color foreground = parameters.UseDefaultShadedColors ? Color.AnsiWhite : parameters.ShadedGlyphForeground;
                Color background = parameters.UseDefaultShadedColors ? Color.Transparent : parameters.ShadedGlyphBackground;
                int rightSideBar = parameters.UseDefaultShadedGlyphs ? 219 : parameters.ShadedGlyph; //█
                int bottomBar = parameters.UseDefaultShadedGlyphs ? 223 : parameters.ShadedGlyph; //▀
                int corner = parameters.UseDefaultShadedGlyphs ? 220 : parameters.ShadedGlyph; //▄

                Surface.DrawLine((Surface.Width - 1, 2), (Surface.Width - 1, Surface.Height - 2), rightSideBar, foreground, background);
                Surface.DrawLine((1, Surface.Height - 1), (Surface.Width - 1, Surface.Height - 1), bottomBar, foreground, background);
                Surface.SetGlyph(Surface.Width - 1, 1, corner, foreground, background);
            }
            else
                Surface.DrawBox(Surface.Area, parameters.BorderBox);

            var title = parameters.Title;

            if (!string.IsNullOrEmpty(title))
                Surface.Print(((Surface.Width - 1) / 2) - ((title.Length + 2) / 2), 0, title.Align(parameters.TitleAlignment, title.Length + 2, ' '), parameters.TitleBackground == Color.Transparent ? contents.Surface[0].Background : parameters.TitleBackground, parameters.TitleForeground);

            IsEnabled = false;
            UseMouse = false;
            UseKeyboard = false;

            contents.Children.Add(this);
        }


        /// <summary>
        /// Creates a border and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Title to display on the border.</param>
        public Border(IScreenSurface contents, string title) : this(contents, new BorderParameters(true, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                                                                                                                                     new ColoredGlyph(contents.Surface.DefaultForeground,
                                                                                                                                                      contents.Surface.DefaultBackground)),
                                                                                                     title, HorizontalAlignment.Center, Color.White, Color.Black,
                                                                                                     true, true, true, 0, Color.White, Color.White))
        {

        }

        /// <summary>
        /// Creates a border and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Optional title to display on the border.</param>
        /// <param name="innerBorderForeground">The color of the </param>
        /// <param name="outerBorderForeground"></param>
        public Border(IScreenSurface contents, string title, Color innerBorderForeground, Color outerBorderForeground) : this(contents, new BorderParameters(true, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                                                                                                                                     new ColoredGlyph(innerBorderForeground,
                                                                                                                                                      outerBorderForeground)),
                                                                                                     title, HorizontalAlignment.Center, outerBorderForeground, innerBorderForeground,
                                                                                                     true, true, true, 0, Color.White, Color.White))
        {
            
        }

        /// <summary>
        /// Creates a border and adds it as a child object to the window.
        /// </summary>
        /// <param name="contents">The window the border will be around.</param>
        public Border(Window contents): this (contents, new BorderParameters(false, null, null, HorizontalAlignment.Center, Color.White, Color.White, true, true, true, 0, Color.White, Color.White))
        {

        }

        /// <summary>
        /// Helper method to add a border to a surface.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Optional title to display on the border.</param>
        public static void AddToSurface(IScreenSurface contents, string title) =>
            new Border(contents, title);

        /// <summary>
        /// Helper method to add a border to a window.
        /// </summary>
        /// <param name="contents">The window the border will be around.</param>
        public static void AddToWindow(Window contents) =>
            new Border(contents);


        private static int GetSize(BorderParameters parameters)
        {
            if (parameters.DrawShadedArea && parameters.DrawBorder)
                return 3;
            else if (parameters.DrawBorder)
                return 2;
            else if (parameters.DrawShadedArea)
                return 1;
            else
                return 0;
        }
    }
}
