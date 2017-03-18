using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitLog : ImplicitModuleBase
    {
        public ImplicitLog(ImplicitModuleBase source)
        {
            this.Source = source;
        }
        
        public ImplicitModuleBase Source { set; get; }

        public override Double Get(Double x, Double y)
        {
            return Math.Log(this.Source.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return Math.Log(this.Source.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Math.Log(this.Source.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return Math.Log(this.Source.Get(x, y, z, w, u, v));
        }
    }
}
