using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitPow : ImplicitModuleBase
    {
        public ImplicitPow(ImplicitModuleBase source, Double power)
        {
            this.Source = source;
            this.Power = new ImplicitConstant(power);
        }

        public ImplicitPow(ImplicitModuleBase source, ImplicitModuleBase power)
        {
            this.Source = source;
            this.Power = power;
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Power { get; set; }

        public override Double Get(Double x, Double y)
        {
            return Math.Pow(this.Source.Get(x, y), this.Power.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Math.Pow(this.Source.Get(x, y, z), this.Power.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Math.Pow(this.Source.Get(x, y, z, w), this.Power.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Math.Pow(this.Source.Get(x, y, z, w, u, v), this.Power.Get(x, y, z, w, u, v));
        }
    }
}