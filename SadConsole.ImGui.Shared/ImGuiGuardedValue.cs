namespace SadConsole.ImGuiSystem;

/// <summary>
/// Makes a copy of a value and allows it to be changed. The change can be committed or reverted.
/// </summary>
/// <typeparam name="T">A structure.</typeparam>
public struct ImGuiGuardedValue<T> where T : struct
{
    /// <summary>
    /// The original value.
    /// </summary>
    public T OriginalValue;

    /// <summary>
    /// The current value.
    /// </summary>
    public T CurrentValue;

    /// <summary>
    /// Creates a new instance of this structure, wrapping the provided value.
    /// </summary>
    /// <param name="value">The original value.</param>
    public ImGuiGuardedValue(T value)
    {
        OriginalValue = value;
        CurrentValue = value;
    }

    /// <summary>
    /// Returns true when the original value doesn't equal the current value.
    /// </summary>
    /// <returns>A value to indicate changed state.</returns>
    public bool IsChanged() =>
        !OriginalValue.Equals(CurrentValue);

    /// <summary>
    /// Sets the original value to the current value.
    /// </summary>
    public void Commit() =>
        OriginalValue = CurrentValue;

    /// <summary>
    /// Sets the current value to the original value.
    /// </summary>
    public void Reset() =>
        CurrentValue = OriginalValue;
}
