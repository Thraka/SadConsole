using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitRotateDomain : ImplicitModuleBase
    {
        private readonly Double[,] rotationMatrix = new Double[3, 3];

        public ImplicitRotateDomain(ImplicitModuleBase source, Double x, Double y, Double z, Double angle)
        {
            this.Source = source;
            this.X = new ImplicitConstant(x);
            this.Y = new ImplicitConstant(y);
            this.Z = new ImplicitConstant(z);
            this.Angle = new ImplicitConstant(angle);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase X { get; set; }

        public ImplicitModuleBase Y { get; set; }

        public ImplicitModuleBase Z { get; set; }

        public ImplicitModuleBase Angle { get; set; }

        public void SetAxis(Double x, Double y, Double z)
        {
            this.X = new ImplicitConstant(x);
            this.Y = new ImplicitConstant(y);
            this.Z = new ImplicitConstant(z);
        }

        public override Double Get(Double x, Double y)
        {
            var d = this.Angle.Get(x, y) * 360.0 * 3.14159265 / 180.0;
            var cos2D = Math.Cos(d);
            var sin2D = Math.Sin(d);
            var nx = x * cos2D - y * sin2D;
            var ny = y * cos2D + x * sin2D;
            return this.Source.Get(nx, ny);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            this.CalculateRotMatrix(x, y, z);
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);
            return this.Source.Get(nx, ny, nz);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            this.CalculateRotMatrix(x, y, z, w);
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);
            return this.Source.Get(nx, ny, nz, w);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            this.CalculateRotMatrix(x, y, z, w, u, v);
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);
            return this.Source.Get(nx, ny, nz, w, u, v);
        }

        private void CalculateRotMatrix(Double x, Double y)
        {
            var angle = this.Angle.Get(x, y) * 360.0 * Math.PI / 180.0;
            var ax = this.X.Get(x, y);
            var ay = this.Y.Get(x, y);
            var az = this.Z.Get(x, y);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(Double x, Double y, Double z)
        {
            var angle = this.Angle.Get(x, y, z) * 360.0 * Math.PI / 180.0;
            var ax = this.X.Get(x, y, z);
            var ay = this.Y.Get(x, y, z);
            var az = this.Z.Get(x, y, z);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(Double x, Double y, Double z, Double w)
        {
            var angle = this.Angle.Get(x, y, z, w) * 360.0 * Math.PI / 180.0;
            var ax = this.X.Get(x, y, z, w);
            var ay = this.Y.Get(x, y, z, w);
            var az = this.Z.Get(x, y, z, w);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var angle = this.Angle.Get(x, y, z, w, u, v) * 360.0 * Math.PI / 180.0;
            var ax = this.X.Get(x, y, z, w, u, v);
            var ay = this.Y.Get(x, y, z, w, u, v);
            var az = this.Z.Get(x, y, z, w, u, v);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }
    }
}

