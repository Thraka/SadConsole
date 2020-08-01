using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class ButtonTheme : ThemeBase
    {
        /// <summary>
        /// When true, renders the <see cref="EndCharacterLeft"/> 
        /// and <see cref="EndCharacterRight"/> on the button.
        /// </summary>
        [DataMember]
        public bool ShowEnds { get; set; } = true;

        /// <summary>
        /// The character on the left side of the button. Defaults to '&lt;'.
        /// </summary>
        [DataMember]
        public int EndCharacterLeft { get; set; } = '<';

        /// <summary>
        /// The character on the right side of the button. Defaults to '>'.
        /// </summary>
        [DataMember]
        public int EndCharacterRight { get; set; } = '>';

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance = GetStateAppearance(control.State);

            int middle = (button.Height != 1 ? button.Height / 2 : 0);

            // Redraw the control
            button.Surface.Fill(
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph, null);

            if (ShowEnds && button.Width >= 3)
            {
                button.Surface.Print(1, middle, (button.Text).Align(button.TextAlignment, button.Width - 2));
                button.Surface.SetGlyph(0, middle, EndCharacterLeft);
                button.Surface.SetGlyph(button.Width - 1, middle, EndCharacterRight);
            }
            else
            {
                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width));
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new ButtonTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            ShowEnds = ShowEnds,
            EndCharacterLeft = EndCharacterLeft,
            EndCharacterRight = EndCharacterRight
        };
    }

    /// <summary>
    /// A 3D shadow theme of the button control
    /// </summary>
    [DataContract]
    public class Button3dTheme : ButtonTheme
    {
        private ColoredGlyph _shade;
        protected ColoredGlyph _shadeDefault = new ColoredGlyph();

        /// <summary>
        /// The shade appearance for the 3D aspect of the button.
        /// </summary>
        [DataMember]
        public ColoredGlyph Shade
        {
            get => _shade ?? _shadeDefault;
            set => _shade = value;
        }


        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width + 2, control.Height + 1)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance = GetStateAppearance(control.State);

            int middle = button.Height != 1 ? button.Height / 2 : 0;

            var shadowBounds = new Rectangle(0, 0, button.Width, button.Height).WithPosition((2, 1));

            button.Surface.Clear();

            if (appearance.Matches(MouseDown))
            {
                middle += 1;

                // Redraw the control
                button.Surface.Fill(shadowBounds,
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                button.Surface.Print(shadowBounds.X, middle, button.Text.Align(button.TextAlignment, button.Width));
                button.MouseArea = new Rectangle(0, 0, button.Width + 2, button.Height + 1);
            }
            else
            {
                // Redraw the control
                button.Surface.Fill(new Rectangle(0, 0, button.Width, button.Height),
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width));

                // Bottom line
                button.Surface.DrawLine(new Point(shadowBounds.X, shadowBounds.MaxExtentY),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 1
                button.Surface.DrawLine(new Point(shadowBounds.MaxExtentX - 1, shadowBounds.Y),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 2
                button.Surface.DrawLine(new Point(shadowBounds.MaxExtentX, shadowBounds.Y),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                button.MouseArea = new Rectangle(0, 0, button.Width, button.Height);
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors colors, ControlBase control)
        {
            if (colors == null) colors = control.FindThemeColors();

            base.RefreshTheme(colors, control);

            _shadeDefault.Foreground = colors.ControlBackDark;
            _shadeDefault.Background = Color.Transparent;
            _shadeDefault.Glyph = 176;

            Normal.Foreground = colors.Appearance_ControlNormal.Foreground;
            Normal.Background = colors.ControlBackLight;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new Button3dTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            _shade = _shade?.Clone(),
            ShowEnds = ShowEnds,
            EndCharacterLeft = EndCharacterLeft,
            EndCharacterRight = EndCharacterRight
        };
    }

    /// <summary>
    /// A 3D theme of the button control using thin lines. Supports the SadConsole extended character set.
    /// </summary>
    [DataContract]
    public class ButtonLinesTheme : ButtonTheme
    {
        private ColoredGlyph _topLeftLineColors;
        protected ColoredGlyph _topLeftLineColorsDefault = new ColoredGlyph();

        private ColoredGlyph _bottomRightLineColors;
        protected ColoredGlyph _bottomRightLineColorsDefault = new ColoredGlyph();

        /// <summary>
        /// The colors to use for the top and left lines.
        /// </summary>
        [DataMember]
        public ColoredGlyph TopLeftLineColors
        {
            get => _topLeftLineColors ?? _topLeftLineColorsDefault;
            set => _topLeftLineColors = value;
        }

        /// <summary>
        /// The colors to use for the bottom and right lines.
        /// </summary>
        [DataMember]
        public ColoredGlyph BottomRightLineColors
        {
            get => _bottomRightLineColors ?? _bottomRightLineColorsDefault;
            set => _bottomRightLineColors = value;
        }

        [DataMember]
        public bool UseExtended { get; set; }

        public ButtonLinesTheme() =>
            UseExtended = true;

        /// <inheritdoc />
        public override void RefreshTheme(Colors colors, ControlBase control)
        {
            if (colors == null) colors = control.FindThemeColors();

            base.RefreshTheme(colors, control);

            _topLeftLineColorsDefault = new ColoredGlyph(colors.Gray, Color.Transparent);
            _bottomRightLineColorsDefault = new ColoredGlyph(colors.GrayDark, Color.Transparent);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance;
            bool mouseDown = false;
            bool mouseOver = false;
            bool focused = false;

            if (Helpers.HasFlag((int)button.State, (int)ControlStates.Disabled))
                appearance = Disabled;
            else
                appearance = Normal;

            if (Helpers.HasFlag((int)button.State, (int)ControlStates.MouseLeftButtonDown) ||
                Helpers.HasFlag((int)button.State, (int)ControlStates.MouseRightButtonDown))
                mouseDown = true;

            if (Helpers.HasFlag((int)button.State, (int)ControlStates.MouseOver))
                mouseOver = true;

            if (Helpers.HasFlag((int)button.State, (int)ControlStates.Focused))
                focused = true;


            // Middle part of the button for text.
            int middle = button.Surface.BufferHeight != 1 ? button.Surface.BufferHeight / 2 : 0;
            Color topleftcolor = !mouseDown ? TopLeftLineColors.Foreground : BottomRightLineColors.Foreground;
            Color bottomrightcolor = !mouseDown ? BottomRightLineColors.Foreground : TopLeftLineColors.Foreground;
            Color textColor = Normal.Foreground;

            if (button.Surface.BufferHeight > 1 && button.Surface.BufferHeight % 2 == 0)
            {
                middle -= 1;
            }

            if (mouseOver)
                textColor = MouseOver.Foreground;
            else if (focused)
                textColor = Focused.Foreground;

            // Extended font draw
            if (button.Parent.Host.ParentConsole.Font.IsSadExtended && UseExtended)
            {
                // Redraw the control
                button.Surface.Fill(appearance.Foreground, appearance.Background,
                                    appearance.Glyph, Mirror.None);

                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), textColor);

                if (button.Height == 1)
                {
                    button.Surface.SetDecorator(0, button.Surface.BufferWidth,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));
                    button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.BufferWidth - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else if (button.Height == 2)
                {
                    button.Surface.SetDecorator(0, button.Surface.BufferWidth,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));

                    


                    button.Surface.SetDecorator(Point.ToIndex(0,  button.Surface.BufferHeight - 1, button.Surface.BufferWidth), button.Surface.BufferWidth,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                    button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(Point.ToIndex(0,  1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.BufferWidth - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    button.Surface.AddDecorator(Point.ToIndex(button.Surface.BufferWidth - 1,  1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else
                {
                    button.Surface.SetDecorator(0, button.Surface.BufferWidth,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));

                    button.Surface.SetDecorator(Point.ToIndex(0,  button.Surface.BufferHeight - 1, button.Surface.BufferWidth), button.Surface.BufferWidth,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                    button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(Point.ToIndex(0,  button.Surface.BufferHeight - 1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(button.Surface.BufferWidth - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    button.Surface.AddDecorator(Point.ToIndex(button.Surface.BufferWidth - 1,  button.Surface.BufferHeight - 1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));

                    for (int y = 0; y < button.Surface.BufferHeight - 2; y++)
                    {
                        button.Surface.AddDecorator(Point.ToIndex(0,  y + 1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                        button.Surface.AddDecorator(Point.ToIndex(button.Surface.BufferWidth - 1,  y + 1, button.Surface.BufferWidth), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    }
                }
            }
            else // Non extended normal draw
            {
                button.Surface.Fill(appearance.Foreground, appearance.Background,
                    appearance.Glyph, Mirror.None);

                button.Surface.Print(1, middle, button.Text.Align(button.TextAlignment, button.Width - 2), textColor);

                button.Surface.DrawBox(new Rectangle(0, 0, button.Width, button.Surface.BufferHeight),
                                       new ColoredGlyph(topleftcolor, appearance.Background, 0),
                                       null,
                                       connectedLineStyle: focused ? ICellSurface.ConnectedLineThick : ICellSurface.ConnectedLineThin);

                //SadConsole.Algorithms.Line(0, 0, button.Width - 1, 0, (x, y) => { return true; });

                button.Surface.DrawLine(new Point(0, 0), new Point(button.Width - 1, 0), topleftcolor, appearance.Background);
                button.Surface.DrawLine(new Point(0, 0), new Point(0, button.Surface.BufferHeight - 1), topleftcolor, appearance.Background);
                button.Surface.DrawLine(new Point(button.Width - 1, 0), new Point(button.Width - 1, button.Surface.BufferHeight - 1), bottomrightcolor, appearance.Background);
                button.Surface.DrawLine(new Point(1, button.Surface.BufferHeight - 1), new Point(button.Width - 1, button.Surface.BufferHeight - 1), bottomrightcolor, appearance.Background);
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new ButtonLinesTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            _topLeftLineColors = _topLeftLineColors?.Clone(),
            _bottomRightLineColors = _bottomRightLineColors?.Clone(),
            ShowEnds = ShowEnds,
            EndCharacterLeft = EndCharacterLeft,
            EndCharacterRight = EndCharacterRight,
            UseExtended = UseExtended
        };
    }
}
