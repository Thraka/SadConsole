using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using SadConsole.GameHelpers;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class SceneSerialized
    {
        [DataMember]
        public PointSerialized Position;
        [DataMember]
        public bool IsVisible;
        [DataMember]
        public bool IsPaused;
        [DataMember]
        public List<GameObject> Objects;
        [DataMember]
        public List<Zone> Zones;
        [DataMember]
        public List<Hotspot> Hotspots;
        [DataMember]
        public bool UsePixelPositioning;

        public static implicit operator SceneSerialized(SadConsole.GameHelpers.Scene scene)
        {
            return new SceneSerialized()
            {
                Objects = scene.Objects,
                Zones = scene.Zones,
                Hotspots = scene.Hotspots,
                Position = scene.Position,
                IsVisible = scene.IsVisible,
                IsPaused = scene.IsPaused,
                UsePixelPositioning = scene.UsePixelPositioning
            };
        }

        public static implicit operator SadConsole.GameHelpers.Scene(SceneSerialized scene)
        {
            return new SadConsole.GameHelpers.Scene(1, 1)
            {
                Objects = scene.Objects,
                Zones = scene.Zones,
                Hotspots = scene.Hotspots,
                Position = scene.Position,
                IsVisible = scene.IsVisible,
                IsPaused = scene.IsPaused,
                UsePixelPositioning = scene.UsePixelPositioning
            };
        }
    }

}
