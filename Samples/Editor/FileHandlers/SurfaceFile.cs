using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.FileHandlers;
internal class SurfaceFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public string FriendlyName => "Surface";

    public string[] ExtensionsLoading => ["surface", "surfacez"];

    public string[] ExtensionsSaving => ["surface"];

    public string HelpInformation => "Saves just the surface, without any other metadata, such as the document title.";

    public object Load(string file) =>
        Serializer.Load<ScreenSurface>(file, file.EndsWith('z'));

    public bool Save(object instance, string file)
    {
        if (instance is Model.IDocumentSurface)
            instance = ((Model.IDocumentSurface)instance).Surface;

        if (!file.EndsWith(ExtensionsSaving[0], StringComparison.InvariantCulture))
            file += "." + ExtensionsSaving[0];

        if (instance is ScreenSurface surface)
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
