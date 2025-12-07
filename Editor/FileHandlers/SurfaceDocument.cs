using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;

namespace SadConsole.Editor.FileHandlers;

internal class SurfaceDocument: IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Editor Document";

    public string[] ExtensionsLoading => ["surfdoc"];

    public string[] ExtensionsSaving => ["surfdoc"];

    public string HelpInformation => "Saves or loads the surface document.";

    public object? GetSerializedObject(object instance)
    {
        if (instance is DocumentSurface doc)
        {
            return new Serialization.SurfaceDocumentSerialized()
            {
                Title = doc.Title,
                Surface = (CellSurface)doc.EditingSurface.Surface,
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
        }
        return null;
    }

    public object? CreateFromSerialized(object serializedObject)
    {
        if (serializedObject is Serialization.SurfaceDocumentSerialized serializedObj)
        {
            var doc = new DocumentSurface(serializedObj.Surface);
            doc.Title = serializedObj.Title;
            doc.EditingSurfaceFont = SerializedTypes.FontSerialized.ToFont(serializedObj.SurfaceFont);
            doc.EditingSurfaceFontSize = serializedObj.SurfaceFontSize;
            doc.EditorFontSize = serializedObj.EditorFontSize;
            doc.EditingSurface.Font = doc.EditingSurfaceFont;
            doc.EditingSurface.FontSize = doc.EditingSurfaceFontSize;
            doc.Options = serializedObj.Options ?? new Serialization.DocumentOptions();

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
            return doc;
        }
        return null;
    }

    public object? Load(string file)
    {
        DocumentSurface? doc = null;

        // New format
        if (Serializer.TryLoad(file, true, out Serialization.SurfaceDocumentSerialized? serializedObj))
        {
            doc = CreateFromSerialized(serializedObj) as DocumentSurface;
        }
        else
        {
            // Old format, uncompressed
            if (!Serializer.TryLoad<DocumentSurface>(file, false, out doc))
            {
                // old format, compressed
                Serializer.TryLoad<DocumentSurface>(file, true, out doc);
            }
        }

        if (doc == null)
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

        if (instance is DocumentSurface doc)
        {
            try
            {
                var convertedObj = GetSerializedObject(doc) as Serialization.SurfaceDocumentSerialized;

                if (convertedObj == null)
                {
                    MessageWindow.Show($"Unable to serialize document.", "Error");
                    return false;
                }

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
