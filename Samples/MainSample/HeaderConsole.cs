using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo
{
    internal class HeaderConsole : ScreenSurface
    {
        public HeaderConsole() : base(80, 2)
        {
            Surface.DefaultBackground = SadConsole.UI.Themes.Library.Default.Colors.GrayDark;
            Surface.DefaultForeground = SadConsole.UI.Themes.Library.Default.Colors.Yellow;
        }

        public void SetHeader(string title, string summary)
        {
            Surface.Clear();
            Surface.Print(1, 0, title.ToUpper());
            Surface.Print(1, 1, summary, SadConsole.UI.Themes.Library.Default.Colors.Gray);
            //this.Print(0, 2, new string((char)223, 80), Theme.GrayDark, Color.Transparent);
        }

        /// <summary>
        /// Returns the value "Header Surface".
        /// </summary>
        /// <returns>The string "Header Surface".</returns>
        public override string ToString() =>
            "Header Surface";
    }
}
