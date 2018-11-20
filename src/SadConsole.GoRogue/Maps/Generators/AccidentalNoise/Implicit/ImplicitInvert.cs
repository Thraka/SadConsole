using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitInvert : ImplicitModuleBase
    {
        public ImplicitInvert(ImplicitModuleBase source)
        {
            this.Source = source;
        }
        
        public ImplicitModuleBase Source { set; get; }

        public override Double Get(Double x, Double y)
        {
            return -this.Source.Get(x, y);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return -this.Source.Get(x, y, z);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return -this.Source.Get(x, y, z, w);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return -this.Source.Get(x, y, z, w, u, v);
        }
    }
}
