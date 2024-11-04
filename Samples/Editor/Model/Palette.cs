namespace SadConsole.Editor.Model;

public class Palette
{
    public string[] Names;
    public string[] Values;
    public Color[] ParsedValues;

    public void Load(string file)
    {
        SerializedPalette items = System.Text.Json.JsonSerializer.Deserialize<SerializedPalette>(System.IO.File.ReadAllText(file));

        if (items.Colors.Count > 0)
        {

            Names = new string[items.Colors.Count];
            Values = new string[items.Colors.Count];
            ParsedValues = new Color[items.Colors.Count];

            for (int i = 0; i < items.Colors.Count; i++)
            {
                Names[i] = items.Colors.ElementAt(i).Key;
                Values[i] = items.Colors.ElementAt(i).Value;
                ParsedValues[i] = Color.White.FromParser(items.Colors.ElementAt(i).Value, out _, out _, out _, out _, out _);
            }

        }
        else
            LoadDefault();
    }

    public Palette() =>
        LoadDefault();

    private void LoadDefault()
    {
        Names = ["White", "Black"];
        Values = [Color.White.ToParser(), Color.Black.ToParser()];
        ParsedValues = [Color.White, Color.Black];
    }   

    public void Save(string file)
    {
        SerializedPalette items = new SerializedPalette();
        items.Colors = new Dictionary<string, string>();

        for (int i = 0; i < Names.Length; i++)
            items.Colors.Add(Names[i], Values[i]);

        System.IO.File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(items));
    }

    private class SerializedPalette
    {
        public Dictionary<string, string> Colors { get; set; }
    }
}
