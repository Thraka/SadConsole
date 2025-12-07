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

    public object? Load(string file)
    {
        bool loaded = false;
        DocumentAnimated? doc;
        Serialization.AnimatedDocumentSerialized? serializedObj;

        // New format
        if (Serializer.TryLoad(file, true, out serializedObj))
        {
            loaded = true;
            doc = new DocumentAnimated(serializedObj.Animation);
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
        }

        else
        {
            // Old format, uncompressed
            loaded = Serializer.TryLoad<DocumentAnimated>(file, false, out doc);

            // old format, compressed
            if (!loaded)
                loaded = Serializer.TryLoad<DocumentAnimated>(file, true, out doc);
        }

        if (!loaded)
        {
            MessageWindow.Show($"Unable to load file.", "Error");
            return null;
        }

        doc!.RefreshDuration();
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
                Serialization.AnimatedDocumentSerialized convertedObj = new()
                {
                    Title = doc.Title,
                    Animation = doc._baseAnimation,
                    SurfaceFont = SerializedTypes.FontSerialized.FromFont(doc.EditingSurfaceFont),
                    SurfaceFontSize = doc.EditingSurfaceFontSize,
                    EditorFontSize = doc.EditorFontSize,
                    Options = doc.Options
                };

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
