using System;
using System.Net.Mime;
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
    public class ButtonTheme : ThemeBase<Button>
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

        public ButtonTheme()
        {

        }

        public override void Attached(Button control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(Button control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell appearance;

            if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                appearance = Disabled;
                
            else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                appearance = MouseDown;

            else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                appearance = MouseOver;

            else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                appearance = Focused;

            else
                appearance = Normal;

            var middle = (control.Height != 1 ? control.Height / 2 : 0);

            // Redraw the control
            control.Surface.Fill(
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph, null);

            if (ShowEnds)
            {
                control.Surface.Print(1, middle, (control.Text).Align(control.TextAlignment, control.Width - 2));
                control.Surface.SetGlyph(0, middle, EndCharacterLeft);
                control.Surface.SetGlyph(control.Width - 1, middle, EndCharacterRight);
            }
            else
                control.Surface.Print(0, middle, control.Text.Align(control.TextAlignment, control.Width));

            control.IsDirty = false;
        }

        /// <inheritdoc />
        public override object Clone()
        {
            return new ButtonTheme()
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
    }

    /// <summary>
    /// A 3D shadow theme of the button control
    /// </summary>
    [DataContract]
    public class Button3dTheme : ButtonTheme
    {
        [DataMember]
        public Cell Shade;

        public Button3dTheme()
        {
            Shade = new Cell(Colors.ControlBackDark, Color.Transparent, 176);
            Normal = new Cell(Colors.CyanDark, Colors.ControlBackLight);
        }

        public override void Attached(Button control)
        {
            control.Surface = new BasicNoDraw(control.Width + 2, control.Height + 1);
        }

        public override void UpdateAndDraw(Button control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell appearance;

            if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                appearance = Disabled;

            else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                appearance = MouseDown;

            else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                appearance = MouseOver;

            else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                appearance = Focused;

            else
                appearance = Normal;


            var middle = control.Height != 1 ? control.Height / 2 : 0;

            Rectangle shadowBounds = new Rectangle(0, 0, control.Width, control.Height);
            shadowBounds.Location += new Point(2, 1);

            control.Surface.Clear();

            if (appearance == MouseDown)
            {
                middle += 1;

                // Redraw the control
                control.Surface.Fill(shadowBounds,
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                control.Surface.Print(shadowBounds.Left, middle, control.Text.Align(control.TextAlignment, control.Width));
            }
            else
            {
                // Redraw the control
                control.Surface.Fill(new Rectangle(0, 0, control.Width, control.Height),
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                control.Surface.Print(0, middle, control.Text.Align(control.TextAlignment, control.Width));

                // Bottom line
                control.Surface.DrawLine(new Point(shadowBounds.Left, shadowBounds.Bottom - 1),
                    new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 1
                control.Surface.DrawLine(new Point(shadowBounds.Right - 2, shadowBounds.Top),
                    new Point(shadowBounds.Right - 2, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);

                // Side line 2
                control.Surface.DrawLine(new Point(shadowBounds.Right - 1, shadowBounds.Top),
                    new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                    Shade.Glyph);
            }

            control.IsDirty = false;
        }

        /// <inheritdoc />
        public override object Clone()
        {
            return new Button3dTheme()
            {
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

        public ButtonLinesTheme()
        {
            TopLeftLineColors = new Cell(Themes.Colors.Gray, Color.Transparent);
            BottomRightLineColors = new Cell(Themes.Colors.GrayDark, Color.Transparent);
        }

        public override void Attached(Button control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(Button control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell appearance;
            var mouseDown = false;
            var mouseOver = false;
            var focused = false;

            if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                appearance = Disabled;
            else
                appearance = Normal;

            if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) ||
                Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                mouseDown = true;

            if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                mouseOver = true;

            if (Helpers.HasFlag(control.State, ControlStates.Focused))
                focused = true;


            // Middle part of the button for text.
            var middle = control.Height != 1 ? control.Height / 2 : 0;
            var topleftcolor = !mouseDown ? TopLeftLineColors.Foreground : BottomRightLineColors.Foreground;
            var bottomrightcolor = !mouseDown ? BottomRightLineColors.Foreground : TopLeftLineColors.Foreground;
            Color textColor = Normal.Foreground;

            if (mouseOver)
                textColor = MouseOver.Foreground;
            else if (focused)
                textColor = Focused.Foreground;

            // Redraw the control
            control.Surface.Fill(appearance.Foreground, appearance.Background,
                appearance.Glyph, SpriteEffects.None);

            control.Surface.Print(0, middle, control.Text.Align(control.TextAlignment, control.Width), textColor);
                
            control.Surface.DrawBox(new Rectangle(0,0,control.Width, control.Height), topleftcolor, TopLeftLineColors.Background,
                connectedLineStyle: control.Parent.Font.Master.IsSadExtended ? SurfaceBase.ConnectedLineThinExtended : SurfaceBase.ConnectedLineThin);

            control.Surface.DrawLine(Point.Zero, new Point(control.Width - 1, 0), topleftcolor, appearance.Background);
            control.Surface.DrawLine(Point.Zero, new Point(0, control.Height - 1), topleftcolor, appearance.Background);
            control.Surface.DrawLine(new Point(control.Width - 1, 0), new Point(control.Width - 1, control.Height - 1), bottomrightcolor, appearance.Background);
            control.Surface.DrawLine(new Point(1, control.Height - 1), new Point(control.Width - 1, control.Height - 1), bottomrightcolor, appearance.Background);

            if (control.Parent.Font.Master.IsSadExtended)
            {
                // Tweak the corners
                //hostSurface.SetGlyph(0, 0, 0);
                control.Surface.SetGlyph(control.Width - 1, 0, 0);
                control.Surface.SetGlyph(0, control.Height - 1, 0);
                //hostSurface.SetGlyph(control.Width - 1, control.Height - 1, 0);

                control.Surface.SetDecorator(
                    new Point(0, control.Height - 1).ToIndex(control.Width), 1, new[]
                    {
                        control.Parent.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                        control.Parent.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor)
                    });

                //hostSurface.SetDecorator(new Point(0, 0).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-top", topleftcolor)
                //});

                control.Surface.SetDecorator(
                    new Point(control.Width - 1, 0).ToIndex(control.Width), 1, new[]
                    {
                        control.Parent.Font.Master.GetDecorator("box-edge-top", topleftcolor),
                        control.Parent.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                    });

                //hostSurface.SetDecorator(new Point(control.Width - 1, control.Height - 1).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                //});

            }

            control.IsDirty = false;
        }

        public override object Clone()
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
