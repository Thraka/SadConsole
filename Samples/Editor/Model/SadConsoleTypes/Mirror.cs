namespace SadConsole.Editor.Model.SadConsoleTypes;

public static class Mirror
{
    private static string[] _names = Enum.GetNames<SadConsole.Mirror>();
    private static SadConsole.Mirror[] _values = Enum.GetValues<SadConsole.Mirror>();

    public static string[] Names => _names;

    public static SadConsole.Mirror GetValueFromIndex(int index) =>
        _values[index];

    public static int GetIndexFromValue(SadConsole.Mirror value) =>
        Array.IndexOf(_values, value);
        
}
