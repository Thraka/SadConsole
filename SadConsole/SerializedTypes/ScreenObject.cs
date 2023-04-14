using System;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ScreenObjectJsonConverter : JsonConverter<ScreenObject>
{
    public override void WriteJson(JsonWriter writer, ScreenObject? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ScreenObjectSerialized)value!);

    public override ScreenObject? ReadJson(JsonReader reader, Type objectType, ScreenObject? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ScreenObjectSerialized>(reader)!;
}

public class ScreenObjectSerialized
{
    public bool UseMouse;
    public bool UseKeyboard;
    public bool IsVisible;
    public bool IsEnabled;
    public Point Position;
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

        foreach (var component in screen.Components)
            screenObject.SadComponents.Add(component);

        screenObject.UpdateAbsolutePosition();

        return screenObject;
    }
}
