using System.Runtime.Serialization;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The base class for a theme.
    /// </summary>
    [DataContract]
    public abstract class ThemeBase
    {
        /// <summary>
        /// The colors last used when <see cref="RefreshTheme(Colors, ControlBase)"/> was called.
        /// </summary>
        protected Colors _colorsLastUsed;

        /// <summary>
        /// Default theme state of the whole control.
        /// </summary>
        [DataMember]
        public ThemeStates ControlThemeState { get; set; } = new ThemeStates();

        /// <summary>
        /// Draws the control state to the control.
        /// </summary>
        /// <param name="control">The control to draw.</param>
        /// <param name="time">The time since the last update frame call.</param>
        public abstract void UpdateAndDraw(ControlBase control, System.TimeSpan time);

        /// <summary>
        /// Called when the theme is attached to a control.
        /// </summary>
        /// <param name="control">The control that will use this theme instance.</param>
        public virtual void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = SadRogue.Primitives.Color.Transparent
            };
            control.Surface.Clear();
        }

        /// <summary>
        /// Creates a new theme instance based on the current instance.
        /// </summary>
        /// <returns>A new theme instance.</returns>
        public abstract ThemeBase Clone();


        /// <summary>
        /// Reloads the theme values based on the colors provided.
        /// </summary>
        /// <param name="colors">The colors to create the theme with.</param>
        /// <param name="control">The control being drawn with the theme.</param>
        public virtual void RefreshTheme(Colors colors, ControlBase control)
        {
            _colorsLastUsed = colors ?? control.FindThemeColors();

            ControlThemeState.RefreshTheme(_colorsLastUsed);
        }
    }
}
