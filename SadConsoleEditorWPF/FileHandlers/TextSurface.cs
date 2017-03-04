using System;
using SadConsole.Consoles;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    class TextSurface : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "surface" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Single Surface";
            }
        }

        public object Load(string file)
        {
            return SadConsole.Consoles.TextSurface.Load(file);
        }

        public void Save(object surface, string file)
        {
            ((SadConsole.Consoles.TextSurface)surface).Save(file);
        }
    }
}
