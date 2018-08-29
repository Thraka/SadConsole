using System;
using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The theme of a radio button control.
    /// </summary>
    [DataContract]
    public class ProgressBarTheme: ThemeBase<ProgressBar>
    {
        /// <summary>
        /// The theme of the unprogressed part of the bar.
        /// </summary>
        [DataMember]
        public ThemeStates Background;

        /// <summary>
        /// The theme of the progressed part of the bar.
        /// </summary>
        [DataMember]
        public ThemeStates Foreground;


        public ProgressBarTheme()
        {
            Background = new ThemeStates();
            Background.SetForeground(Normal.Foreground);
            Background.SetBackground(Normal.Background);
            Background.SetGlyph(176);
            Background.Disabled = new Cell(Color.Gray, Color.Black, 176);
            Foreground = new ThemeStates();
            Foreground.SetForeground(Normal.Foreground);
            Foreground.SetBackground(Normal.Background);
            Foreground.SetGlyph(219);
            Foreground.Disabled = new Cell(Color.Gray, Color.Black, 219);
        }

        public override void Attached(ProgressBar control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(ProgressBar control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell foregroundAppearance;
            Cell backgroundAppearance;

            if (Helpers.HasFlag(control.State, ControlStates.Disabled))
            {
                foregroundAppearance = Foreground.Disabled;
                backgroundAppearance = Background.Disabled;
            }

            else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) ||
                     Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
            {
                foregroundAppearance = Foreground.MouseDown;
                backgroundAppearance = Background.MouseDown;
            }

            else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
            {
                foregroundAppearance = Foreground.MouseOver;
                backgroundAppearance = Background.MouseOver;
            }

            else if (Helpers.HasFlag(control.State, ControlStates.Focused))
            {
                foregroundAppearance = Foreground.Focused;
                backgroundAppearance = Background.Focused;
            }

            else
            {
                foregroundAppearance = Foreground.Normal;
                backgroundAppearance = Background.Normal;
            }

            control.Surface.Clear();

            control.Surface.Fill(backgroundAppearance.Foreground, backgroundAppearance.Background, backgroundAppearance.Glyph);

            if (control.IsHorizontal)
            {
                Rectangle fillRect;

                if (control.HorizontalAlignment == HorizontalAlignment.Left)
                    fillRect = new Rectangle(0, 0, control.fillSize, control.Height);
                else
                    fillRect = new Rectangle(control.Width - control.fillSize, 0, control.fillSize, control.Height);

                control.Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);
            }

            else
            {
                Rectangle fillRect;

                if (control.VerticalAlignment == VerticalAlignment.Top)
                    fillRect = new Rectangle(0, 0, control.Width, control.fillSize);
                else
                    fillRect = new Rectangle(0, control.Height - control.fillSize, control.Width, control.fillSize);

                control.Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);
            }

            control.IsDirty = false;
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new ProgressBarTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                Foreground = Foreground.Clone(),
                Background = Background.Clone()
            };
        }
    }
}
