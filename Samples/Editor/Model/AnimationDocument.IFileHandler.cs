using SadConsole.Editor.FileHandlers;

namespace SadConsole.Editor.Model;

internal partial class AnimationDocument
{
    bool IFileHandler.SupportsLoad => true;

    bool IFileHandler.SupportsSave => true;

    string IFileHandler.FriendlyName => "Animation Document";

    string[] IFileHandler.ExtensionsLoading => ["animdoc"]; 

    string[] IFileHandler.ExtensionsSaving => ["animdoc"];

    public string HelpInformation => "Saves the editor document.";

    object IFileHandler.Load(string file) =>
        Serializer.Load<AnimationDocument>(file, true);

    bool IFileHandler.Save(object instance, string file)
    {
        if (!file.EndsWith(((IFileHandler)this).ExtensionsSaving[0], StringComparison.InvariantCulture))
            file += "." + ((IFileHandler)this).ExtensionsSaving[0];

        if (instance is AnimationDocument surface)
        {
            try
            {
                Serializer.Save(surface, file, true);

                return true;
            }
            catch (Exception e)
            {
                ImGuiCore.Alert($"Unable to save file.\r\n\r\n{e.Message}");
                return false;
            }
        }

        ImGuiCore.Alert($"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}");
        return false;
    }
}
