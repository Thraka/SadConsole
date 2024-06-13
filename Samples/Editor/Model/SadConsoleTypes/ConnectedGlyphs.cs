using System.Linq;

namespace SadConsole.Editor.Model.SadConsoleTypes;

internal class ConnectedGlyphs
{
    public static string[] _names = { "Empty", "Thin", "Thin Extended", "Thick", "3D" };
    public static int[][] _values = { ICellSurface.ConnectedLineEmpty, ICellSurface.ConnectedLineThin, ICellSurface.ConnectedLineThinExtended, ICellSurface.ConnectedLineThick, ICellSurface.Connected3dBox };

    public static string[] Names => _names;

    public static int[] GetValueFromIndex(int index) =>
        _values[index];

    public static int GetIndexFromValue(int[] value)
    {
        for (int i = 0; i < _values.Length; i++)
        {
            if (_values[i].SequenceEqual(value))
                return i;
        }

        return -1;
    }
}
