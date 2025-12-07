using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public interface IDocumentZones
{
    ImGuiList<ZoneSimplified> Zones { get; }

    public bool TryLoadZones(string file)
    {
        if (!File.Exists(file)) return false;

        Serialization.ZoneSerialized[] zones = Serializer.Load<Serialization.ZoneSerialized[]>(file, false);

        Zones.Objects.Clear();
        foreach (Serialization.ZoneSerialized zone in zones)
            Zones.Objects.Add(zone);

        return true;
    }

    public void SaveZones(string file)
    {
        Serializer.Save(Zones.Objects.ToArray(), file, false);
    }
}
