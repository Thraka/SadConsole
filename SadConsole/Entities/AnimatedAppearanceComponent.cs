using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SadConsole.Entities;

/// <summary>
/// A component for <see cref="Entity"/> that animates the <see cref="Entity.Appearance"/> and <see cref="Entity.Effect"/> properties.
/// </summary>
[DataContract]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class AnimatedAppearanceComponent : Components.UpdateComponent
{
    [DataMember] private ColoredString.ColoredGlyphEffect[] _frames = Array.Empty<ColoredString.ColoredGlyphEffect>();
    [DataMember] private int _frameIndex;
    [DataMember] private bool _isPlaying;
    [DataMember] private TimeSpan _animationTime;
    [DataMember] private TimeSpan _timePerFrame;
    [DataMember] private TimeSpan _totalTime;

    private Entity _entity;

    /// <summary>
    /// The frames of animation.
    /// </summary>
    public ColoredString.ColoredGlyphEffect[] Frames
    {
        get => _frames;
        set
        {
            _frames = value ?? Array.Empty<ColoredString.ColoredGlyphEffect>();
            _frameIndex = 0;
            AnimationTime = _animationTime;
        }
    }

    /// <summary>
    /// The total time it takes to play all <see cref="Frames"/>.
    /// </summary>
    public TimeSpan AnimationTime
    {
        get => _animationTime;
        set
        {
            _animationTime = value;

            if (_animationTime == TimeSpan.Zero || _frames.Length == 0)
                _timePerFrame = TimeSpan.Zero;
            else
                _timePerFrame = _animationTime.Divide(_frames.Length);
        }
    }

    /// <summary>
    /// When <see langword="true"/>, the animation will automatically restart after the last frame is applied. Otherwise, <see langword="false"/> and the animation stops when completed.
    /// </summary>
    public bool IsRepeatable { get; set; }

    /// <summary>
    /// Called by the component system when this component is added to an object. Must be of type <see cref="Entity"/>.
    /// </summary>
    /// <param name="host">The component host.</param>
    /// <exception cref="InvalidCastException">This component was added to a type other than <see cref="Entity"/>.</exception>
    public override void OnAdded(IScreenObject host)
    {
        if (!(host is Entity entity)) throw new InvalidCastException("Component can only be used on an entity.");

        _entity = entity;
    }

    /// <summary>
    /// Called by the component system when this component is removed from an object.
    /// </summary>
    /// <param name="host">The component host.</param>
    public override void OnRemoved(IScreenObject host) =>
        _entity = null;


    /// <summary>
    /// Updates the animation frame index and applies the animation to the entity.
    /// </summary>
    /// <param name="host">The component host.</param>
    /// <param name="delta">The time between calls to this method.</param>
    public override void Update(IScreenObject host, TimeSpan delta)
    {
        if (!_isPlaying) return;

        _totalTime += delta;

        if (_totalTime >= _timePerFrame)
        {
            _totalTime -= _timePerFrame;

            _frameIndex++;

            if (_frameIndex >= _frames.Length)
            {
                _frameIndex = 0;

                if (!IsRepeatable)
                {
                    Stop();
                    return;
                }
            }

            _frames[_frameIndex].CopyAppearanceTo(_entity.Appearance, false);
            _entity.Effect = _frames[_frameIndex].Effect;
        }
    }

    /// <summary>
    /// Starts the animation and immediately applies the current frame to the entity.
    /// </summary>
    /// <exception cref="InvalidOperationException">The animation was started but there aren't any frames to animate.</exception>
    public void Start()
    {
        if (_frames.Length == 0) throw new InvalidOperationException("Animation was started but there aren't any frames to animate");

        _isPlaying = true;
        _frames[_frameIndex].CopyAppearanceTo(_entity.Appearance, false);
        _entity.Effect = _frames[_frameIndex].Effect;
    }

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public void Stop()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// Restarts the animation at the first frame.
    /// </summary>
    public void Restart()
    {
        if (_frames.Length == 0) throw new InvalidOperationException("Animation was started but there aren't any frames to animate");

        _frameIndex = 0;
        Start();
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
        if (_frames == null || _frames.Length == 0) _frames = Array.Empty<ColoredString.ColoredGlyphEffect>();
    }
}
