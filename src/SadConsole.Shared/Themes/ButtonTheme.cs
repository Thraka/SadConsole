using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class ButtonTheme: ThemeBase
    {
        /// <summary>
        /// When true, renders the <see cref="EndCharacterLeft"/> and <see cref="EndCharacterRight"/> on the button.
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
            control.Surface = new CellSurface(control.Width, control.Height);

            base.Attached(control);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            
            if (!button.IsDirty) return;

            Cell appearance;

            if (Helpers.HasFlag(button.State, ControlStates.Disabled))
                appearance = Disabled;
                
            else if (Helpers.HasFlag(button.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(button.State, ControlStates.MouseRightButtonDown))
                appearance = MouseDown;

            else if (Helpers.HasFlag(button.State, ControlStates.MouseOver))
                appearance = MouseOver;

            else if (Helpers.HasFlag(button.State, ControlStates.Focused))
                appearance = Focused;

            else
                appearance = Normal;

            var middle = (button.Height != 1 ? button.Height / 2 : 0);

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
                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width));

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new ButtonTheme()
            {
                Colors = Colors?.Clone(),
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
    }

    /// <summary>
    /// A 3D shadow theme of the button control
    /// </summary>
    [DataContract]
    public class Button3dTheme : ButtonTheme
    {
        [DataMember]
        public Cell Shade;
        
        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width + 2, control.Height + 1);

            var colors = Colors ?? control.Parent?.Theme.Colors ?? Library.Default.Colors;

            RefreshTheme(colors);
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            //TODO shade should not hard code the glyph
            Shade = new Cell(themeColors.ControlBackDark, Color.Transparent, 176);
            Normal = new Cell(themeColors.CyanDark, themeColors.ControlBackLight);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            
            if (!button.IsDirty) return;

            Cell appearance;

            if (Helpers.HasFlag(button.State, ControlStates.Disabled))
                appearance = Disabled;

            else if (Helpers.HasFlag(button.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(button.State, ControlStates.MouseRightButtonDown))
                appearance = MouseDown;

            else if (Helpers.HasFlag(button.State, ControlStates.MouseOver))
                appearance = MouseOver;

            else if (Helpers.HasFlag(button.State, ControlStates.Focused))
                appearance = Focused;

            else
                appearance = Normal;


            var middle = button.Height != 1 ? button.Height / 2 : 0;

            Rectangle shadowBounds = new Rectangle(0, 0, button.Width, button.Height);
            shadowBounds.Location += new Point(2, 1);

            button.Surface.Clear();

            if (appearance == MouseDown)
            {
                middle += 1;

                // Redraw the control
                button.Surface.Fill(shadowBounds,
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                button.Surface.Print(shadowBounds.Left, middle, button.Text.Align(button.TextAlignment, button.Width));
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
                button.Surface.DrawLine(new Point(shadowBounds.Left, shadowBounds.Bottom - 1),
                    new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 1
                button.Surface.DrawLine(new Point(shadowBounds.Right - 2, shadowBounds.Top),
                    new Point(shadowBounds.Right - 2, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 2
                button.Surface.DrawLine(new Point(shadowBounds.Right - 1, shadowBounds.Top),
                    new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new Button3dTheme()
            {
                Colors = Colors.Clone(),
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                Shade = Shade.Clone()
            };
        }
    }

    /// <summary>
    /// A 3D theme of the button control using thin lines. Supports the SadConsole extended character set.
    /// </summary>
    [DataContract]
    public class ButtonLinesTheme : ButtonTheme
    {
        [DataMember]
        public Cell TopLeftLineColors;

        [DataMember]
        public Cell BottomRightLineColors;

        [DataMember]
        public bool UseExtended;

        public ButtonLinesTheme()
        {            
            UseExtended = true;
        }
        
        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            TopLeftLineColors = new Cell(themeColors.Gray, Color.Transparent);
            BottomRightLineColors = new Cell(themeColors.GrayDark, Color.Transparent);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            
            if (!button.IsDirty) return;

            Cell appearance;
            var mouseDown = false;
            var mouseOver = false;
            var focused = false;

            if (Helpers.HasFlag(button.State, ControlStates.Disabled))
                appearance = Disabled;
            else
                appearance = Normal;

            if (Helpers.HasFlag(button.State, ControlStates.MouseLeftButtonDown) ||
                Helpers.HasFlag(button.State, ControlStates.MouseRightButtonDown))
                mouseDown = true;

            if (Helpers.HasFlag(button.State, ControlStates.MouseOver))
                mouseOver = true;

            if (Helpers.HasFlag(button.State, ControlStates.Focused))
                focused = true;


            // Middle part of the button for text.
            var middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
            var topleftcolor = !mouseDown ? TopLeftLineColors.Foreground : BottomRightLineColors.Foreground;
            var bottomrightcolor = !mouseDown ? BottomRightLineColors.Foreground : TopLeftLineColors.Foreground;
            Color textColor = Normal.Foreground;

            if (mouseOver)
                textColor = MouseOver.Foreground;
            else if (focused)
                textColor = Focused.Foreground;

            // Redraw the control
            button.Surface.Fill(appearance.Foreground, appearance.Background,
                appearance.Glyph, SpriteEffects.None);

            button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), textColor);
                
            button.Surface.DrawBox(new Rectangle(0,0,button.Width, button.Surface.Height), new Cell(topleftcolor, TopLeftLineColors.Background, 0),
                connectedLineStyle: button.Parent.Font.Master.IsSadExtended ? CellSurface.ConnectedLineThinExtended : CellSurface.ConnectedLineThin);

            //SadConsole.Algorithms.Line(0, 0, button.Width - 1, 0, (x, y) => { return true; });

            button.Surface.DrawLine(Point.Zero, new Point(button.Width - 1, 0), topleftcolor, appearance.Background);
            button.Surface.DrawLine(Point.Zero, new Point(0, button.Surface.Height - 1), topleftcolor, appearance.Background);
            button.Surface.DrawLine(new Point(button.Width - 1, 0), new Point(button.Width - 1, button.Surface.Height - 1), bottomrightcolor, appearance.Background);
            button.Surface.DrawLine(new Point(1, button.Surface.Height - 1), new Point(button.Width - 1, button.Surface.Height - 1), bottomrightcolor, appearance.Background);

            if (button.Parent.Font.Master.IsSadExtended && UseExtended)
            {
                // Tweak the corners
                //hostSurface.SetGlyph(0, 0, 0);
                button.Surface.SetGlyph(button.Width - 1, 0, 0);
                button.Surface.SetGlyph(0, button.Surface.Height - 1, 0);
                //hostSurface.SetGlyph(control.Width - 1, control.Surface.Height - 1, 0);

                button.Surface.SetDecorator(
                    new Point(0, button.Surface.Height - 1).ToIndex(button.Width), 1, new[]
                    {
                        button.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                        button.Parent.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor)
                    });

                //hostSurface.SetDecorator(new Point(0, 0).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-top", topleftcolor)
                //});

                button.Surface.SetDecorator(
                    new Point(button.Width - 1, 0).ToIndex(button.Width), 1, new[]
                    {
                        button.Parent.Font.Master.GetDecorator("box-edge-top", topleftcolor),
                        button.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                    });

                //hostSurface.SetDecorator(new Point(control.Width - 1, control.Surface.Height - 1).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                //});

            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new ButtonLinesTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                TopLeftLineColors = TopLeftLineColors.Clone(),
                BottomRightLineColors = BottomRightLineColors.Clone()
            };
        }
    }

}
