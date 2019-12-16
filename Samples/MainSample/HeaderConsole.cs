using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo
{
    [System.Diagnostics.DebuggerDisplay("Header Area")]
    internal class HeaderConsole : ScreenSurface
    {
        public HeaderConsole() : base(80, 2)
        {
            DefaultBackground = Color.Transparent;
            DefaultForeground = SadConsole.UI.Themes.Library.Default.Colors.Yellow;
        }

        public void SetConsole(string title, string summary)
        {
            this.Fill(SadConsole.UI.Themes.Library.Default.Colors.Yellow, SadConsole.UI.Themes.Library.Default.Colors.GrayDark, 0);
            this.Print(1, 0, title.ToUpper(), SadConsole.UI.Themes.Library.Default.Colors.Yellow);
            this.Print(1, 1, summary, SadConsole.UI.Themes.Library.Default.Colors.Gray);
            //this.Print(0, 2, new string((char)223, 80), Theme.GrayDark, Color.Transparent);
        }
    }
}
