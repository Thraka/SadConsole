using SadConsole.UI;

namespace SadConsole.Editor.Model;

public class Palette
{
    public NamedColor[] Colors;

    public void Load(string file)
    {
        NamedColorCollection items = NamedColorCollection.Load(file);

        if (items.Colors.Count == 0)
            LoadDefault();
        else
            Colors = [.. items.Colors.Values]; 
    }

    public Palette() =>
        LoadDefault();

    private void LoadDefault() =>
        Colors = [new ("White", Color.White), new("Black", Color.Black)];
}
