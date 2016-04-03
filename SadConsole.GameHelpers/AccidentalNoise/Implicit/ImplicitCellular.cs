using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public class ImplicitCellular : ImplicitModuleBase
    {
        private CellularGenerator generator;

        public readonly Double[] Coefficients = new Double[4];

        public ImplicitCellular(CellularGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException("generator");

            this.generator = generator;
        }

        public CellularGenerator Generator
        {
            get { return this.generator; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this.generator = value;
            }
        }

        public override Double Get(Double x, Double y)
        {
            var c = this.generator.Get(x, y);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var c = this.generator.Get(x, y, z);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var c = this.generator.Get(x, y, z, w);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var c = this.generator.Get(x, y, z, w, u, v);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }
    }
}
