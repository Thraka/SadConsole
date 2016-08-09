using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitScaleOffset : ImplicitModuleBase
    {
        public ImplicitScaleOffset(ImplicitModuleBase source, Double scale = 1.00, Double offset = 0.00)
        {
            this.Source = source;
            this.Scale = new ImplicitConstant(scale);
            this.Offset = new ImplicitConstant(offset);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Scale { get; set; }

        public ImplicitModuleBase Offset { get; set; }

        public override Double Get(Double x, Double y)
        {
            return this.Source.Get(x, y) * this.Scale.Get(x, y) + this.Offset.Get(x, y);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return this.Source.Get(x, y, z) * this.Scale.Get(x, y, z) + this.Offset.Get(x, y, z);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return this.Source.Get(x, y, z, w) * this.Scale.Get(x, y, z, w) + this.Offset.Get(x, y, z, w);
        }
            
        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return this.Source.Get(x, y, z, w, u, v) * this.Scale.Get(x, y, z, w, u, v) + this.Offset.Get(x, y, z, w, u, v);
        }
    }
}
