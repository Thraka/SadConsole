using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;

namespace SadConsole.Editor.FileHandlers;

/// <summary>
/// File handler for scene documents. Handles saving and loading scenes with embedded child documents.
/// </summary>
internal class SceneDocument : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Scene Document";

    public string[] ExtensionsLoading => ["scenedoc"];

    public string[] ExtensionsSaving => ["scenedoc"];

    public string HelpInformation => "Saves or loads a scene document with all embedded child documents.";

    /// <summary>
    /// Gets the appropriate file handler for a document type.
    /// </summary>
    private static IFileHandler? GetHandlerForDocumentType(string documentType) => documentType switch
    {
        "DocumentSurface" => new SurfaceDocument(),
        "DocumentLayeredSurface" => new LayeredSurfaceDocument(),
        "DocumentAnimated" => new AnimatedDocument(),
        _ => null
    };

    /// <summary>
    /// Gets the document type string for a document.
    /// </summary>
    private static string GetDocumentType(Document doc) => doc switch
    {
        DocumentSurface => KnownDocumentTypes.SurfaceDocument,
        DocumentLayeredSurface => KnownDocumentTypes.LayeredSurfaceDocument,
        DocumentAnimated => KnownDocumentTypes.AnimatedSurfaceDocument,
        _ => throw new NotSupportedException($"Document type {doc.GetType().Name} is not supported in scenes.")
    };

    public object? GetSerializedObject(object instance)
    {
        if (instance is DocumentScene doc)
        {
            var childrenSerialized = new List<SceneChildSerialized>();

            foreach (var child in doc.ChildSceneItems)
            {
                string docType = GetDocumentType(child.Document);
                var handler = GetHandlerForDocumentType(docType);
                
                if (handler == null)
                    continue;

                var serializedChild = handler.GetSerializedObject(child.Document);
                if (serializedChild == null)
                    continue;

                childrenSerialized.Add(new SceneChildSerialized
                {
                    DocumentType = docType,
                    Position = child.Position,
                    UsePixelPositioning = child.UsePixelPositioning,
                    Label = child.Label,
                    SerializedDocument = serializedChild,
                    Viewport = child.Viewport
                });
            }

            return new SceneDocumentSerialized
            {
                Title = doc.Title,
                Children = childrenSerialized.ToArray(),
                ScenePixelSize = doc.ScenePixelSize,
                Options = doc.Options,
                Metadata = doc.Metadata
            };
        }
        return null;
    }

    public object? CreateFromSerialized(object serializedObject)
    {
        if (serializedObject is SceneDocumentSerialized serializedObj)
        {
            var doc = new DocumentScene();
            doc.Title = serializedObj.Title;
            doc.ScenePixelSize = serializedObj.ScenePixelSize;

            if (serializedObj.Options != null)
            {
                doc.Options = serializedObj.Options;

                // Force these core options
                doc.Options.UseToolsWindow = true;
                doc.Options.ToolsWindowShowToolsList = false;
                doc.Options.DisableScrolling = false;
            }

            if (serializedObj.Metadata != null)
                doc.Metadata = serializedObj.Metadata;

            if (serializedObj.Children != null)
            {
                foreach (var childSerialized in serializedObj.Children)
                {
                    var handler = GetHandlerForDocumentType(childSerialized.DocumentType);
                    if (handler == null || childSerialized.SerializedDocument == null)
                        continue;

                    var childDoc = handler.CreateFromSerialized(childSerialized.SerializedDocument) as Document;
                    if (childDoc == null)
                        continue;

                    // Use AddChildDocument to properly set Parent relationship
                    var sceneChild = doc.AddChildDocument(childDoc);
                    sceneChild.Position = childSerialized.Position;
                    sceneChild.UsePixelPositioning = childSerialized.UsePixelPositioning;
                    sceneChild.Label = childSerialized.Label;
                    sceneChild.Viewport = childSerialized.Viewport;

                    // Apply position to the document's surface
                    childDoc.EditingSurface.Position = sceneChild.Position;
                    childDoc.EditingSurface.UsePixelPositioning = sceneChild.UsePixelPositioning;
                }
            }

            return doc;
        }
        return null;
    }

    public object? Load(string file)
    {
        if (!Serializer.TryLoad(file, true, out SceneDocumentSerialized? serializedObj))
        {
            MessageWindow.Show("Unable to load scene file.", "Error");
            return null;
        }

        var doc = CreateFromSerialized(serializedObj) as DocumentScene;
        
        if (doc == null)
        {
            MessageWindow.Show("Unable to reconstruct scene from file.", "Error");
            return null;
        }

        doc.TryLoadPalette(file + ".pal");

        return doc;
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentScene doc)
        {
            try
            {
                var serializedObj = GetSerializedObject(doc) as SceneDocumentSerialized;
                
                if (serializedObj == null)
                {
                    MessageWindow.Show("Unable to serialize scene.", "Error");
                    return false;
                }

                Serializer.Save(serializedObj, file, true);

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
