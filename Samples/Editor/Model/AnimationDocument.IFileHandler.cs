using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal partial class AnimationDocument
{
    bool IFileHandler.SupportsLoad => true;

    bool IFileHandler.SupportsSave => true;

    string IFileHandler.FriendlyName => "Editor Document";

    string[] IFileHandler.ExtensionsLoading => ["sadsurface"]; 

    string[] IFileHandler.ExtensionsSaving => ["sadsurface"];

    public string HelpInformation => "Saves just the surface document.";

    object IFileHandler.Load(string file) =>
        Serializer.Load<AnimationDocument>(file, file.EndsWith('z'));

    bool IFileHandler.Save(object instance, string file)
    {
        if (!file.EndsWith(((IFileHandler)this).ExtensionsSaving[0], StringComparison.InvariantCulture))
            file += "." + ((IFileHandler)this).ExtensionsSaving[0];

        if (instance is AnimationDocument surface)
        {
            try
            {
                Serializer.Save(surface, file, false);

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
