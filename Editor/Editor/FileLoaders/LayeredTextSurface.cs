using System;
using SadConsole.Surfaces;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    class LayeredSurface : IFileLoader
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
            var surface = SadConsole.Surfaces.Layered.Load(file, typeof(LayerMetadata));
            int i = 0;
            foreach (var layer in surface.GetLayers())
            {
                if (layer.Metadata == null)
                    LayerMetadata.Create($"layer{i}", true, i != 0, true, layer);

                i++;
            }
            return surface;
        }

        public void Save(object surface, string file)
        {
            ((SadConsole.Surfaces.Layered)surface).Save(file, typeof(LayerMetadata));
        }
    }
}
