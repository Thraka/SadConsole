using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;

namespace SadConsole.Editor.FileHandlers;

internal class LayeredSurfaceDocument : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Layered Surface Document";

    public string[] ExtensionsLoading => ["layereddoc"];

    public string[] ExtensionsSaving => ["layereddoc"];

    public string HelpInformation => "Saves or loads the layered surface document.";

    public object? Load(string file)
    {
        bool loaded = false;
        DocumentLayeredSurface? doc;

        if (Serializer.TryLoad(file, true, out LayeredSurfaceDocumentSerialized? serializedObj))
        {
            loaded = true;

            // Create the layered screen surface from the serialized layers
            LayeredScreenSurface layeredSurface = new(serializedObj.Layers[0]);

            for (int i = 1; i < serializedObj.Layers.Length; i++)
                layeredSurface.Layers.Add(serializedObj.Layers[i]);

            // Restore layer visibility
            if (serializedObj.LayerVisibility != null)
            {
                for (int i = 0; i < serializedObj.LayerVisibility.Length && i < layeredSurface.Layers.Count; i++)
                    layeredSurface.Layers.SetLayerVisibility(i, serializedObj.LayerVisibility[i]);
            }

            doc = new DocumentLayeredSurface(layeredSurface);
            doc.Title = serializedObj.Title;
            doc.EditingSurfaceFont = SerializedTypes.FontSerialized.ToFont(serializedObj.SurfaceFont);
            doc.EditingSurfaceFontSize = serializedObj.SurfaceFontSize;
            doc.EditorFontSize = serializedObj.EditorFontSize;
            doc.EditingSurface.Font = doc.EditingSurfaceFont;
            doc.EditingSurface.FontSize = doc.EditingSurfaceFontSize;
            doc.Options = serializedObj.Options ?? new DocumentOptions();

            if (doc.Options.UseZones && serializedObj.Zones != null)
            {
                foreach (ZoneSerialized item in serializedObj.Zones)
                    doc.Zones.Objects.Add((ZoneSimplified)item);
            }

            if (doc.Options.UseSimpleObjects && serializedObj.SimpleObjects != null)
            {
                foreach (SimpleObjectDefinition item in serializedObj.SimpleObjects)
                    doc.SimpleObjects.Objects.Add(item);
            }

            doc.SyncToolModes();
            doc.Resync();
        }
        else
        {
            doc = null;
        }

        if (!loaded || doc == null)
        {
            MessageWindow.Show($"Unable to load file.", "Error");
            return null;
        }

        doc.TryLoadPalette(file + ".pal");

        if (doc is IDocumentSimpleObjects docObjs)
            docObjs.TryLoadObjects(file + ".objs");

        return doc;
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentLayeredSurface doc)
        {
            try
            {
                CellSurface[] layers = new CellSurface[doc.LayeredEditingSurface.Layers.Count];
                bool[] layerVisibility = new bool[doc.LayeredEditingSurface.Layers.Count];
                for (int i = 0; i < doc.LayeredEditingSurface.Layers.Count; i++)
                {
                    layers[i] = (CellSurface)doc.LayeredEditingSurface.Layers[i];
                    layerVisibility[i] = doc.LayeredEditingSurface.Layers.GetLayerVisibility(i);
                }

                LayeredSurfaceDocumentSerialized convertedObj = new()
                {
                    Title = doc.Title,
                    Layers = layers,
                    LayerVisibility = layerVisibility,
                    SurfaceFont = SerializedTypes.FontSerialized.FromFont(doc.EditingSurfaceFont),
                    SurfaceFontSize = doc.EditingSurfaceFontSize,
                    EditorFontSize = doc.EditorFontSize,
                    Options = doc.Options,
                    Zones = doc.Options.UseZones ? doc.Zones.Objects.Select(z => new ZoneSerialized
                    {
                        Name = z.Name,
                        ZoneArea = z.ZoneArea.ToArray(),
                        Appearance = z.Appearance,
                        Settings = z.Settings
                    }).ToArray() : null,
                    SimpleObjects = doc.Options.UseSimpleObjects ? [.. doc.SimpleObjects.Objects] : null
                };

                Serializer.Save(convertedObj, file, true);

                if (doc.HasPalette)
                    doc.SavePalette(file + ".pal");

                return true;
            }
            catch (Exception e)
            {
                MessageWindow.Show($"Unable to save file.\r\n\r\n{e.Message}", "Error");
                return false;
            }
        }

        MessageWindow.Show($"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}", "Error");

        return false;
    }
}
