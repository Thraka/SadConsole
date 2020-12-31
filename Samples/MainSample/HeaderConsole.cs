using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo
{
    internal class HeaderConsole : ScreenSurface
    {
        public HeaderConsole() : base(80, 2)
        {
            Surface.DefaultBackground = Color.Transparent;
            Surface.DefaultForeground = SadConsole.UI.Themes.Library.Default.Colors.Yellow;
        }

        public void SetConsole(string title, string summary)
        {
            Surface.Fill(SadConsole.UI.Themes.Library.Default.Colors.Yellow, SadConsole.UI.Themes.Library.Default.Colors.GrayDark, 0);
            Surface.Print(1, 0, title.ToUpper(), SadConsole.UI.Themes.Library.Default.Colors.Yellow);
            Surface.Print(1, 1, summary, SadConsole.UI.Themes.Library.Default.Colors.Gray);
            //this.Print(0, 2, new string((char)223, 80), Theme.GrayDark, Color.Transparent);
        }

        /// <summary>
        /// Returns the value "Window".
        /// </summary>
        /// <returns>The string "Window".</returns>
        public override string ToString() =>
            "Header Surface";
    }
}
