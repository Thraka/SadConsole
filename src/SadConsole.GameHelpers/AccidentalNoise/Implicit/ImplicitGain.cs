using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitGain : ImplicitModuleBase
    {
        public ImplicitGain(ImplicitModuleBase source, Double gain = 0.00)
        {
            this.Source = source;
            this.Gain = new ImplicitConstant(gain);
        }

        public ImplicitGain(ImplicitModuleBase source, ImplicitModuleBase gain)
        {
            this.Source = source;
            this.Gain = gain;
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Gain { get; set; }

        public override Double Get(Double x, Double y)
        {
            return Utilities.Gain(this.Gain.Get(x, y), this.Source.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Utilities.Gain(this.Gain.Get(x, y, z), this.Source.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Utilities.Gain(this.Gain.Get(x, y, z, w), this.Source.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Utilities.Gain(this.Gain.Get(x, y, z, w, u, v), this.Source.Get(x, y, z, w, u, v));
        }
    }
}
