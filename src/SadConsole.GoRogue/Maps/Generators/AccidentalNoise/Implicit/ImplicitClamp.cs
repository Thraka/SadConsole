using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitClamp : ImplicitModuleBase
    {
        public ImplicitClamp(ImplicitModuleBase source, Double low = 0.00, Double high = 1.00)
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
            return Utilities.Clamp(Source.Get(x, y), Low.Get(x, y), High.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Utilities.Clamp(Source.Get(x, y, z), Low.Get(x, y, z), High.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Utilities.Clamp(Source.Get(x, y, z, w), Low.Get(x, y, z, w), High.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Utilities.Clamp(Source.Get(x, y, z, w, u, v), Low.Get(x, y, z, w, u, v), High.Get(x, y, z, w, u, v));
        }
    }
}
