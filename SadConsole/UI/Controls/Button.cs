using System;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Simple button control with a height of 1.
/// </summary>
[DataContract]
public class Button : ButtonBase
{
    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public Button(int width, int height = 1)
        : base(width, height)
    {
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
