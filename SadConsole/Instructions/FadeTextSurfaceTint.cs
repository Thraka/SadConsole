﻿using System;
using SadRogue.Primitives;

namespace SadConsole.Instructions;

/// <summary>
/// Animates a color change to <see cref="ScreenSurface.Tint"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Instruction: Fade tint")]
public class FadeTextSurfaceTint : AnimatedValue
{
    private IScreenSurface? _objectSurface;
    private Gradient _colors;

    /// <summary>
    /// The color to fade the tint to.
    /// </summary>
    public Gradient Colors
    {
        get => _colors;
        set => _colors = value ?? throw new Exception($"{nameof(Colors)} can't be set to null");
    }

    /// <summary>
    /// Creates a new tint fade instruction that targets the specified console.
    /// </summary>
    /// <param name="objectSurface">The <see cref="IScreenSurface.Tint"/> to fade.</param>
    /// <param name="colors">The gradient pattern to fade through.</param>
    /// <param name="duration">How long the fade takes.</param>
    public FadeTextSurfaceTint(IScreenSurface objectSurface, Gradient colors, TimeSpan duration) : base(duration, 0d, 1d)
    {
        _colors = colors ?? throw new Exception($"{nameof(Colors)} can't be set to null");
        _objectSurface = objectSurface;
    }

    /// <summary>
    /// Creates a new tint fade instruction that uses the console passed to <see cref="SadConsole.Components.IComponent.Update(IScreenObject, TimeSpan)"/>.
    /// </summary>
    /// <param name="colors">The gradient pattern to fade through.</param>
    /// <param name="duration">How long the fade takes.</param>
    public FadeTextSurfaceTint(Gradient colors, TimeSpan duration) : base(duration, 0d, 1d)
    {
        _colors = colors ?? throw new Exception($"{nameof(Colors)} can't be set to null");
    }

    /// <summary>
    /// Creates a new tint fade instruction with default settings that uses the console passed to <see cref="SadConsole.Components.IComponent.Update(IScreenObject, TimeSpan)"/>.
    /// </summary>
    /// <remarks>
    /// The default settings are:
    /// 
    ///   - <see cref="Colors"/>: <see cref="Color.White"/> to <see cref="Color.Black"/>
    ///   - Duration: 1 second
    /// </remarks>
    public FadeTextSurfaceTint() : base(TimeSpan.FromSeconds(1), 0d, 1d)
    {
        _colors = new Gradient(Color.White, Color.Transparent);
    }

    /// <inheritdoc />
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        if (!IsFinished)
        {
            base.Update(componentHost, delta);

            if (_objectSurface != null)
                _objectSurface.Tint = Colors.Lerp((float)Value);

            else if (componentHost is IScreenSurface surface)
                surface.Tint = Colors.Lerp((float)Value);
        }
    }
}
