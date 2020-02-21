using System.Runtime.Serialization;
using SadConsole.Controls;

namespace SadConsole.Themes
{

    /// <summary> 
    /// The base class for a theme.
    /// </summary>
    [DataContract]
    public abstract class ThemeBase : ThemeStates
    {
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
        public virtual void Attached(ControlBase control) { }

        /// <summary>
        /// Creates a new theme instance based on the current instance.
        /// </summary>
        /// <returns>A new theme instance.</returns>
        public new abstract ThemeBase Clone();
    }
}
