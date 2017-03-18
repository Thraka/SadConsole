using System;
using SadConsole.Consoles;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    class LayeredTextSurface : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "console" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Layered Surface";
            }
        }

        public object Load(string file)
        {
            return SadConsole.Consoles.LayeredTextSurface.Load(file, typeof(LayerMetadata));
        }

        public void Save(object surface, string file)
        {
            ((SadConsole.Consoles.LayeredTextSurface)surface).Save(file, typeof(LayerMetadata));
        }
    }
}
