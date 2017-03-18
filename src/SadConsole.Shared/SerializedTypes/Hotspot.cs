using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class HotspotSerialized
    {
        [DataMember]
        public PointSerialized[] Positions;

        [DataMember]
        public string Title;

        [DataMember]
        public CellSerialized DebugAppearance;

        [DataMember]
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        public static implicit operator HotspotSerialized(GameHelpers.Hotspot zone)
        {
            return new HotspotSerialized()
            {
                Positions = zone.Positions.Select(p => (PointSerialized)p).ToArray(),
                Title = zone.Title,
                DebugAppearance = zone.DebugAppearance,
                Settings = zone.Settings
            };
        }

        public static implicit operator GameHelpers.Hotspot(HotspotSerialized zone)
        {
            return new GameHelpers.Hotspot()
            {
                Positions = zone.Positions.Select(p => (Microsoft.Xna.Framework.Point)p).ToList(),
                Title = zone.Title,
                DebugAppearance = zone.DebugAppearance,
                Settings = zone.Settings
            };
        }
    }
}
