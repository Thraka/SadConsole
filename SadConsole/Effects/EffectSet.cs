using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Effects;

/// <summary>
/// Chains one effect after another.
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("Effect: Set")]
public class EffectSet : CellEffectBase, IEnumerable<ICellEffect>
{
    private LinkedListNode<ICellEffect> _currentEffectNode;

    /// <summary>
    /// The list of effects to process.
    /// </summary>
    [DataMember]
    public LinkedList<ICellEffect> Effects { get; } = new LinkedList<ICellEffect>();

    /// <summary>
    /// When <see langword="true"/>, instead of ending when finished, it will repeat. Otherwise, <see langword="false"/>.
    /// </summary>
    [DataMember]
    public bool Repeat { get; set; }

    [DataMember]
    private bool _inChainDelay = false;

    /// <summary>
    /// An artificial delay between each effect.
    /// </summary>
    [DataMember]
    public System.TimeSpan DelayBetweenEffects { get; set; } = System.TimeSpan.Zero;

    ///// <summary>
    ///// Disables the effect.
    ///// </summary>
    //public void End() => _enabled = false;

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
    {
        if (_currentEffectNode != null)
            return _currentEffectNode.Value.ApplyToCell(cell, originalState);
        else
            return false;
    }

    /// <inheritdoc />
    public override void Update(System.TimeSpan delta)
    {
        // Check that there are effects to run, otherwise finish and quit
        if (Effects.Count == 0 || IsFinished)
        {
            IsFinished = true;
            return;
        }

        // If the following state is true, it's the first run of the set
        if (!IsFinished && _currentEffectNode == null)
            _currentEffectNode = Effects.First;

        // Handles updating _timeElapsed and _delayFinished
        base.Update(delta);

        if (_delayFinished)
        {
            // Check to see if we are in a chain delay, if so, we wait x seconds until we move on to the next chained effect.
            if (!_inChainDelay)
            {
                // Process the effect
                _currentEffectNode.Value.Update(delta);

                // If the effect finished, we move on to the next effect
                if (_currentEffectNode.Value.IsFinished)
                {
                    // Restart time
                    _timeElapsed = System.TimeSpan.Zero;

                    // Get next node
                    _currentEffectNode = _currentEffectNode.Next;

                    // If no node, check for repeat or end.
                    if (_currentEffectNode == null)
                    {
                        if (Repeat)
                            Restart();
                        else
                            IsFinished = true;
                    }
                    else
                    {
                        // When moving to the next effect, check and see if we have a delay. If so, flag and wait.
                        if (DelayBetweenEffects != System.TimeSpan.Zero)
                            _inChainDelay = true;
                    }
                }
            }
            else
            {
                if (_timeElapsed >= DelayBetweenEffects)
                {
                    _inChainDelay = false;
                    _timeElapsed = System.TimeSpan.Zero;
                }
            }
        }
    }

    /// <summary>
    /// Restarts the cell effect but does not reset it.
    /// </summary>
    public override void Restart()
    {
        // Handles delay and toggle IsFinished
        base.Restart();

        _inChainDelay = false;

        foreach (ICellEffect item in Effects)
            item.Restart();

        _currentEffectNode = Effects.First;
    }

    /// <inheritdoc />
    public override ICellEffect Clone()
    {
        EffectSet chain = new EffectSet()
        {
            DelayBetweenEffects = DelayBetweenEffects,
            Repeat = Repeat,

            IsFinished = IsFinished,
            StartDelay = StartDelay,
            CloneOnAdd = CloneOnAdd,
            RemoveOnFinished = RemoveOnFinished,
            RestoreCellOnRemoved = RestoreCellOnRemoved,
            _timeElapsed = _timeElapsed,
        };

        foreach (ICellEffect item in Effects)
        {
            chain.Effects.AddLast(item.CloneOnAdd ? item.Clone() : item);
        }



        return chain;
    }

    /// <inheritdoc />
    public override string ToString() =>
        string.Format("EFFECTSCHAIN-{0}-{1}-{2}-{3}", StartDelay, RemoveOnFinished, DelayBetweenEffects, Repeat);

    /// <summary>
    /// Gets an enumerator of all the effects.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<ICellEffect> GetEnumerator() => ((IEnumerable<ICellEffect>)Effects).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Effects).GetEnumerator();

    /// <summary>
    /// Adds an effect to the end of the <see cref="Effects"/> collection.
    /// </summary>
    /// <param name="effect">The effect to add.</param>
    public void Add(ICellEffect effect) =>
        Effects.AddLast(effect);
}
