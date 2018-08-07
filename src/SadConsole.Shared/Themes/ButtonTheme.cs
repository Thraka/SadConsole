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

        public override void Render(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
                int middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;

                // Redraw the control
                hostSurface.Fill(control.Bounds, 
                                control.StateAppearance.Foreground, 
                                control.StateAppearance.Background, 
                                control.StateAppearance.Glyph, null);

                if (ShowEnds)
                {
                    hostSurface.Print(control.Position.X + 1, middle, (control.Text).Align(control.TextAlignment, control.Width - 2));
                    hostSurface.SetGlyph(control.Position.X, middle, EndCharacterLeft);
                    hostSurface.SetGlyph(control.Width, middle, EndCharacterRight);
                }
                else
                    hostSurface.Print(control.Position.X, middle, control.Text.Align(control.TextAlignment, control.Width));
                
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

        public override void Render(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
                int middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;

                Rectangle shadowBounds = control.Bounds;
                shadowBounds.Location += new Point(2, 1);

                hostSurface.Clear(new Rectangle(control.Bounds.X, control.Bounds.Y, control.Width + 2, control.Height + 1));

                if (control.StateAppearance == Selected)
                {
                    middle += 1;

                    // Redraw the control
                    hostSurface.Fill(shadowBounds,
                        control.StateAppearance.Foreground,
                        control.StateAppearance.Background,
                        control.StateAppearance.Glyph, null);

                    hostSurface.Print(shadowBounds.X, middle, control.Text.Align(control.TextAlignment, control.Width));
                }
                else
                {
                    // Redraw the control
                    hostSurface.Fill(control.Bounds,
                        control.StateAppearance.Foreground,
                        control.StateAppearance.Background,
                        control.StateAppearance.Glyph, null);

                    hostSurface.Print(control.Bounds.X, middle, control.Text.Align(control.TextAlignment, control.Width));

                    // Bottom line
                    hostSurface.DrawLine(new Point(shadowBounds.Location.X, shadowBounds.Bottom - 1),
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
            TopLeftLineColors = new Cell(Themes.Colors.White, Color.Transparent);
            BottomRightLineColors = new Cell(Themes.Colors.Gray, Color.Transparent);
        }

        public override void Render(Button control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
                int middle = (control.Height != 1 ? control.Height / 2 : 0) + control.Position.Y;

                // Redraw the control
                hostSurface.Fill(control.Bounds,
                                control.StateAppearance.Foreground,
                                control.StateAppearance.Background,
                                control.StateAppearance.Glyph, null);

                hostSurface.DrawLine(control.Position, new Point(control.Bounds.Right - 1, control.Bounds.Top), TopLeftLineColors.Foreground, TopLeftLineColors.Background, ConnectedLineStyle[0]);
                hostSurface.DrawLine(control.Position, new Point(control.Position.X, control.Bounds.Bottom - 1), TopLeftLineColors.Foreground, TopLeftLineColors.Background, ConnectedLineStyle[0]);
                hostSurface.DrawLine(control.Position, new Point(control.Bounds.Right - 1, control.Bounds.Top), TopLeftLineColors.Foreground, TopLeftLineColors.Background, ConnectedLineStyle[0]);
                hostSurface.DrawLine(control.Position, new Point(control.Bounds.Right - 1, control.Bounds.Top), TopLeftLineColors.Foreground, TopLeftLineColors.Background, ConnectedLineStyle[0]);

                control.IsDirty = false;
                hostSurface.IsDirty = true;
            }
        }
    }

}
