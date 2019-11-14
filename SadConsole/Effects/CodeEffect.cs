using System;

namespace SadConsole.Effects
{
    // Not sure if I should make this serailizable... 
    internal class CodeEffect : CellEffectBase
    {
        public string Id;
        public object Tag;
        public bool UseDuration;
        public double Duration;

        private double timeElapsed;
        private readonly Func<CodeEffect, ColoredGlyph, EffectsManager.ColoredGlyphState, bool> _applyAction;
        private readonly Action<CodeEffect, double> _updateAction;
        private readonly Action<CodeEffect> _restartAction;

        public CodeEffect(string id, Func<CodeEffect, ColoredGlyph, EffectsManager.ColoredGlyphState, bool> apply, Action<CodeEffect, double> update, Action<CodeEffect> restart) =>
            (Id, _applyAction, _updateAction, _restartAction) = (id, apply, update, restart);

        public override bool ApplyToCell(ColoredGlyph cell, EffectsManager.ColoredGlyphState originalState) =>
            _applyAction(this, cell, originalState);

        public override ICellEffect Clone() => throw new NotSupportedException();

        public override void Update(double timeElapsed)
        {
            this.timeElapsed += timeElapsed;

            if (this.timeElapsed >= Duration)
            {
                IsFinished = true;
            }

            _updateAction(this, timeElapsed);
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
