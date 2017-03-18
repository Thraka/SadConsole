using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class ZoneSerialized
    {
        [DataMember]
        public RectangleSerialized Area;

        [DataMember]
        public string Title;

        [DataMember]
        public CellSerialized DebugAppearance;

        [DataMember]
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        public static implicit operator ZoneSerialized(GameHelpers.Zone zone)
        {
            return new ZoneSerialized()
            {
                Area = zone.Area,
                Title = zone.Title,
                DebugAppearance = zone.DebugAppearance,
                Settings = zone.Settings
            };
        }

        public static implicit operator GameHelpers.Zone(ZoneSerialized zone)
        {
            return new GameHelpers.Zone()
            {
                Area = zone.Area,
                Title = zone.Title,
                DebugAppearance = zone.DebugAppearance,
                Settings = zone.Settings
            };
        }
    }
}
