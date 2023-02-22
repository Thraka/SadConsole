using SadConsole.UI.Themes;

namespace SadConsole.UI.Controls;

public class TabItem
{
	public string Header { get; private set; }
	public ScreenSurface Console { get; private set; }

	internal Button TabButton { get; private set; }

	public TabItem(string _header, ScreenSurface _console)
	{
		Header = _header;
		Console = _console;
		TabButton = new(Header.Length)
		{
			Text = Header,
			IsVisible = false
		};

		((ButtonTheme)TabButton.Theme).ShowEnds = false;
	}

	public override string ToString() => Header;
}
