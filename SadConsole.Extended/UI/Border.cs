﻿using System;
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

            /// <summary>
            /// Creates an instance of BorderParameters with the thin line as a border, no title and no shadow.
            /// </summary>
            /// <returns></returns>
            public static BorderParameters GetDefault()
            {
                return new BorderParameters(true, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph()),
                                            string.Empty, HorizontalAlignment.Center, Color.White, Color.Black,
                                            false, true, true, 0, Color.White, Color.Black);
            }

            /// <summary>
            /// Sets the Title parameter with the given text.
            /// </summary>
            /// <param name="title">Title text.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddTitle(string title)
            {
                Title = title;
                return this;
            }

            /// <summary>
            /// Sets the Title parameter with the given text and changes title colors.
            /// </summary>
            /// <param name="title">Title text.</param>
            /// <param name="foregroundColor">Title foreground <see cref="SadRogue.Primitives.Color"/>.</param>
            /// <param name="backgroundColor">Title background <see cref="HorizontalAlignment"/>.</param>
            /// <param name="horizontalAlignment">Title text <see cref="HorizontalAlignment"/>.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddTitle(string title, Color foregroundColor, Color backgroundColor, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
            {
                Title = title;
                TitleForeground = backgroundColor;
                TitleBackground = foregroundColor;
                TitleAlignment = horizontalAlignment;
                return this;
            }

            /// <summary>
            /// Sets the DrawShadedArea to true.
            /// </summary>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddShadow()
            {
                DrawShadedArea = true;
                return this;
            }

            /// <summary>
            /// Sets the DrawShadedArea to true and modifies shadow glyph.
            /// </summary>
            /// <param name="shadedGlyph">Glyph number to use as a shadow.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddShadow(int shadedGlyph)
            {
                DrawShadedArea = true;
                UseDefaultShadedGlyphs = false;
                ShadedGlyph = shadedGlyph;
                return this;
            }

            /// <summary>
            /// Sets the DrawShadedArea to true and modifies default shadow colors.
            /// </summary>
            /// <param name="foregroundColor">ShadedGlyph new foreground color.</param>
            /// <param name="backgroundColor">ShadedGlyph new background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddShadow(Color foregroundColor, Color backgroundColor)
            {
                DrawShadedArea = true;
                UseDefaultShadedColors = false;
                ShadedGlyphForeground = foregroundColor;
                ShadedGlyphBackground = backgroundColor;
                return this;
            }

            /// <summary>
            /// Sets the DrawShadedArea to true and modifies default shadow glyph and colors.
            /// </summary>
            /// /// <param name="shadedGlyph">Glyph number to use as a shadow.</param>
            /// <param name="foregroundColor">ShadedGlyph new foreground color.</param>
            /// <param name="backgroundColor">ShadedGlyph new background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters AddShadow(int shadedGlyph, Color foregroundColor, Color backgroundColor)
            {
                AddShadow(shadedGlyph);
                AddShadow(foregroundColor, backgroundColor);
                return this;
            }

            /// <summary>
            /// Set the BorderBox parameter to the new value.
            /// </summary>
            /// <param name="borderStyle">New <see cref="ShapeParameters"/> to use for the border style.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderStyle(ShapeParameters borderStyle)
            {
                BorderBox = borderStyle;
                return this;
            }

            /// <summary>
            /// Changes the array of glyphs to be used as the border line.
            /// </summary>
            /// <param name="borderStyle">Array of glyphs to be used as the border line.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderGlyph(int[] borderStyle)
            {
                BorderBox.BoxBorderStyle = borderStyle;
                return this;
            }

            /// <summary>
            /// Changes the array of glyphs to be used as the border line and their colors.
            /// </summary>
            /// <param name="borderStyle">Array of glyphs to be used as the border line.</param>
            /// <param name="foregroundColor">Border line foreground color.</param>
            /// <param name="backgroundColor">Border line background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderGlyph(int[] borderStyle, Color foregroundColor, Color backgroundColor)
            {
                BorderBox.BoxBorderStyle = borderStyle;
                ChangeBorderColors(foregroundColor, backgroundColor);
                return this;
            }

            /// <summary>
            /// Fills the array of glyphs to be used as the border line with the given glyph.
            /// </summary>
            /// <param name="glyph">The glyph to be used the border line.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderGlyph(int glyph)
            {
                BorderBox.BoxBorderStyle = ICellSurface.CreateLine(glyph);
                return this;
            }

            /// <summary>
            /// Fills the array of glyphs to be used as the border line with the given glyph and changes the border colors.
            /// </summary>
            /// <param name="glyph">The glyph to be used the border line.</param>
            /// <param name="foregroundColor">Border line foreground color.</param>
            /// <param name="backgroundColor">Border line background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderGlyph(int glyph, Color foregroundColor, Color backgroundColor)
            {
                ChangeBorderGlyph(glyph);
                ChangeBorderColors(foregroundColor, backgroundColor);
                return this;
            }

            /// <summary>
            /// Fills the array of glyphs to be used as the border line with the given <see cref="ColoredGlyph"/>.
            /// </summary>
            /// <param name="glyph">The <see cref="ColoredGlyph"/> to be used the border line.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderGlyph(ColoredGlyph glyph)
            {
                ChangeBorderGlyph(glyph.Glyph);
                ChangeBorderColors(glyph.Foreground, glyph.Background);
                return this;
            }

            /// <summary>
            /// Changes border color parameters.
            /// </summary>
            /// <param name="foregroundColor">Border line foreground color.</param>
            /// <param name="backgroundColor">Border line background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderColors(Color foregroundColor, Color backgroundColor)
            {
                BorderBox.BorderGlyph.Foreground = foregroundColor;
                BorderBox.BorderGlyph.Background = backgroundColor;
                return this;
            }

            /// <summary>
            /// Changes border foreground color.
            /// </summary>
            /// <param name="foregroundColor">Border line foreground color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderForegroundColor(Color foregroundColor)
            {
                BorderBox.BorderGlyph.Foreground = foregroundColor;
                return this;
            }

            /// <summary>
            /// Changes border background color.
            /// </summary>
            /// <param name="backgroundColor">Border line background color.</param>
            /// <returns>The modified instance of <see cref="BorderParameters"/>.</returns>
            public BorderParameters ChangeBorderBackgroundColor(Color backgroundColor)
            {
                BorderBox.BorderGlyph.Background = backgroundColor;
                return this;
            }
        }

        /// <summary>
        /// Creates a border and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="parameters"><see cref="BorderParameters"/> to be used in creating the <see cref="Border"/>.</param>
        /// <param name="font">Optional <see cref="IFont"/> for the border <see cref="CellSurface"/>.</param>
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
        /// Creates a border (with a shadow and a title) and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Title to display on the border.</param>
        public Border(IScreenSurface contents, string title) : this(contents, BorderParameters.GetDefault().AddTitle(title).AddShadow())
        {

        }

        /// <summary>
        /// Creates a border (with a shadow and a title) and adds it as a child object to <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The object the border will be around.</param>
        /// <param name="title">Optional title to display on the border.</param>
        /// <param name="textColor">Title text foreground color.</param>
        /// <param name="borderColor">Border line foreground color and title text background color.</param>
        public Border(IScreenSurface contents, string title, Color textColor, Color borderColor) :
            this(contents, BorderParameters.GetDefault().AddTitle(title, textColor, borderColor)
                .ChangeBorderForegroundColor(borderColor).AddShadow())
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
