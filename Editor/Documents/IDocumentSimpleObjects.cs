using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public interface IDocumentSimpleObjects
{
    ImGuiList<SimpleObjectDefinition> SimpleObjects { get; }

    public bool TryLoadObjects(string file)
    {
        if (!File.Exists(file)) return false;

        SimpleObjectDefinition[] simpleObjects = Serializer.Load<SimpleObjectDefinition[]>(file, false);

        SimpleObjects.Objects.Clear();
        foreach (SimpleObjectDefinition simpleObject in simpleObjects)
            SimpleObjects.Objects.Add(simpleObject);

        return true;
    }

    public void SaveObjects(string file)
    {
        Serializer.Save(SimpleObjects.Objects.ToArray(), file, false);
    }
}
