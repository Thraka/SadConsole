using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a button that can be toggled on/off within a group of other buttons.
/// </summary>
[DataContract]
public partial class ToggleSwitch : ToggleButtonBase
{
    /// <summary>
    /// Creates a new checkbox control with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="height">The height of the control.</param>
    public ToggleSwitch(int width, int height) : base(width, height)
    {
        OnGlyph = 178;
        OnGlyphColor = Color.LawnGreen;
        BackgroundGlyph = 177;
        OffGlyphColor = Color.DarkGreen;
        SwitchOrientation = HorizontalAlignment.Right;
    }

    /// <summary>
    /// Perfroms a click on the base button and also toggles the <see cref="ToggleButtonBase.IsSelected"/> property.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();
        IsSelected = !IsSelected;
    }


}
