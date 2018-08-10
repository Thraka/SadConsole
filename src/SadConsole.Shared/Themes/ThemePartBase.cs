using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Has the basic appearances of each control state.
    /// </summary>
    [DataContract]
    public class ThemeStates
    {
        /// <summary>
        /// The normal appearance of the control.
        /// </summary>
        [DataMember]
        public Cell Normal;

        /// <summary>
        /// The appearance of the control when it is disabled.
        /// </summary>
        [DataMember]
        public Cell Disabled;

        /// <summary>
        /// The appearance of the control when it is focused.
        /// </summary>
        [DataMember]
        public Cell Focused;

        /// <summary>
        /// The appearence of the control when it is in a selected state.
        /// </summary>
        [DataMember]
        public Cell Selected;

        /// <summary>
        /// The appearance of the control when the mouse is over it.
        /// </summary>
        [DataMember]
        public Cell MouseOver;

        /// <summary>
        /// THe appearance of the control when a mouse button is held down.
        /// </summary>
        [DataMember]
        public Cell MouseDown;

        /// <summary>
        /// Creates a new instance of the theme states object.
        /// </summary>
        public ThemeStates()
        {
            Normal = Library.Default.Appearance_ControlNormal;
            Disabled = Library.Default.Appearance_ControlDisabled;
            MouseOver = Library.Default.Appearance_ControlOver;
            MouseDown = Library.Default.Appearance_ControlMouseDown;
            Selected = Library.Default.Appearance_ControlSelected;
            Focused = Library.Default.Appearance_ControlFocused;
        }
    }


    /// <summary>
    /// The base class for a theme.
    /// </summary>
    [DataContract]
    public abstract class ThemeBase<T> : ThemeStates
        where T : ControlBase
    {
        /// <summary>
        /// Draws the control state to the control.
        /// </summary>
        /// <param name="control">The control to draw.</param>
        /// <param name="hostSurface">The surface the control renders to.</param>
        public abstract void Draw(T control, SurfaceBase hostSurface);

        /// <summary>
        /// Defaults the base properties to the library.
        /// </summary>
        protected ThemeBase() { }
    }
}
