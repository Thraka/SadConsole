using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.FileHandlers;
internal class AnimationFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public string FriendlyName => "Export Animation";

    public string[] ExtensionsLoading => ["anim"];

    public string[] ExtensionsSaving => ["anim"];

    public string HelpInformation => "Exports the animation data and saves it to a compressed file.";

    public object Load(string file) =>
        Serializer.Load<AnimatedScreenObject>(file, true);

    public bool Save(object instance, string file)
    {
        if (instance is not AnimatedScreenObject)
        {
            ImGuiCore.Alert($"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}");
            return false;
        }

        if (!file.EndsWith(ExtensionsSaving[0], StringComparison.InvariantCulture))
            file += "." + ExtensionsSaving[0];

        try
        {
            Serializer.Save(instance, file, true);

            return true;
        }
        catch (Exception e)
        {
            ImGuiCore.Alert($"Unable to save file.\r\n\r\n{e.Message}");
            return false;
        }
    }
}
