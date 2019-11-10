#if !SILVERLIGHT && !WPF && !WINDOWS_PHONE
namespace SadConsole
{
    public enum HorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Stretch = 3,
    }

    public enum VerticalAlignment
    {
        Top = 0,
        Center = 1,
        Bottom = 2,
        Stretch = 3,
    }

    public enum Orientation
    {
        Vertical = 0,
        Horizontal = 1,
    }
}
#endif