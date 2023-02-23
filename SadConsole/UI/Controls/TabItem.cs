using SadConsole.UI.Themes;

namespace SadConsole.UI.Controls
{

    /// <summary>
    ///
    /// </summary>
    public class TabItem
    {
        /// <summary>
        /// Display text of the header. Also functions as the access key
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Reference to the content area.
        /// </summary>
        public ScreenSurface Console { get; private set; }

        /// <summary>
        /// The width/height of the tab. This is based on the theme:
        /// Width for horizontal theme, Height for vertical theme.
        /// If a TabSize value is given, but is smaller then the header length, the longest value will be used
        /// </summary>
        /// <value>Size in cells</value>
        public int TabSize { get; set; }

        /// <summary>
        /// If setting a custom width for the tab, this defines how the text will be alligned
        /// </summary>
        /// <value></value>
        public HorizontalAlignment TextAlignment { get; set; }

        internal Button TabButton { get; private set; }

        /// <summary>
        /// Constructs a new tab item
        /// </summary>
        /// <param name="_header">Display name of the tab</param>
        /// <param name="_console">ScreenSurface with the content</param>
        public TabItem(string _header, ScreenSurface _console)
        {
            Header = _header;
            Console = _console;

            if (Header.Length == 0 && TabSize == 0)
            {
                throw new System.ArgumentException("Either a header needs to be set, or a TabSize");
            }

            TabButton = new Button(Header.Length == 0 ? 1 : Header.Length)
            {
                Text = Header,
                IsVisible = false
            };
            TextAlignment = HorizontalAlignment.Center;

            ((ButtonTheme)TabButton.Theme).ShowEnds = false;
        }
    }
}
