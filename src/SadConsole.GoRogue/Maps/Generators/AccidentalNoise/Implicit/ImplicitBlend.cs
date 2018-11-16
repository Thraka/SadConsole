using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitBlend : ImplicitModuleBase
    {
        public ImplicitBlend(ImplicitModuleBase source, Double low = 0.00, Double high = 0.00)
        {
            this.Source = source;
            this.Low = new ImplicitConstant(low);
            this.High = new ImplicitConstant(high);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Low { get; set; }

        public ImplicitModuleBase High { get; set; }

        public override Double Get(Double x, Double y)
        {
            var v1 = this.Low.Get(x, y);
            var v2 = this.High.Get(x, y);
            var blend = (this.Source.Get(x, y) + 1.0) * 0.5;
            return Utilities.Lerp(blend, v1, v2);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var v1 = this.Low.Get(x, y, z);
            var v2 = this.High.Get(x, y, z);
            var blend = this.Source.Get(x, y, z);
            return Utilities.Lerp(blend, v1, v2);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var v1 = this.Low.Get(x, y, z, w);
            var v2 = this.High.Get(x, y, z, w);
            var blend = this.Source.Get(x, y, z, w);
            return Utilities.Lerp(blend, v1, v2);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var v1 = this.Low.Get(x, y, z, w, u, v);
            var v2 = this.High.Get(x, y, z, w, u, v);
            var blend = this.Source.Get(x, y, z, w, u, v);
            return Utilities.Lerp(blend, v1, v2);
        }
    }
}
