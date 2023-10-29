using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Contains the header and content of a tab for the <see cref="TabControl"/>.
/// </summary>
public class TabItem
{
    /// <summary>
    /// Display text of the header. Also functions as the access key
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    /// Temporary variable holding the theme state of the tab item header.
    /// </summary>
    public ThemeStates ThemeHeaderStates { get; set; }

    /// <summary>
    /// Temporary variable holding where on the tab control the header is drawn.
    /// </summary>
    public Rectangle ThemeHeaderArea { get; set; }

    /// <summary>
    /// Temporary variable holding where on the tab control the mouse is tracked for this header.
    /// </summary>
    public Rectangle ThemeHeaderMouseArea { get; set; }

    /// <summary>
    /// The content of this tab item.
    /// </summary>
    public Panel Content { get; set; }

    /// <summary>
    /// The size in cells to allocate for displaying the header. <code>-1</code> indicates that the tab should be automatically sized to fit the size of the <see cref="Header"/>.
    /// </summary>
    public int TabSize { get; set; }

    /// <summary>
    /// Padding to add around the <see cref="Header"/> text when <see cref="TabSize"/> is unset (-1).
    /// </summary>
    public int AutomaticPadding { get; set; }

    /// <summary>
    /// If <see cref="TabSize"/> is any value other than <code>-1</code>, the <see cref="Header"/> is aligned according to this property.
    /// </summary>
    /// <value></value>
    public HorizontalAlignment TextAlignment { get; set; }

    /// <summary>
    /// Creates a new tab item with the specified header as a colored string, and sets the content for the tab.
    /// </summary>
    /// <param name="header">The header to display on this tab item.</param>
    /// <param name="content">The panel content to display for this tab.</param>
    /// <exception cref="System.ArgumentException">Thrown when the <paramref name="header"/> value is an empty string.</exception>
    public TabItem(string header, Panel content)
    {
        if (string.IsNullOrEmpty(header)) throw new System.ArgumentException("Header text cannot be empty.", nameof(header));

        Header = header;
        ThemeHeaderStates = new ThemeStates();
        TextAlignment = HorizontalAlignment.Center;
        TabSize = -1;
        AutomaticPadding = 1;
        Content = content;
    }
}
