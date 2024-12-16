using System;

namespace SadConsole.ImGuiSystem;

/// <summary>
/// Wraps an enum type for ImGui controls, like listboxes.
/// </summary>
/// <typeparam name="TEnum">The enum to wrap.</typeparam>
public static class ImGuiListEnum<TEnum> where TEnum : struct, Enum
{
    /// <summary>
    /// A collection of each enum name.
    /// </summary>
    public static string[] Names = Enum.GetNames<TEnum>();

    /// <summary>
    /// A collection of each enum value.
    /// </summary>
    public static TEnum[] Values = Enum.GetValues<TEnum>();

    /// <summary>
    /// The count of the enums items.
    /// </summary>
    public static int Count => Names.Length;

    /// <summary>
    /// Gets a value from an index.
    /// </summary>
    /// <param name="index">Index of the value to get.</param>
    /// <returns>A value.</returns>
    public static TEnum GetValueFromIndex(int index) =>
        Values[index];

    /// <summary>
    /// Gets an index from a value.
    /// </summary>
    /// <param name="value">The value to get an index from.</param>
    /// <returns>The index.</returns>
    public static int GetIndexFromValue(TEnum value) =>
        Array.IndexOf(Values, value);
}
