namespace SadConsole.UI
{
    /// <summary>
    /// Registers the controls in SadConsole.Extended with a library.
    /// </summary>
    public static class RegistrarExtended
    {
        /// <summary>
        /// Registers the controls with the <see cref="Themes.Library.Default"/> library.
        /// </summary>
        public static void Register() =>
            Register(Themes.Library.Default);

        /// <summary>
        /// Registers the controls with the provided library.
        /// </summary>
        /// <param name="library"></param>
        public static void Register(Themes.Library library)
        {
            library.SetControlTheme(typeof(Controls.ColorBar), typeof(Themes.ColorBar));
            library.SetControlTheme(typeof(Controls.ColorPicker), typeof(Themes.ColorPicker));
            library.SetControlTheme(typeof(Controls.HueBar), typeof(Themes.HueBar));
            library.SetControlTheme(typeof(Controls.FileDirectoryListbox), typeof(Themes.ListBoxTheme));
            library.SetControlTheme(typeof(Controls.CharacterPicker), typeof(Themes.CharacterPicker));
        }
    }
}
