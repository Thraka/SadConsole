using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitTan : ImplicitModuleBase
    {
        public ImplicitTan(ImplicitModuleBase source)
        {
            this.Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override Double Get(Double x, Double y)
        {
            return Math.Tan(this.Source.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Math.Tan(this.Source.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Math.Tan(this.Source.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Math.Tan(this.Source.Get(x, y, z, w, u, v));
        }
    }
}
