using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitConstant : ImplicitModuleBase
    {
        public ImplicitConstant()
        {
            this.Value = 0.00;
        }

        public ImplicitConstant(Double value)
        {
            this.Value = value;
        }

        public Double Value { get;  set; }

        public override Double Get(Double x, Double y)
        {
            return this.Value;
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return this.Value;
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return this.Value;
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return this.Value;
        }
    }
}
