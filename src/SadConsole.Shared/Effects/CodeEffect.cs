using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Effects
{
    // Not sure if I should make this serailizable... 
    class CodeEffect : CellEffectBase
    {
        public string Id;
        public object Tag;
        public bool UseDuration;
        public double Duration;

        private double timeElapsed;

        Func<CodeEffect, Cell, bool> applyAction;
        Action<CodeEffect, Cell> clearAction;
        Action<CodeEffect, double, double> updateAction;
        Action<CodeEffect> restartAction;

        public CodeEffect(string id, Action<CodeEffect, Cell> apply, Action<CodeEffect, Cell> clear, Action<CodeEffect, double, double> update, Action<CodeEffect> restart)
        {
            Id = id;
        }

        public override bool Apply(Cell cell)
        {
            return applyAction(this, cell);
        }

        public override void Clear(Cell cell)
        {
            clearAction(this, cell);
        }

        public override ICellEffect Clone()
        {
            throw new NotSupportedException();
        }

        public override void Update(double timeElapsed)
        {
            this.timeElapsed += timeElapsed;

            if (this.timeElapsed >= Duration)
                IsFinished = true;

            updateAction(this, timeElapsed, this.timeElapsed);
        }

        public override void Restart()
        {
            base.Restart();
            restartAction(this);
        }

        public override string ToString()
        {
            return string.Format("CODE-{0}", Id);
        }
    }
}
