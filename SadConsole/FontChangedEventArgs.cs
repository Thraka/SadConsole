using System;

namespace SadConsole;

/// <summary>
/// Event arguments for when the default font changes.
/// </summary>
public class FontChangedEventArgs : EventArgs
{
    /// <summary>
    /// The font that was previously set as the default.
    /// </summary>
    public IFont OldFont { get; }

    /// <summary>
    /// The new font that is now set as the default.
    /// </summary>
    public IFont NewFont { get; }

    /// <summary>
    /// Creates a new instance of the font changed event arguments.
    /// </summary>
    /// <param name="oldFont">The previous default font.</param>
    /// <param name="newFont">The new default font.</param>
    public FontChangedEventArgs(IFont oldFont, IFont newFont)
    {
        OldFont = oldFont;
        NewFont = newFont;
    }
}
