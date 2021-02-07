using System;
using SadConsole.Surfaces;
using System.Linq;
using System.IO;

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
            var surfaceFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".surface");

            if (File.Exists(surfaceFile))
            {
                var surface = SadConsole.Surfaces.Layered.Load(surfaceFile, typeof(LayerMetadata));

                return SadConsole.GameHelpers.Scene.Load(file, surface, new SadConsole.Renderers.LayeredSadConsole.Renderers.Basic());
            }
            else
                throw new Exception("A .surface file is missing. It should be the same name as the scene.");
        }

        public void Save(object surface, string file)
        {
            ((SadConsole.Surfaces.Layered)((SadConsole.GameHelpers.Scene)surface).Surface.TextSurface).Save(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".surface"), typeof(LayerMetadata));
            ((SadConsole.GameHelpers.Scene)surface).Save(file);
        }
    }
}
