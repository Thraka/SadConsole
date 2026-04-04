namespace SadConsole.Editor.Addins;

/// <summary>
/// A menu item contributed by an addin. Rendered in the top menu bar by GuiTopBar.
/// Follows the StatusItems pattern — a simple data record processed additively.
/// </summary>
/// <param name="Menu">Top-level menu name (e.g. "Tilemap", "My Addin").
/// Items with the same Menu name are grouped under one dropdown.</param>
/// <param name="Label">The menu item label (e.g. "Export as PNG").</param>
/// <param name="OnClick">Action invoked when the menu item is clicked.</param>
public record AddinMenuItem(string Menu, string Label, Action OnClick);
