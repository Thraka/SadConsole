#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using SadConsole.Controls;

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

        /// <summary>
        /// The current appearance based on the control state.
        /// </summary>
        public Cell Appearance { get; protected set; }

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();

            base.Attached(control);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is DrawingSurface drawingSurface))
            {
                return;
            }

            RefreshTheme(control.ThemeColors, control);

            if (!UseNormalStateOnly)
            {
                if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                {
                    Appearance = Disabled;
                }
                else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) ||
                         Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                {
                    Appearance = MouseDown;
                }
                else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                {
                    Appearance = MouseOver;
                }
                else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                {
                    Appearance = Focused;
                }
                else
                {
                    Appearance = Normal;
                }
            }
            else
            {
                Appearance = Normal;
            }

            drawingSurface?.OnDraw(drawingSurface);
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new DrawingSurfaceTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            UseNormalStateOnly = UseNormalStateOnly
        };
    }
}
