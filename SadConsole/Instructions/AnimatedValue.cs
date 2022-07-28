using System;

namespace SadConsole.Instructions;

/// <summary>
/// An function that applies an <see cref="EasingFunctions.EasingBase"/> function between two values.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Instruction: Animated value")]
public class AnimatedValue : Wait
{
    /// <summary>
    /// The easing function assigned to animate the value.
    /// </summary>
    protected EasingFunctions.EasingBase EasingFunction;

    /// <summary>
    /// The current or last value of the animation.
    /// </summary>
    public double Value
    {
        get
        {
            if (IsFinished)
                return EndingValue;

            return GetValueForDuration(CountedTime.TotalMilliseconds);
        }
    }

    /// <summary>
    /// The stating value of the animation.
    /// </summary>
    public double StartingValue { get; private set; }

    /// <summary>
    /// The ending value of the animation.
    /// </summary>
    public double EndingValue { get; private set; }

    /// <summary>
    /// Creates a new value animated over time.
    /// </summary>
    /// <param name="duration">The total time this animation should run.</param>
    /// <param name="startingValue">The starting value of the animation.</param>
    /// <param name="endingValue">The ending value of the animation.</param>
    /// <param name="easingFunction">The easing function used during animation. Defaults to <see cref="EasingFunctions.Linear"/>.</param>
    public AnimatedValue(TimeSpan duration, double startingValue, double endingValue, EasingFunctions.EasingBase easingFunction = null) =>
        Reset(duration, startingValue, endingValue, easingFunction);

    /// <summary>
    /// Resets this object to new values.
    /// </summary>
    /// <param name="duration">The total time this animation should run.</param>
    /// <param name="startingValue">The starting value of the animation.</param>
    /// <param name="endingValue">The ending value of the animation.</param>
    /// <param name="easingFunction">The easing function used during animation. Defaults to <see cref="EasingFunctions.Linear"/>.</param>
    public void Reset(TimeSpan duration, double startingValue, double endingValue, EasingFunctions.EasingBase easingFunction = null)
    {
        Duration = duration;
        EasingFunction = easingFunction ?? new EasingFunctions.Linear();
        StartingValue = startingValue;
        EndingValue = endingValue;
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        CountedTime = TimeSpan.Zero;
        base.Reset();
    }

    /// <summary>
    /// Updates the timer with the time since the last call.
    /// </summary>
    /// <param name="console">The parent object.</param>
    /// <param name="delta">The time since the last frame update.</param>
    public override void Update(IScreenObject console, TimeSpan delta)
    {
        if (!IsFinished)
            base.Update(console, delta);
    }

    private double GetValueForDuration(double time)
    {
        double timeDuration = Duration.TotalMilliseconds;

        if (time > timeDuration)
            time = timeDuration;

        return EasingFunction.Ease(time, StartingValue, EndingValue - StartingValue, timeDuration);
    }
}
