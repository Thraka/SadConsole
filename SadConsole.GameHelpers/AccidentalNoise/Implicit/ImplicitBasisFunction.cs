using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitBasisFunction : ImplicitModuleBase
    {
        private readonly Double[] scale = new Double[4];

        private readonly Double[] offset = new Double[4];

        private InterpolationDelegate interpolator;

        private Noise2DDelegate noise2D;

        private Noise3DDelegate noise3D;

        private Noise4DDelegate noise4D;

        private Noise6DDelegate noise6D;

        private Int32 seed;

        private BasisType basisType;

        private InterpolationType interpolationType;

        private readonly Double[,] rotationMatrix = new Double[3, 3];

        private Double cos2D;

        private Double sin2D;

        public ImplicitBasisFunction(BasisType basisType = BasisType.Gradient, InterpolationType interpolationType = InterpolationType.Quintic)
        {
            this.BasisType = basisType;
            this.InterpolationType = interpolationType;
            this.Seed = (Int32)DateTime.Now.Ticks;
        }

        public override Int32 Seed
        {
            get { return this.seed; }
            set
            {
                this.seed = value;
                var random = new Random(value);

                var ax = random.NextDouble();
                var ay = random.NextDouble();
                var az = random.NextDouble();
                var len = Math.Sqrt(ax * ax + ay * ay + az * az);
                ax /= len;
                ay /= len;
                az /= len;
                SetRotationAngle(ax, ay, az, random.NextDouble() * Math.PI * 2.0);
                var angle = random.NextDouble() * Math.PI * 2.0;
                cos2D = Math.Cos(angle);
                sin2D = Math.Sin(angle);
            }
        }

        public BasisType BasisType
        {
            get { return this.basisType; }
            set
            {
                this.basisType = value;
                switch (this.basisType)
                {
                    case BasisType.Value:
                        this.noise2D = Noise.ValueNoise;
                        this.noise3D = Noise.ValueNoise;
                        this.noise4D = Noise.ValueNoise;
                        this.noise6D = Noise.ValueNoise;
                        break;
                    case BasisType.Gradient:
                        this.noise2D = Noise.GradientNoise;
                        this.noise3D = Noise.GradientNoise;
                        this.noise4D = Noise.GradientNoise;
                        this.noise6D = Noise.GradientNoise;
                        break;
                    case BasisType.GradientValue:
                        this.noise2D = Noise.GradientValueNoise;
                        this.noise3D = Noise.GradientValueNoise;
                        this.noise4D = Noise.GradientValueNoise;
                        this.noise6D = Noise.GradientValueNoise;
                        break;
                    case BasisType.White:
                        this.noise2D = Noise.WhiteNoise;
                        this.noise3D = Noise.WhiteNoise;
                        this.noise4D = Noise.WhiteNoise;
                        this.noise6D = Noise.WhiteNoise;
                        break;
                    case BasisType.Simplex:
                        this.noise2D = Noise.SimplexNoise;
                        this.noise3D = Noise.SimplexNoise;
                        this.noise4D = Noise.SimplexNoise;
                        this.noise6D = Noise.SimplexNoise;
                        break;

                    default:
                        this.noise2D = Noise.GradientNoise;
                        this.noise3D = Noise.GradientNoise;
                        this.noise4D = Noise.GradientNoise;
                        this.noise6D = Noise.GradientNoise;
                        break;
                }
                SetMagicNumbers(this.basisType);
            }
        }

        public InterpolationType InterpolationType
        {
            get { return this.interpolationType; }
            set
            {
                this.interpolationType = value;
                switch (this.interpolationType)
                {
                    case InterpolationType.None: this.interpolator = Noise.NoInterpolation; break;
                    case InterpolationType.Linear: this.interpolator = Noise.LinearInterpolation; break;
                    case InterpolationType.Cubic: this.interpolator = Noise.HermiteInterpolation; break;
                    default: this.interpolator = Noise.QuinticInterpolation; break;
                }
            }
        }

        public override Double Get(Double x, Double y)
        {
            var nx = x * cos2D - y * sin2D;
            var ny = y * cos2D + x * sin2D;

            return this.noise2D(nx, ny, this.seed, this.interpolator);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);

            return this.noise3D(nx, ny, nz, this.seed, this.interpolator);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);

            return this.noise4D(nx, ny, nz, w, this.seed, this.interpolator);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var nx = (this.rotationMatrix[0, 0] * x) + (this.rotationMatrix[1, 0] * y) + (this.rotationMatrix[2, 0] * z);
            var ny = (this.rotationMatrix[0, 1] * x) + (this.rotationMatrix[1, 1] * y) + (this.rotationMatrix[2, 1] * z);
            var nz = (this.rotationMatrix[0, 2] * x) + (this.rotationMatrix[1, 2] * y) + (this.rotationMatrix[2, 2] * z);

            return this.noise6D(nx, ny, nz, w, u, v, this.seed, this.interpolator);
        }

        private void SetRotationAngle(Double x, Double y, Double z, Double angle)
        {
            this.rotationMatrix[0, 0] = 1 + (1 - Math.Cos(angle)) * (x * x - 1);
            this.rotationMatrix[1, 0] = -z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
            this.rotationMatrix[2, 0] = y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;

            this.rotationMatrix[0, 1] = z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
            this.rotationMatrix[1, 1] = 1 + (1 - Math.Cos(angle)) * (y * y - 1);
            this.rotationMatrix[2, 1] = -x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;

            this.rotationMatrix[0, 2] = -y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;
            this.rotationMatrix[1, 2] = x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;
            this.rotationMatrix[2, 2] = 1 + (1 - Math.Cos(angle)) * (z * z - 1);
        }

        private void SetMagicNumbers(BasisType type)
        {
            // This function is a damned hack.
            // The underlying noise functions don't return values in the range [-1,1] cleanly, and the ranges vary depending
            // on basis type and dimensionality. There's probably a better way to correct the ranges, but for now I'm just
            // setting he magic numbers scale and offset manually to empirically determined magic numbers.
            switch (type)
            {
                case BasisType.Value:
                    this.scale[0] = 1.0;
                    this.offset[0] = 0.0;
                    this.scale[1] = 1.0;
                    this.offset[1] = 0.0;
                    this.scale[2] = 1.0;
                    this.offset[2] = 0.0;
                    this.scale[3] = 1.0;
                    this.offset[3] = 0.0;
                    break;

                case BasisType.Gradient:
                    this.scale[0] = 1.86848;
                    this.offset[0] = -0.000118;
                    this.scale[1] = 1.85148;
                    this.offset[1] = -0.008272;
                    this.scale[2] = 1.64127;
                    this.offset[2] = -0.01527;
                    this.scale[3] = 1.92517;
                    this.offset[3] = 0.03393;
                    break;

                case BasisType.GradientValue:
                    this.scale[0] = 0.6769;
                    this.offset[0] = -0.00151;
                    this.scale[1] = 0.6957;
                    this.offset[1] = -0.133;
                    this.scale[2] = 0.74622;
                    this.offset[2] = 0.01916;
                    this.scale[3] = 0.7961;
                    this.offset[3] = -0.0352;
                    break;

                case BasisType.White:
                    this.scale[0] = 1.0;
                    this.offset[0] = 0.0;
                    this.scale[1] = 1.0;
                    this.offset[1] = 0.0;
                    this.scale[2] = 1.0;
                    this.offset[2] = 0.0;
                    this.scale[3] = 1.0;
                    this.offset[3] = 0.0;
                    break;

                default:
                    this.scale[0] = 1.0;
                    this.offset[0] = 0.0;
                    this.scale[1] = 1.0;
                    this.offset[1] = 0.0;
                    this.scale[2] = 1.0;
                    this.offset[2] = 0.0;
                    this.scale[3] = 1.0;
                    this.offset[3] = 0.0;
                    break;
            }
        }
    }
}