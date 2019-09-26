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
        private readonly Func<CodeEffect, Cell, bool> applyAction;
        private readonly Action<CodeEffect, Cell> clearAction;
        private readonly Action<CodeEffect, double, double> updateAction;
        private readonly Action<CodeEffect> restartAction;

        public CodeEffect(string id, Action<CodeEffect, Cell> apply, Action<CodeEffect, Cell> clear, Action<CodeEffect, double, double> update, Action<CodeEffect> restart) => Id = id;

        public override bool UpdateCell(Cell cell) => applyAction(this, cell);

        public override void ClearCell(Cell cell) => clearAction(this, cell);

        public override ICellEffect Clone() => throw new NotSupportedException();

        public override void Update(double timeElapsed)
        {
            this.timeElapsed += timeElapsed;

            if (this.timeElapsed >= Duration)
            {
                IsFinished = true;
            }

            updateAction(this, timeElapsed, this.timeElapsed);
        }

        public override void Restart()
        {
            base.Restart();
            restartAction(this);
        }

        public override string ToString() => string.Format("CODE-{0}", Id);
    }
}
