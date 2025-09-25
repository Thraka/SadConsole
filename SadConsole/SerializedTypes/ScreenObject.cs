using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadRogue.Primitives;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ScreenObjectJsonConverter : JsonConverter<ScreenObject>
{
    public override void WriteJson(JsonWriter writer, ScreenObject? value, JsonSerializer serializer)
    {
        if (value == null) return;

        writer.WriteStartObject();
        writer.WritePropertyName("$type"); writer.WriteValue($"{typeof(ScreenObject).FullName}, {typeof(ScreenObject).Assembly.GetName().Name}");
        writer.WritePropertyName(nameof(value.UseMouse)); writer.WriteValue(value.UseMouse);
        writer.WritePropertyName(nameof(value.UseKeyboard)); writer.WriteValue(value.UseKeyboard);
        writer.WritePropertyName(nameof(value.IsVisible)); writer.WriteValue(value.IsVisible);
        writer.WritePropertyName(nameof(value.IsEnabled)); writer.WriteValue(value.IsEnabled);
        writer.WritePropertyName(nameof(value.Position)); serializer.Serialize(writer, value.Position, typeof(Point));
        writer.WritePropertyName(nameof(value.SortOrder)); writer.WriteValue(value.SortOrder);
        writer.WritePropertyName(nameof(value.IgnoreParentPosition)); writer.WriteValue(value.IgnoreParentPosition);
        writer.WritePropertyName(nameof(value.FocusedMode)); serializer.Serialize(writer, value.FocusedMode, typeof(FocusBehavior));
        writer.WritePropertyName(nameof(value.IsExclusiveMouse)); writer.WriteValue(value.IsExclusiveMouse);
        writer.WritePropertyName("Children");
        writer.WriteStartArray();

        foreach (var item in value.Children)
            serializer.Serialize(writer, item, item.GetType());

        writer.WriteEndArray();
        writer.WritePropertyName("SadComponents");
        writer.WriteStartArray();

        foreach (var item in value.SadComponents)
            serializer.Serialize(writer, item, item.GetType());

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public override ScreenObject? ReadJson(JsonReader reader, Type objectType, ScreenObject? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        ScreenObject returnObject = new();

        while (reader.Read())
        {
            switch (reader.Value)
            {
                case "UseMouse": returnObject.UseMouse = reader.ReadAsBoolean()!.Value; break;
                case "UseKeyboard": returnObject.UseKeyboard = reader.ReadAsBoolean()!.Value; break;
                case "IsVisible": returnObject.IsVisible = reader.ReadAsBoolean()!.Value; break;
                case "IsEnabled": returnObject.IsEnabled = reader.ReadAsBoolean()!.Value; break;
                case "Position": reader.Read(); returnObject.Position = serializer.Deserialize<Point>(reader); break;
                case "SortOrder": returnObject.SortOrder = (uint)reader.ReadAsInt32()!.Value; break;
                case "IgnoreParentPosition": returnObject.IgnoreParentPosition = reader.ReadAsBoolean()!.Value; break;
                case "FocusedMode": returnObject.FocusedMode = Enum.Parse<FocusBehavior>(reader.ReadAsString()!); break;
                case "IsExclusiveMouse": returnObject.IsExclusiveMouse = reader.ReadAsBoolean()!.Value; break;
                case "Children":
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        while (reader.Read() && reader.TokenType == JsonToken.StartObject)
                        {
                            JObject token = JObject.Load(reader);
                            string[] typeParts = token["$type"]!.Value<string>()!.Split(", ");

                            JsonReader childReader = token.CreateReader();
                            
                            returnObject.Children.Add((IScreenObject)serializer.Deserialize(childReader, System.Reflection.Assembly.Load(typeParts[1]).GetType(typeParts[0]))!);
                        }
                    }
                    else
                        throw new JsonException("Unable to read object for 'Children' property. Should be an array of IScreenObject implementations");

                    break;
                case "SadComponents":
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        while (reader.Read() && reader.TokenType == JsonToken.StartObject)
                        {
                            JObject token = JObject.Load(reader);
                            string[] typeParts = token["$type"]!.Value<string>()!.Split(", ");

                            JsonReader childReader = token.CreateReader();

                            returnObject.SadComponents.Add((SadConsole.Components.IComponent)serializer.Deserialize(childReader, System.Reflection.Assembly.Load(typeParts[1]).GetType(typeParts[0]))!);
                        }
                    }
                    else
                        throw new JsonException("Unable to read object for 'Children' property. Should be an array of IScreenObject implementations");

                    break;
                default: reader.Skip(); break;
            }
        }

        return returnObject;
    }
}

public class ScreenObjectSerializedJsonConverter : JsonConverter<ScreenObjectSerialized>
{
    public override void WriteJson(JsonWriter writer, ScreenObjectSerialized? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ScreenObject)value!);

    public override ScreenObjectSerialized? ReadJson(JsonReader reader, Type objectType, ScreenObjectSerialized? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ScreenObject>(reader)!;
}

public class ScreenObjectSerialized
{
    public bool UseMouse;
    public bool UseKeyboard;
    public bool IsVisible;
    public bool IsEnabled;
    public Point Position;
    public IScreenObject[] Children;
    public Components.IComponent[] Components;
    public uint SortOrder;
    public bool IgnoreParentPosition;
    public FocusBehavior FocusedMode;
    public bool IsExclusiveMouse;


    public static implicit operator ScreenObjectSerialized(ScreenObject screen) =>
        new ScreenObjectSerialized()
        {
            UseMouse = screen.UseMouse,
            UseKeyboard = screen.UseKeyboard,
            IsVisible = screen.IsVisible,
            IsEnabled = screen.IsEnabled,
            Position = screen.Position,
            Children = screen.Children.ToArray<IScreenObject>(),
            Components = screen.SadComponents.ToArray(),
            SortOrder = screen.SortOrder,
            IgnoreParentPosition = screen.IgnoreParentPosition,
            FocusedMode = screen.FocusedMode,
            IsExclusiveMouse = screen.IsExclusiveMouse
        };

    public static implicit operator ScreenObject(ScreenObjectSerialized screen)
    {
        var screenObject = new ScreenObject()
        {
            UseMouse = screen.UseMouse,
            UseKeyboard = screen.UseKeyboard,
            IsVisible = screen.IsVisible,
            IsEnabled = screen.IsEnabled,
            Position = screen.Position,
            SortOrder = screen.SortOrder,
            IgnoreParentPosition = screen.IgnoreParentPosition,
            FocusedMode = screen.FocusedMode,
            IsExclusiveMouse = screen.IsExclusiveMouse
        };

        foreach (var child in screen.Children)
            screenObject.Children.Add(child);

        foreach (var component in screen.Components)
            screenObject.SadComponents.Add(component);

        screenObject.UpdateAbsolutePosition();

        return screenObject;
    }
}
