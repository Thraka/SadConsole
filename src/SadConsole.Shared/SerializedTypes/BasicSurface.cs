using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;
using System.Linq;
using Newtonsoft.Json;
using SadConsole.Surfaces;

namespace SadConsole.SerializedTypes
{
    public class BasicSurfaceJsonConverter: JsonConverter<Surfaces.Basic>
    {
        public override void WriteJson(JsonWriter writer, Basic value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (BasicSurfaceSerialized)value);
        }

        public override Basic ReadJson(JsonReader reader, Type objectType, Basic existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<BasicSurfaceSerialized>(reader);
        }
    }


    [DataContract]
    public class BasicSurfaceSerialized: ScreenObjectSerialized
    {
        [DataMember] public FontSerialized Font;
        [DataMember] public RectangleSerialized ViewPort;
        [DataMember] public CellSerialized[] Cells;
        [DataMember] public int Width;
        [DataMember] public int Height;
        [DataMember] public ColorSerialized DefaultForeground;
        [DataMember] public ColorSerialized DefaultBackground;
        [DataMember] public ColorSerialized Tint;

        public static implicit operator BasicSurfaceSerialized(Surfaces.SurfaceBase surface)
        {
            return new BasicSurfaceSerialized()
            {
                Font = surface.Font,
                ViewPort = surface.ViewPort,
                Cells = surface.Cells.Select(c => (CellSerialized) c).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }

        public static implicit operator BasicSurfaceSerialized(Surfaces.BasicNoDraw surface)
        {
            return new BasicSurfaceSerialized()
            {
                ViewPort = surface.ViewPort,
                Cells = surface.Cells.Select(c => (CellSerialized) c).ToArray(),
                Width = surface.Width,
                Height = surface.Height,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Tint = surface.Tint,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }

        public static implicit operator Surfaces.Basic(BasicSurfaceSerialized surface)
        {
            return new Surfaces.Basic(surface.Width, surface.Height, surface.Font, surface.ViewPort,
                surface.Cells.Select(c => (Cell) c).ToArray())
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }

        public static implicit operator Surfaces.BasicNoDraw(BasicSurfaceSerialized surface)
        {
            return new Surfaces.BasicNoDraw(surface.Width, surface.Height, surface.Font, surface.ViewPort,
                surface.Cells.Select(c => (Cell) c).ToArray())
            {
                Tint = surface.Tint,
                DefaultForeground = surface.DefaultForeground,
                DefaultBackground = surface.DefaultBackground,
                Position = surface.Position,
                IsVisible = surface.IsVisible,
                IsPaused = surface.IsPaused
            };
        }
    }
}
