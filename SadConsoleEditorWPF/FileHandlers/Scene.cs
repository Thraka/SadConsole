using System;
using SadConsole.Consoles;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    class Scene : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "scene", "layers" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Scene";
            }
        }

        public object Load(string file)
        {
            if (System.IO.Path.GetExtension(file) == ".scene")
            {

            }

            return SadConsole.Game.Scene.Load(file, null, typeof(LayerMetadata));
        }

        public void Save(object surface, string file)
        {
            ((SadConsole.Game.Scene)surface).Save(file, typeof(LayerMetadata));
        }
    }
}
