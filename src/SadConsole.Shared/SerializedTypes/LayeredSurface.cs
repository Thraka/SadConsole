using FrameworkPoint = Microsoft.Xna.Framework.Point;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class LayeredSurface
    {
        [DataContract]
        public class Layer
        {
            [DataMember]
            public Cell[] Cells;

            [DataMember]
            public bool IsVisible;

            [DataMember]
            public object Metadata;

            [DataMember]
            public int Index;
        }

        [DataMember]
        public Font Font;
        [DataMember]
        public Rectangle RenderArea;
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;
        [DataMember]
        public Color DefaultForeground;
        [DataMember]
        public Color DefaultBackground;
        [DataMember]
        public Color Tint;
        [DataMember]
        public Layer[] Layers;
        [DataMember]
        public int ActiveLayer;

        public static implicit operator LayeredSurface(Surfaces.LayeredSurface surface)
        {
            return new LayeredSurface()
            {
                Font = surface.Font,
                RenderArea = surface.RenderArea,
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint,
                Layers = surface.GetLayers().Select(l => new LayeredSurface.Layer() { Cells = l.Cells, Index = l.Index, IsVisible = l.IsVisible, Metadata = l.Metadata }).ToArray(),
                ActiveLayer = surface.ActiveLayerIndex
            };
        }

        public static implicit operator Surfaces.LayeredSurface(LayeredSurface surface)
        {
            var returnSurface = new Surfaces.LayeredSurface(surface.Width, surface.Height, surface.Font, surface.Layers.Length)
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
            };

            foreach (var layer in surface.Layers)
            {
                var orgLayer = returnSurface.GetLayer(layer.Index);
                orgLayer.Cells = orgLayer.RenderCells = layer.Cells;
                orgLayer.IsVisible = layer.IsVisible;
                orgLayer.Metadata = layer.Metadata;
            }

            returnSurface.SetActiveLayer(surface.ActiveLayer);
            returnSurface.RenderArea = surface.RenderArea;

            return returnSurface;
        }
    }
}
