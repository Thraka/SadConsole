using System;

namespace SadConsole;

/// <summary>
/// Event arguments for when the default font size changes.
/// </summary>
public class FontSizeChangedEventArgs : EventArgs
{
    /// <summary>
    /// The font size that was previously set as the default.
    /// </summary>
    public IFont.Sizes OldSize { get; }

    /// <summary>
    /// The new font size that is now set as the default.
    /// </summary>
    public IFont.Sizes NewSize { get; }

    /// <summary>
    /// Creates a new instance of the font size changed event arguments.
    /// </summary>
    /// <param name="oldSize">The previous default font size.</param>
    /// <param name="newSize">The new default font size.</param>
    public FontSizeChangedEventArgs(IFont.Sizes oldSize, IFont.Sizes newSize)
    {
        OldSize = oldSize;
        NewSize = newSize;
    }
}
