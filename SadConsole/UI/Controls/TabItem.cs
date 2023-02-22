using SadConsole.UI.Themes;

namespace SadConsole.UI.Controls;

public class TabItem
{
	public string Header { get; private set; }
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

	public TabItem(string _header, ScreenSurface _console)
	{
		Header = _header;
		Console = _console;
		TabButton = new(Header.Length)
		{
			Text = Header,
			IsVisible = false
		};
		TextAlignment = HorizontalAlignment.Center;

		((ButtonTheme)TabButton.Theme).ShowEnds = false;
	}

	public override string ToString() => Header;
}
