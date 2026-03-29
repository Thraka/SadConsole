using System;

namespace SadConsole.Transitions;

/// <summary>
/// Fades in a single <see cref="IScreenSurface"/> from transparent to fully opaque.
/// </summary>
public class FadeIn : Instructions.InstructionBase
{
    private bool _firstRun = false;

    private Instructions.AnimatedValue _valueInstruction;

    private IScreenSurface _fadeTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="FadeIn"/> class to animate a screen surface from transparent to opaque
    /// over a specified duration.
    /// </summary>
    /// <remarks>Sets <see cref="Instructions.InstructionBase.RemoveOnFinished"/> to <see langword="true"/>.</remarks>
    /// <param name="target">The screen surface to fade in. Cannot be null.</param>
    /// <param name="duration">The total time over which the fade animation occurs.</param>
    /// <param name="easingFunction">An optional easing function that controls the rate of change of the fade animation. If null, a linear easing
    /// function is used.</param>
    public FadeIn(IScreenSurface target, TimeSpan duration, EasingFunctions.EasingBase? easingFunction = null)
    {
        easingFunction ??= new EasingFunctions.Linear();

        _fadeTarget = target;

        _valueInstruction = new Instructions.AnimatedValue(duration, 0, 255, easingFunction);

        RemoveOnFinished = true;
    }

    /// <summary>
    /// Processes the fade in of the target object.
    /// </summary>
    /// <param name="componentHost">The host running the component.</param>
    /// <param name="delta">The time difference for the frame.</param>
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        if (IsFinished) return;

        if (!_firstRun)
        {
            if (_fadeTarget.Renderer == null) throw new NullReferenceException("The target must have a renderer.");

            _fadeTarget.Renderer.Opacity = 0;
            _fadeTarget.IsVisible = true;
            _firstRun = true;
        }

        _valueInstruction.Update(componentHost, delta);

        _fadeTarget.Renderer!.Opacity = (byte)_valueInstruction.Value;

        if (_valueInstruction.IsFinished)
        {
            IsFinished = true;
            OnFinished(componentHost);
        }
    }
}
