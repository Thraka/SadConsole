using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class AnimatedDocument: IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Animation Document";

    public string[] ExtensionsLoading => ["animdoc"];

    public string[] ExtensionsSaving => ["animdoc"];

    public string HelpInformation => "Saves or loads the animation document.";

    public object? GetSerializedObject(object instance)
    {
        if (instance is DocumentAnimated doc)
        {
            return new Serialization.AnimatedDocumentSerialized()
            {
                Title = doc.Title,
                Animation = doc._baseAnimation,
                SurfaceFont = SerializedTypes.FontSerialized.FromFont(doc.EditingSurfaceFont),
                SurfaceFontSize = doc.EditingSurfaceFontSize,
                EditorFontSize = doc.EditorFontSize,
                Options = doc.Options
            };
        }
        return null;
    }

    public object? CreateFromSerialized(object serializedObject)
    {
        if (serializedObject is Serialization.AnimatedDocumentSerialized serializedObj)
        {
            var doc = new DocumentAnimated(serializedObj.Animation);
            doc.Title = serializedObj.Title;
            doc.EditingSurfaceFont = SerializedTypes.FontSerialized.ToFont(serializedObj.SurfaceFont);
            doc.EditingSurfaceFontSize = serializedObj.SurfaceFontSize;
            doc.EditorFontSize = serializedObj.EditorFontSize;
            doc.EditingSurface.Font = doc.EditingSurfaceFont;
            doc.EditingSurface.FontSize = doc.EditingSurfaceFontSize;
            doc.Options = serializedObj.Options ?? new Serialization.DocumentOptions();
            doc.SyncToolModes();
            doc.RefreshDuration();
            doc.SetFrameIndex(0);
            doc.Resync();
            return doc;
        }
        return null;
    }

    public object? Load(string file)
    {
        DocumentAnimated? doc = null;

        // New format
        if (Serializer.TryLoad(file, true, out Serialization.AnimatedDocumentSerialized? serializedObj))
        {
            doc = CreateFromSerialized(serializedObj) as DocumentAnimated;
        }
        else
        {
            // Old format, uncompressed
            if (!Serializer.TryLoad<DocumentAnimated>(file, false, out doc))
            {
                // old format, compressed
                Serializer.TryLoad<DocumentAnimated>(file, true, out doc);
            }
        }

        if (doc == null)
        {
            MessageWindow.Show($"Unable to load file.", "Error");
            return null;
        }

        doc.RefreshDuration();
        doc.SetFrameIndex(0);

        doc.TryLoadPalette(file + ".pal");

        if (doc is IDocumentSimpleObjects docObjs)
            docObjs.TryLoadObjects(file + ".objs");

        return doc;
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentAnimated doc)
        {
            try
            {
                var convertedObj = GetSerializedObject(doc) as Serialization.AnimatedDocumentSerialized;

                if (convertedObj == null)
                {
                    MessageWindow.Show($"Unable to serialize document.", "Error");
                    return false;
                }

                Serializer.Save(convertedObj, file, true);

                if (doc.HasPalette)
                    doc.SavePalette(file + ".pal");

                if (doc is IDocumentSimpleObjects docObjs && docObjs.SimpleObjects.Count != 0)
                    docObjs.SaveObjects(file + ".objs");

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
