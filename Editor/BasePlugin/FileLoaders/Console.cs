using System;
using SadConsole;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    public class Console : IFileLoader
    {

        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "console", "consolez" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Console";
            }
        }

        public string Id => "CONSOLE";

        public object Load(string file)
        {
            return SadConsole.Serializer.Load<SadConsole.Console>(file, file.EndsWith('z'));
        }

        public bool Save(object surface, string file)
        {
            try
            {
                SadConsole.Serializer.Save<SadConsole.Console>((SadConsole.Console)surface, file, file.EndsWith('z'));
                return true;
            }
            catch (Exception e)
            {
                SadConsoleEditor.MainConsole.Instance.ShowSaveErrorPopup();
                return false;
            }
        }
    }
}
