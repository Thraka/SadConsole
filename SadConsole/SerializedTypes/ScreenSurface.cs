using System;
using System.Linq;
using Newtonsoft.Json;
using SadRogue.Primitives;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SadConsole.SerializedTypes;

public class ScreenSurfaceJsonConverter : JsonConverter<ScreenSurface>
{
    public override void WriteJson(JsonWriter writer, ScreenSurface? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, (ScreenSurfaceSerialized)value!);

    public override ScreenSurface? ReadJson(JsonReader reader, Type objectType, ScreenSurface? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        serializer.Deserialize<ScreenSurfaceSerialized>(reader)!;
}

public class ScreenSurfaceSerialized: ScreenObjectSerialized
{
    public IFont Font;
    public Point FontSize;
    public Color Tint;
    public bool UsePixelPositioning;
    public object CellSurface;
    public bool QuietSurfaceHandling;
    public bool MoveToFrontOnMouseClick;
    public bool FocusOnMouseClick;



    public static implicit operator ScreenSurfaceSerialized(ScreenSurface screen) =>
        new ScreenSurfaceSerialized()
        {
            // ScreenObject
            UseMouse = screen.UseMouse,
            UseKeyboard = screen.UseKeyboard,
            IsVisible = screen.IsVisible,
            IsEnabled = screen.IsEnabled,
            Position = screen.Position,
            Components = screen.SadComponents.ToArray(),
            SortOrder = screen.SortOrder,
            IgnoreParentPosition = screen.IgnoreParentPosition,
            FocusedMode = screen.FocusedMode,
            IsExclusiveMouse = screen.IsExclusiveMouse,

            // ScreenSurface
            Font = screen.Font,
            FontSize = screen.FontSize,
            Tint = screen.Tint,
            UsePixelPositioning = screen.UsePixelPositioning,
            CellSurface = screen.Surface,
            QuietSurfaceHandling = screen.QuietSurfaceHandling,
            MoveToFrontOnMouseClick = screen.MoveToFrontOnMouseClick,
            FocusOnMouseClick = screen.FocusOnMouseClick
        };

    public static implicit operator ScreenSurface(ScreenSurfaceSerialized screen)
    {
        var screenSurface = new ScreenSurface((ICellSurface)screen.CellSurface, screen.Font, screen.FontSize)
        {
            // ScreenObject
            UseMouse = screen.UseMouse,
            UseKeyboard = screen.UseKeyboard,
            IsVisible = screen.IsVisible,
            IsEnabled = screen.IsEnabled,
            Position = screen.Position,
            SortOrder = screen.SortOrder,
            IgnoreParentPosition = screen.IgnoreParentPosition,
            FocusedMode = screen.FocusedMode,
            IsExclusiveMouse = screen.IsExclusiveMouse,

            // ScreenSurface
            Tint = screen.Tint,
            UsePixelPositioning = screen.UsePixelPositioning,
            QuietSurfaceHandling = screen.QuietSurfaceHandling,
            MoveToFrontOnMouseClick = screen.MoveToFrontOnMouseClick,
            FocusOnMouseClick = screen.FocusOnMouseClick
        };

        foreach (var component in screen.Components)
            screenSurface.SadComponents.Add(component);

        screenSurface.UpdateAbsolutePosition();

        return screenSurface;
    }
}
