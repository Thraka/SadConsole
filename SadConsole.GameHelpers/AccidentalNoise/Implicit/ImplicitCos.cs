using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitCos : ImplicitModuleBase
    {
        public ImplicitCos(ImplicitModuleBase source)
        {
            this.Source = source;
        }

        public ImplicitModuleBase Source { get; set; }

        public override Double Get(Double x, Double y)
        {
            return Math.Cos(this.Source.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Math.Cos(this.Source.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Math.Cos(this.Source.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Math.Cos(this.Source.Get(x, y, z, w, u, v));
        }
    }
}
