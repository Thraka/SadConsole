//using System;
//using System.Linq;
//using System.Runtime.Serialization;
//using Newtonsoft.Json;

//namespace SadConsole.SerializedTypes
//{
//    public class ScreenObjectJsonConverter : JsonConverter<ScreenObject>
//    {
//        public override void WriteJson(JsonWriter writer, ScreenObject value, JsonSerializer serializer) =>
//            serializer.Serialize(writer, (ScreenObjectSerialized)value);

//        public override ScreenObject ReadJson(JsonReader reader, Type objectType, ScreenObject existingValue, bool hasExistingValue, JsonSerializer serializer) =>
//            serializer.Deserialize<ScreenObjectSerialized>(reader);
//    }

//    public class ScreenObjectSerialized
//    {
//        public bool UseMouse;
//        public bool UseKeyboard;
//        public bool IsVisible;
//        public bool IsEnabled;
//        public PointSerialized Position;

//        public static implicit operator ScreenObjectSerialized(ScreenObject screen) =>
//            new ScreenObjectSerialized()
//        {
//                UseMouse = screen.UseMouse,
//                UseKeyboard = screen.UseKeyboard,
//                IsVisible = screen.IsVisible,
//                IsEnabled = screen.IsEnabled,
//                Position = screen.Position,
//        };

//        public static implicit operator ScreenObject(ScreenObjectSerialized screen)
//        {
//            var screenObject = new ScreenObject()
//            {
//                UseMouse = screen.UseMouse,
//                UseKeyboard = screen.UseKeyboard,
//                IsVisible = screen.IsVisible,
//                IsEnabled = screen.IsEnabled,
//                Position = screen.Position,
//            };

//            return screenObject;
//        }
//    }
//}
