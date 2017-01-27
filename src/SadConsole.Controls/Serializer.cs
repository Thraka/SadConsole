namespace SadConsole.Controls
{
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Common serialization tasks for SadConsole.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// The types commonly used when sesrializing a <see cref="ControlsConsole"/> or a <see cref="Window"/>
        /// </summary>
        public static IEnumerable<System.Type> ControlTypes
        {
            get
            {
                return (new System.Type[] { typeof(ControlsConsole), typeof(Window), 
                    typeof(Controls.Button), 
                    typeof(Controls.ControlBase), 
                    typeof(Controls.DrawingSurface), 
                    typeof(Controls.InputBox), 
                    typeof(Controls.ListBox), 
                    typeof(Controls.ListBoxItem), 
                    typeof(Controls.ListBoxItemColor), 
                    typeof(Controls.RadioButton), 
                    typeof(Controls.ScrollBar), 
                    typeof(Controls.SelectionButton),

                    typeof(Themes.Library),
                    typeof(Themes.ButtonTheme),
                    typeof(Themes.InputBoxTheme),
                    typeof(Themes.ListBoxTheme),
                    typeof(Themes.RadioButtonTheme),
                    typeof(Themes.ScrollBarTheme),
                    typeof(Themes.ThemePartBase),
                    typeof(Themes.ThemePartClickable),
                    typeof(Themes.ThemePartSelected),
                    typeof(Themes.WindowTheme)
                    }).Union(SadConsole.Serializer.ConsoleTypes);
            }
        }
    }
}
