using System;

namespace SadConsole.Transitions;

/// <summary>
/// Fades out a single <see cref="IScreenSurface"/> from fully opaque to transparent.
/// </summary>
public class FadeOut : Instructions.InstructionBase
{
    private bool _firstRun = false;

    private Instructions.AnimatedValue _valueInstruction;

    private IScreenSurface _fadeTarget;

    /// <summary>
    /// When <see langword="true"/>, removes the target object from its parent when the transition finishes. Supersedes <see cref="HideObject"/>.
    /// </summary>
    public bool DeparentObject { get; set; }

    /// <summary>
    /// When <see langword="true"/>, sets <see cref="IScreenObject.IsVisible"/> to <see langword="false"/> on the target object when the transition finishes.
    /// </summary>
    public bool HideObject { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FadeOut"/> class to animate a screen surface from opaque to transparent
    /// over a specified duration.
    /// </summary>
    /// <remarks>Sets <see cref="Instructions.InstructionBase.RemoveOnFinished"/> to <see langword="true"/>.</remarks>
    /// <param name="target">The screen surface to fade out. Cannot be null.</param>
    /// <param name="duration">The total time over which the fade animation occurs.</param>
    /// <param name="easingFunction">An optional easing function that controls the rate of change of the fade animation. If null, a linear easing
    /// function is used.</param>
    public FadeOut(IScreenSurface target, TimeSpan duration, EasingFunctions.EasingBase? easingFunction = null)
    {
        easingFunction ??= new EasingFunctions.Linear();

        _fadeTarget = target;

        _valueInstruction = new Instructions.AnimatedValue(duration, 255, 0, easingFunction);

        RemoveOnFinished = true;
    }

    /// <summary>
    /// Processes the fade out of the target object.
    /// </summary>
    /// <param name="componentHost">The host running the component.</param>
    /// <param name="delta">The time difference for the frame.</param>
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        if (IsFinished) return;

        if (!_firstRun)
        {
            if (_fadeTarget.Renderer == null) throw new NullReferenceException("The target must have a renderer.");

            _fadeTarget.Renderer.Opacity = 255;
            _fadeTarget.IsVisible = true;
            _firstRun = true;
        }

        _valueInstruction.Update(componentHost, delta);

        _fadeTarget.Renderer!.Opacity = (byte)_valueInstruction.Value;

        if (_valueInstruction.IsFinished)
        {
            IsFinished = true;

            if (DeparentObject)
                _fadeTarget.Parent = null;
            else if (HideObject)
                _fadeTarget.IsVisible = false;

            OnFinished(componentHost);
        }
    }
}
