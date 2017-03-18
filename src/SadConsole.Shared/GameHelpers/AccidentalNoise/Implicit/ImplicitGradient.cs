using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitGradient : ImplicitModuleBase
    {
        private Double gradientX0;

        private Double gradientY0;

        private Double gradientZ0;

        private Double gradientW0;

        private Double gradientU0;

        private Double gradientV0;

        private Double gradientX1;

        private Double gradientY1;

        private Double gradientZ1;

        private Double gradientW1;

        private Double gradientU1;

        private Double gradientV1;

        private Double length2;

        private Double length3;

        private Double length4;

        private Double length6;

        public ImplicitGradient(
            Double x0 = 0.00, Double x1 = 1.00, Double y0 = 0.00, Double y1 = 1.00, Double z0 = 0.00, Double z1 = 1.00, 
            Double w0 = 0.00, Double w1 = 1.00, Double u0 = 0.00, Double u1 = 1.00, Double v0 = 0.00, Double v1 = 1.00)
        {
            this.SetGradient(x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, v1);
        }

        public void SetGradient(Double x0, Double x1, Double y0, Double y1)
        {
            this.SetGradient(x0, x1, y0, y1, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(Double x0, Double x1, Double y0, Double y1, Double z0, Double z1)
        {
            this.SetGradient(x0, x1, y0, y1, z0, z1, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(Double x0, Double x1, Double y0, Double y1, Double z0, Double z1, Double w0, Double w1)
        {
            this.SetGradient(x0, x1, y0, y1, z0, z1, w0, w1, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(Double x0, Double x1, Double y0, Double y1, Double z0, Double z1, Double w0, Double w1, Double u0, Double u1, Double v0, Double v1)
        {
            this.gradientX0 = x0;
            this.gradientY0 = y0;
            this.gradientZ0 = z0;
            this.gradientW0 = w0;
            this.gradientU0 = u0;
            this.gradientV0 = v0;

            this.gradientX1 = x1 - x0;
            this.gradientY1 = y1 - y0;
            this.gradientZ1 = z1 - z0;
            this.gradientW1 = w1 - w0;
            this.gradientU1 = u1 - u0;
            this.gradientV1 = v1 - v0;

            this.length2 = (this.gradientX1 * this.gradientX1 + this.gradientY1 * this.gradientY1);
            this.length3 = (this.gradientX1 * this.gradientX1 + this.gradientY1 * this.gradientY1 + this.gradientZ1 * this.gradientZ1);
            this.length4 = (this.gradientX1 * this.gradientX1 + this.gradientY1 * this.gradientY1 + this.gradientZ1 * this.gradientZ1 + this.gradientW1 * this.gradientW1);
            this.length6 = (this.gradientX1 * this.gradientX1 + this.gradientY1 * this.gradientY1 + this.gradientZ1 * this.gradientZ1 + this.gradientW1 * this.gradientW1 + this.gradientU1 * this.gradientU1 + this.gradientV1 * this.gradientV1);
        }

        public override Double Get(Double x, Double y)
        {
            var dx = x - this.gradientX0;
            var dy = y - this.gradientY0;
            var dp = dx * this.gradientX1 + dy * this.gradientY1;
            dp /= this.length2;
            return dp;
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var dx = x - this.gradientX0;
            var dy = y - this.gradientY0;
            var dz = z - this.gradientZ0;
            var dp = dx * this.gradientX1 + dy * this.gradientY1 + dz * this.gradientZ1;
            dp /= this.length3;
            return dp;
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var dx = x - this.gradientX0;
            var dy = y - this.gradientY0;
            var dz = z - this.gradientZ0;
            var dw = w - this.gradientW0;
            var dp = dx * this.gradientX1 + dy * this.gradientY1 + dz * this.gradientZ1 + dw * this.gradientW1;
            dp /= this.length4;
            return dp;
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var dx = x - this.gradientX0;
            var dy = y - this.gradientY0;
            var dz = z - this.gradientZ0;
            var dw = w - this.gradientW0;
            var du = u - this.gradientU0;
            var dv = v - this.gradientV0;
            var dp = dx * this.gradientX1 + dy * this.gradientY1 + dz * this.gradientZ1 + dw * this.gradientW1 + du * this.gradientU1 + dv * this.gradientV1;
            dp /= this.length6;
            return dp;
        }
    }
}
