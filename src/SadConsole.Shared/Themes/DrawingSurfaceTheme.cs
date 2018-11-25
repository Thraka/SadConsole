using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    /// <summary>
    /// A basic theme for a drawing surface that simply fills the surface based on the state.
    /// </summary>
    public class DrawingSurfaceTheme : ThemeBase
    {
        /// <summary>
        /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
        /// </summary>
        public bool UseNormalStateOnly { get; set; } = true;

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);

            base.Attached(control);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell appearance;

            if (!UseNormalStateOnly)
            {
                if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                    appearance = Disabled;

                else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) ||
                         Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                    appearance = MouseDown;

                else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                    appearance = MouseOver;

                else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                    appearance = Focused;

                else
                    appearance = Normal;
            }
            else
                appearance = Normal;

            control.Surface.Fill(appearance.Foreground, appearance.Background, null);
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new DrawingSurfaceTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
            };
        }
    }
}
