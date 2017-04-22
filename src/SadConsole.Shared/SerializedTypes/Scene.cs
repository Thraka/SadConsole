using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using SadConsole.GameHelpers;
using System.Linq;

namespace SadConsole.SerializedTypes
{
    /// <summary>
    /// Serialized instance of a <see cref="Scene"/> object.
    /// </summary>
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
        public List<GameObjectSerialized> Objects;
        [DataMember]
        public List<ZoneSerialized> Zones;
        [DataMember]
        public List<HotspotSerialized> Hotspots;
        [DataMember]
        public bool UsePixelPositioning;

        public static implicit operator SceneSerialized(SadConsole.GameHelpers.Scene scene)
        {
            return new SceneSerialized()
            {
                Objects = scene.Objects.Select(t => (GameObjectSerialized)t).ToList(),
                Zones = scene.Zones.Select(z => (ZoneSerialized)z).ToList(),
                Hotspots = scene.Hotspots.Select(h => (HotspotSerialized)h).ToList(),
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
                Objects = scene.Objects.Select(t => (GameObject)t).ToList(),
                Zones = scene.Zones.Select(z => (Zone)z).ToList(),
                Hotspots = scene.Hotspots.Select(h => (Hotspot)h).ToList(),
                Position = scene.Position,
                IsVisible = scene.IsVisible,
                IsPaused = scene.IsPaused,
                UsePixelPositioning = scene.UsePixelPositioning
            };
        }
    }

}
