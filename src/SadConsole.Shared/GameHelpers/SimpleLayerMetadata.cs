using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Surfaces
{
    [DataContract]
    public class LayerMetadata
    {
        [DataMember]
        public string Name;
        [DataMember]
        public bool IsMoveable;
        [DataMember]
        public bool IsRemoveable;
        [DataMember]
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
