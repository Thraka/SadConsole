using System;
using System.Net.Mime;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
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
        public int EndCharacterLeft { get; set; } = (int)'<';

        /// <summary>
        /// The character on the right side of the button. Defaults to '>'.
        /// </summary>
        [DataMember]
        public int EndCharacterRight { get; set; } = (int)'>';

        public ButtonTheme()
        {
            Normal = Library.Default.Appearance_ControlNormal;
            Disabled = Library.Default.Appearance_ControlDisabled;
            MouseOver = Library.Default.Appearance_ControlOver;
            MouseDown = Library.Default.Appearance_ControlMouseDown;
            Selected = Library.Default.Appearance_ControlSelected;
            Focused = Library.Default.Appearance_ControlFocused;
        }

        public override void Draw(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
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

                

                int middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;

                // Redraw the control
                hostSurface.Fill(control.Bounds,
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                if (ShowEnds)
                {
                    hostSurface.Print(control.Bounds.Left + 1, middle, (control.Text).Align(control.TextAlignment, control.Width - 2));
                    hostSurface.SetGlyph(control.Bounds.Left, middle, EndCharacterLeft);
                    hostSurface.SetGlyph(control.Bounds.Right - 1, middle, EndCharacterRight);
                }
                else
                    hostSurface.Print(control.Bounds.Left, middle, control.Text.Align(control.TextAlignment, control.Width));
                
                control.IsDirty = false;
                hostSurface.IsDirty = true;
            }
        }
    }

    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class Button3dTheme : ButtonTheme
    {
        [DataMember]
        public Cell Shade;

        public Button3dTheme()
        {
            Shade = new Cell(Colors.ControlBackDark, Color.Transparent, 176);
        }

        public override void Draw(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
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


                int middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;

                Rectangle shadowBounds = control.Bounds;
                shadowBounds.Location += new Point(2, 1);

                hostSurface.Clear(new Rectangle(control.Bounds.Left, control.Bounds.Top, control.Width + 2, control.Height + 1));

                if (appearance == MouseDown)
                {
                    middle += 1;

                    // Redraw the control
                    hostSurface.Fill(shadowBounds,
                        appearance.Foreground,
                        appearance.Background,
                        appearance.Glyph, null);

                    hostSurface.Print(shadowBounds.Left, middle, control.Text.Align(control.TextAlignment, control.Width));
                }
                else
                {
                    // Redraw the control
                    hostSurface.Fill(control.Bounds,
                        appearance.Foreground,
                        appearance.Background,
                        appearance.Glyph, null);

                    hostSurface.Print(control.Bounds.Left, middle, control.Text.Align(control.TextAlignment, control.Width));

                    // Bottom line
                    hostSurface.DrawLine(new Point(shadowBounds.Left, shadowBounds.Bottom - 1),
                        new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                        Shade.Glyph);

                    // Side line 1
                    hostSurface.DrawLine(new Point(shadowBounds.Right - 2, shadowBounds.Top),
                        new Point(shadowBounds.Right - 2, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                        Shade.Glyph);

                    // Side line 2
                    hostSurface.DrawLine(new Point(shadowBounds.Right - 1, shadowBounds.Top),
                        new Point(shadowBounds.Right - 1, shadowBounds.Bottom - 1), Shade.Foreground, Shade.Background,
                        Shade.Glyph);
                }

                control.IsDirty = false;
                hostSurface.IsDirty = true;
            }
        }
    }

    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class ButtonLinesTheme : ButtonTheme
    {
        //TODO Finish this theme

        public Cell TopLeftLineColors;
        public Cell BottomRightLineColors;
        public int[] ConnectedLineStyle;


        public ButtonLinesTheme()
        {
            ConnectedLineStyle = SurfaceBase.ConnectedLineThinExtended;
            TopLeftLineColors = new Cell(Themes.Colors.Gray, Color.Transparent);
            BottomRightLineColors = new Cell(Themes.Colors.GrayDark, Color.Transparent);
        }

        public override void Draw(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
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
                var middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;
                var topleftcolor = !mouseDown ? TopLeftLineColors.Foreground : BottomRightLineColors.Foreground;
                var bottomrightcolor = !mouseDown ? BottomRightLineColors.Foreground : TopLeftLineColors.Foreground;
                Color textColor = Normal.Foreground;

                if (mouseOver)
                    textColor = MouseOver.Foreground;
                else if (focused)
                    textColor = Focused.Foreground;

                // Redraw the control
                hostSurface.Fill(control.Bounds,
                                appearance.Foreground,
                                appearance.Background,
                                appearance.Glyph, null);

                hostSurface.Print(control.Bounds.Left, middle, control.Text.Align(control.TextAlignment, control.Width), textColor);


                hostSurface.DrawBox(control.Bounds, topleftcolor, TopLeftLineColors.Background,
                    connectedLineStyle: ConnectedLineStyle);

                hostSurface.DrawLine(control.Position, new Point(control.Bounds.Right - 1, control.Bounds.Top), topleftcolor, appearance.Background);
                hostSurface.DrawLine(control.Position, new Point(control.Position.X, control.Bounds.Bottom - 1), topleftcolor, appearance.Background);
                hostSurface.DrawLine(new Point(control.Bounds.Right - 1, control.Bounds.Top), control.Position + new Point(control.Width - 1, control.Height - 1), bottomrightcolor, appearance.Background);
                hostSurface.DrawLine(new Point(control.Position.X + 1, control.Bounds.Bottom - 1), control.Position + new Point(control.Width - 1, control.Height - 1), bottomrightcolor, appearance.Background);



                // connectedLineStyle[(int)ConnectedLineIndex.TopLeft]

                // Tweak the corners
                //hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top, 0);
                hostSurface.SetGlyph(control.Bounds.Right - 1, control.Bounds.Top, 0);
                hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Bottom - 1, 0);
                //hostSurface.SetGlyph(control.Bounds.Right - 1, control.Bounds.Bottom - 1, 0);

                hostSurface.SetDecorator(new Point(control.Bounds.Left, control.Bounds.Bottom - 1).ToIndex(hostSurface.Width), 1, new[] {
                        hostSurface.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                        hostSurface.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor)
                    });

                //hostSurface.SetDecorator(new Point(control.Bounds.Left, control.Bounds.Top).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-left", topleftcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-top", topleftcolor)
                //});

                hostSurface.SetDecorator(new Point(control.Bounds.Right - 1, control.Bounds.Top).ToIndex(hostSurface.Width), 1, new[] {
                    hostSurface.Font.Master.GetDecorator("box-edge-top", topleftcolor),
                    hostSurface.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                });

                //hostSurface.SetDecorator(new Point(control.Bounds.Right - 1, control.Bounds.Bottom - 1).ToIndex(hostSurface.Width), 1, new[] {
                //    hostSurface.Font.Master.GetDecorator("box-edge-bottom", bottomrightcolor),
                //    hostSurface.Font.Master.GetDecorator("box-edge-right", bottomrightcolor)
                //});

                control.IsDirty = false;
                hostSurface.IsDirty = true;
            }
        }
    }

}
