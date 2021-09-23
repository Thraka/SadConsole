using System;

namespace SadConsole.Effects
{
    // Not sure if I should make this serailizable... 
    public class CodeEffect : CellEffectBase
    {
        /// <summary>
        /// A user defined identifier of the effect.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// An object associated with this effect.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// When <see langword="true" />, uses the <see cref="Duration"/> timer to stop this effect; otherwise <see langword="false" />.
        /// </summary>
        public bool UseDuration { get; set; }

        /// <summary>
        /// The amount of time this effect runs for in seconds.
        /// </summary>
        public System.TimeSpan Duration { get; set; }

        private readonly Func<CodeEffect, ColoredGlyph, ColoredGlyphState, bool> _applyAction;
        private readonly Action<CodeEffect, System.TimeSpan> _updateAction;
        private readonly Action<CodeEffect> _restartAction;

        public CodeEffect(string id, Func<CodeEffect, ColoredGlyph, ColoredGlyphState, bool> apply, Action<CodeEffect, System.TimeSpan> update, Action<CodeEffect> restart) =>
            (Id, _applyAction, _updateAction, _restartAction) = (id, apply, update, restart);

        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState) =>
            _applyAction(this, cell, originalState);

        public override ICellEffect Clone() => throw new NotSupportedException();

        public override void Update(System.TimeSpan delta)
        {
            _timeElapsed += delta;

            if (_timeElapsed >= Duration)
            {
                IsFinished = true;
            }

            _updateAction(this, delta);
        }

        public override void Restart()
        {
            base.Restart();
            _restartAction(this);
        }

        public override string ToString() =>
            string.Format("CODE-{0}", Id);
    }
}
