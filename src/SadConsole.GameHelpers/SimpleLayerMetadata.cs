using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Surfaces
{
    public class LayerMetadata
    {
        public string Name;
        public bool IsMoveable;
        public bool IsRemoveable;
        public bool IsRenamable;

        public static LayerMetadata Create(string name, bool moveable, bool removeable, bool renamable, LayeredSurface.Layer layer)
        {
            LayerMetadata meta = new LayerMetadata();
            meta.Name = name;
            meta.IsMoveable = moveable;
            meta.IsRenamable = renamable;
            meta.IsRemoveable = removeable;
            layer.Metadata = meta;
            return meta;
        }
    }
}
