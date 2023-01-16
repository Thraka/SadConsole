using System.Runtime.Serialization;

namespace SadConsole.Effects;

/// <summary>
/// A base class for cell effects.
/// </summary>
[DataContract]
public abstract class CellEffectBase : ICellEffect
{
    private System.TimeSpan _startDelay;

    /// <summary>
    /// A flag to indidcate that the delay timer has finished.
    /// </summary>
    [DataMember]
    protected bool _delayFinished = true;

    /// <summary>
    /// The total time elapsed while processing the effect.
    /// </summary>
    [DataMember]
    protected System.TimeSpan _timeElapsed;

    /// <inheritdoc />
    [DataMember]
    public bool IsFinished { get; protected set; }

    /// <inheritdoc />
    [DataMember]
    public bool CloneOnAdd { get; set; }

    /// <inheritdoc />
    [DataMember]
    public System.TimeSpan StartDelay
    {
        get => _startDelay;
        set { _startDelay = value; _delayFinished = _startDelay <= System.TimeSpan.Zero; }
    }

    /// <inheritdoc />
    [DataMember]
    public bool RemoveOnFinished { get; set; }

    /// <inheritdoc />
    [DataMember]
    public bool RestoreCellOnRemoved { get; set; }

    /// <inheritdoc />
    [DataMember]
    public bool RunEffectOnApply { get; set; }

    /// <summary>
    /// Creates a new instance of the effect.
    /// </summary>
    protected CellEffectBase()
    {
        RemoveOnFinished = false;
        StartDelay = System.TimeSpan.Zero;
        IsFinished = false;
        _timeElapsed = System.TimeSpan.Zero;
    }

    /// <inheritdoc />
    public abstract bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState);

    /// <inheritdoc />
    public virtual void Update(System.TimeSpan delta)
    {
        if (!IsFinished)
        {
            _timeElapsed += delta;

            if (!_delayFinished)
            {
                if (_timeElapsed >= _startDelay)
                {
                    _delayFinished = true;
                    _timeElapsed = System.TimeSpan.Zero;
                }
            }
        }
    }

    /// <inheritdoc />
    public virtual void Restart()
    {
        _timeElapsed = System.TimeSpan.Zero;
        IsFinished = false;
        StartDelay = _startDelay;
    }

    /// <inheritdoc />
    public abstract ICellEffect Clone();

    ///// <summary>
    ///// Determines if the passed in ICellEffect equals this one or not.
    ///// </summary>
    ///// <param name="other">The other ICellEffect to test.</param>
    ///// <returns>True or false indicating equality.</returns>
    //public virtual bool Equals(ICellEffect other)
    //{
    //    if (IsFinished == other.IsFinished &&
    //        CloneOnApply == other.CloneOnApply &&
    //        StartDelay == other.StartDelay &&
    //        RemoveOnFinished == other.RemoveOnFinished &&
    //        Permanent == other.Permanent &&
    //        IsFinished == other.IsFinished)

    //        return true;

    //    else
    //        return false;
    //}
}
