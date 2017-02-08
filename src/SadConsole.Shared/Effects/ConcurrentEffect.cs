namespace SadConsole.Effects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;

    /// <summary>
    /// Allows more than one effect to be processed and applied to a cell at the same time.
    /// </summary>
    [DataContract]
    public class ConcurrentEffect : CellEffectBase
    {
        [DataMember]
        private IEnumerable<ICellEffect> _effects;

        /// <summary>
        /// The effects to be processed at the same time. Cannot be set to null.
        /// </summary>
        public IEnumerable<ICellEffect> Effects
        {
            get { return _effects; }
            set
            {
                if (value == null)
                    throw new NullReferenceException("Effects cannot be set to null.");

                _effects = value;
            }
        }

        public ConcurrentEffect()
        {
            _effects = new List<ICellEffect>();
        }

        public override bool Apply(Cell cell)
        {
            bool returnResult = false;

            foreach (var item in _effects)
            {
                if (item.Apply(cell))
                    returnResult = true;
            }

            return returnResult;
        }

        public override void Clear(Cell cell)
        {
            foreach (var item in _effects)
            {
                item.Clear(cell);
            }
        }

        public override ICellEffect Clone()
        {
            var effect = new ConcurrentEffect();
            var list = new List<ICellEffect>(10);
            foreach (var item in _effects)
            {
                list.Add(item.Clone());
            }
            effect.Effects = list;
            return effect;
        }

        //public override bool Equals(ICellEffect other)
        //{
        //    if (other is ConcurrentEffect)
        //    {
        //        if (base.Equals(other))
        //        {
        //            var effect = (ConcurrentEffect)other;

        //            var effects1 = this.Effects.ToList();
        //            var effects2 = effect.Effects.ToList();

        //            if (effects1.Count == effects2.Count)
        //            {
        //                for (int i = 0; i < effects1.Count; i++)
        //                {
        //                    if (!effects1[i].Equals(effects2[i]))
        //                        return false;
        //                }

        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        public override void Restart()
        {
            base.Restart();

            foreach (var item in _effects)
            {
                item.Restart();
            }
        }

        public override void Update(double timeElapsed)
        {
            bool finished = true;

            foreach (var item in _effects)
            {
                item.Update(timeElapsed);

                if (!item.IsFinished)
                    finished = false;
            }

            IsFinished = finished;
        }
    }
}
