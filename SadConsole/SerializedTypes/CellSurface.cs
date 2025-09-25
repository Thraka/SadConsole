//using System;
//using System.Linq;
//using System.Runtime.Serialization;
//using Newtonsoft.Json;

//namespace SadConsole.SerializedTypes
//{
//    public class CellSurfaceJsonConverter : JsonConverter<CellSurface>
//    {
//        public override void WriteJson(JsonWriter writer, CellSurface value, JsonSerializer serializer) =>
//            serializer.Serialize(writer, (CellSurfaceSerialized)value);

//        public override CellSurface ReadJson(JsonReader reader, Type objectType, CellSurface existingValue, bool hasExistingValue, JsonSerializer serializer) =>
//            serializer.Deserialize<CellSurfaceSerialized>(reader);
//    }

//    public class CellSurfaceSerialized
//    {
//        public int Width;
//        public int Height;
//        public ColorSerialized DefaultForeground;
//        public ColorSerialized DefaultBackground;
//        public ColoredGlyphSerialized[] Cells;
//        public RectangleSerialized View;

//        public static implicit operator CellSurfaceSerialized(CellSurface screen) =>
//            new CellSurfaceSerialized()
//        {
//            Cells = screen.Cells.Select(c => (ColoredGlyphSerialized)c).ToArray(),
//            Width = screen.BufferWidth,
//            Height = screen.BufferHeight,
//            DefaultForeground = screen.DefaultForeground,
//            DefaultBackground = screen.DefaultBackground,
//            View = screen.View
//        };

//        public static implicit operator CellSurface(CellSurfaceSerialized screen) =>
//            new CellSurface(screen.View.Width, screen.View.Height, screen.Width, screen.Height, screen.Cells.Select(c => (ColoredGlyph)c).ToArray())
//        {
//            DefaultBackground = screen.DefaultBackground,
//            DefaultForeground = screen.DefaultForeground,
//        };
//    }
//}
