using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.FileHandlers;
internal class SurfaceFileCompressed : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public string FriendlyName => "Surface (Compressed)";

    public string[] ExtensionsLoading => ["surfacez"];

    public string[] ExtensionsSaving => ["surfacez"];

    public string HelpInformation => "Saves just the surface, without any other metadata, such as the document title. The file is compressed with gzip.";

    public object Load(string file) =>
        Serializer.Load<ScreenSurface>(file, true);

    public bool Save(object instance, string file)
    {
        if (instance is ScreenSurface surface)
        {
            try
            {
                Serializer.Save<ScreenSurface>(surface, file, true);

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
