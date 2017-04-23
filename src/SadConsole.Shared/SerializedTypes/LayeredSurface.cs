using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class LayeredSurfaceSerialized
    {
        [DataContract]
        public class Layer
        {
            [DataMember]
            public CellSerialized[] Cells;

            [DataMember]
            public bool IsVisible;

            [DataMember]
            public object Metadata;

            [DataMember]
            public int Index;
        }

        [DataMember]
        public FontSerialized Font;
        [DataMember]
        public RectangleSerialized RenderArea;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public ColorSerialized DefaultForeground;
        [DataMember]
        public ColorSerialized DefaultBackground;
        [DataMember]
        public ColorSerialized Tint;
        [DataMember]
        public Layer[] Layers;
        [DataMember]
        public int ActiveLayer;

        public static implicit operator LayeredSurfaceSerialized(Surfaces.LayeredSurface surface)
        {
            return new LayeredSurfaceSerialized()
            {
                Font = surface.Font,
                RenderArea = surface.RenderArea,
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint,
                Layers = surface.GetLayers().Select(l => new LayeredSurfaceSerialized.Layer() { Cells = l.Cells.Select(c => (CellSerialized)c).ToArray(), Index = l.Index, IsVisible = l.IsVisible, Metadata = l.Metadata }).ToArray(),
                ActiveLayer = surface.ActiveLayerIndex
            };
        }

        public static implicit operator Surfaces.LayeredSurface(LayeredSurfaceSerialized surface)
        {
            var returnSurface = new Surfaces.LayeredSurface(surface.Width, surface.Height, surface.Font, surface.RenderArea, surface.Layers.Length)
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
            };

            foreach (var layer in surface.Layers)
            {
                var orgLayer = returnSurface.GetLayer(layer.Index);
                orgLayer.Cells = orgLayer.RenderCells = layer.Cells.Select(c => (Cell)c).ToArray();
                orgLayer.IsVisible = layer.IsVisible;
                orgLayer.Metadata = layer.Metadata;
            }

            returnSurface.SetActiveLayer(surface.ActiveLayer);

            return returnSurface;
        }
    }
}
