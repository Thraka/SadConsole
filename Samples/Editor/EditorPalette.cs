using System.Diagnostics.CodeAnalysis;
using SadConsole.UI;

namespace SadConsole.Editor;

/// <summary>
/// This class essentially wraps NamedColorCollection to make it easy to use with ImGui.
/// </summary>
public class EditorPalette
{
    public NamedColor[] Colors;

    public EditorPalette() =>
        LoadDefault();

    private EditorPalette(string file)
    {
        NamedColorCollection items = NamedColorCollection.Load(file);

        if (items.Colors.Count == 0)
            LoadDefault();
        else
            Colors = [.. items.Colors.Values];
    }

    [MemberNotNull(nameof(Colors))]
    private void LoadDefault() =>
        Colors = [new NamedColor("White", Color.White), new NamedColor("Black", Color.Black)];

    public void Save(string file)
    {
        if (File.Exists(file))
            File.Delete(file);

        new NamedColorCollection(Colors).Save(file);
    }

    public static EditorPalette Load(string file) =>
        new EditorPalette(file);
}
