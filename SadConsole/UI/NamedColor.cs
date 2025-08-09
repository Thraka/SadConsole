using System.Collections;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.UI;

/// <summary>
/// A color associated with a name.
/// </summary>
/// <param name="Name">The name of the color.</param>
/// <param name="Color">The color.</param>
public record NamedColor (string Name, Color Color);

/// <summary>
/// A collection of named colors
/// </summary>
public class NamedColorCollection : IEnumerable<NamedColor>
{
    /// <summary>
    /// The backing dictionary of colors.
    /// </summary>
    public Dictionary<string, NamedColor> Colors { get; set; } = new();

    /// <summary>
    /// Adds a color to the collection.
    /// </summary>
    /// <param name="color"></param>
    public void Add(NamedColor color) =>
        Colors[color.Name] = color;

    /// <summary>
    /// Creates a new instance with the default colors of White and Black.
    /// </summary>
    public NamedColorCollection() { }

    public NamedColorCollection(IEnumerable<NamedColor> initialCollection)
    {
        foreach (NamedColor color in initialCollection)
            Colors[color.Name] = color;
    }

    /// <summary>
    /// Clears the collection of colors and adds White and Black colors.
    /// </summary>
    public void LoadDefaults()
    {
        Colors.Clear();
        Colors.Add("White", new("White", Color.White));
        Colors.Add("Black", new("Black", Color.Black));
    }

    /// <summary>
    /// Loads a <see cref="Colors"/> dictionary from a file, and returns a new instance of the <see cref="NamedColorCollection"/> class with those colors.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <exception cref="System.IO.FileNotFoundException">Thrown when the file isn't found.</exception>
    public static NamedColorCollection Load(string file)
    {
        NamedColorCollection collection = new();

        if (!System.IO.File.Exists(file))
            throw new System.IO.FileNotFoundException(file);

        Dictionary<string, string> items = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(System.IO.File.ReadAllText(file))!;

        collection.Colors.Clear();
        if (items.Count > 0)
        {
            foreach (KeyValuePair<string, string> item in items)
                collection.Colors.Add(item.Key, new(item.Key, Color.White.FromParser(item.Value, out _, out _, out _, out _, out _)));
        }
        else
            collection.LoadDefaults();

        return collection;
    }

    /// <summary>
    /// Saves the <see cref="Colors"/> dictionary to a file.
    /// </summary>
    /// <param name="file">The file.</param>
    public void Save(string file)
    {
        Dictionary<string, string> colors = new(Colors.Count);

        foreach (KeyValuePair<string, NamedColor> color in Colors)
            colors.Add(color.Key, color.Value.Color.ToParser());

        System.IO.File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(colors));
    }

    public IEnumerator<NamedColor> GetEnumerator() =>
        Colors.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}
