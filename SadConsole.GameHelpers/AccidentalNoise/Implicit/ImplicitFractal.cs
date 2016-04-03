using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitFractal : ImplicitModuleBase
    {
        private readonly ImplicitBasisFunction[] basisFunctions = new ImplicitBasisFunction[Noise.MAX_SOURCES];

        private readonly ImplicitModuleBase[] sources = new ImplicitModuleBase[Noise.MAX_SOURCES];

        private readonly Double[] expArray = new Double[Noise.MAX_SOURCES];

        private readonly Double[,] correct = new Double[Noise.MAX_SOURCES, 2];

        private Int32 seed;

        private FractalType type;

        private Int32 octaves;

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType)
        {
            this.Octaves = 8;
            this.Frequency = 1.00;
            this.Lacunarity = 2.00;
            this.Type = fractalType;
            this.SetAllSourceTypes(basisType, interpolationType);
            this.ResetAllSources();
        }

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType, Int32 octaves, double frequency, Int32 seed)
        {
            this.seed = seed;
            this.Octaves = octaves;
            this.Frequency = frequency;
            this.Octaves = 8;
            this.Frequency = 1.00;
            this.Lacunarity = 2.00;
            this.Type = fractalType;
            this.SetAllSourceTypes(basisType, interpolationType);
            this.ResetAllSources();
        }

        public override Int32 Seed
        {
            get { return this.seed; }
            set
            {
                this.seed = value;
                for (var source = 0; source < Noise.MAX_SOURCES; source += 1)
                    this.sources[source].Seed = ((this.seed + source * 300));
            }
        }

        public FractalType Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
                switch (this.type)
                {
                    case FractalType.FractionalBrownianMotion:
                        this.H = 1.00;
                        this.Gain = 0.00;
                        this.Offset = 0.00;
                        this.FractionalBrownianMotion_CalculateWeights();
                        break;
                    case FractalType.RidgedMulti:
                        this.H = 0.90;
                        this.Gain = 2.00;
                        this.Offset = 1.00;
                        this.RidgedMulti_CalculateWeights();
                        break;
                    case FractalType.Billow:
                        this.H = 1.00;
                        this.Gain = 0.00;
                        this.Offset = 0.00;
                        this.Billow_CalculateWeights();
                        break;
                    case FractalType.Multi:
                        this.H = 1.00;
                        this.Gain = 0.00;
                        this.Offset = 0.00;
                        this.Multi_CalculateWeights();
                        break;
                    case FractalType.HybridMulti:
                        this.H = 0.25;
                        this.Gain = 1.00;
                        this.Offset = 0.70;
                        this.HybridMulti_CalculateWeights();
                        break;
                    default:
                        this.H = 1.00;
                        this.Gain = 0.00;
                        this.Offset = 0.00;
                        this.FractionalBrownianMotion_CalculateWeights();
                        break;
                }
            }
        }

        public Int32 Octaves
        {
            get {return this.octaves; }
            set
            {
                if (value >= Noise.MAX_SOURCES)
                    value = Noise.MAX_SOURCES - 1;
                this.octaves = value;
            }
        }

        public Double Frequency { get; set; }

        public Double Lacunarity { get; set; }

        public Double Gain { get; set; }

        public Double Offset { get; set; }

        public Double H { get; set; }

        public void SetAllSourceTypes(BasisType newBasisType, InterpolationType newInterpolationType)
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                this.basisFunctions[i] = new ImplicitBasisFunction(newBasisType, newInterpolationType);
            }
        }

        public void SetSourceType(Int32 which, BasisType newBasisType, InterpolationType newInterpolationType)
        {
            if (which >= Noise.MAX_SOURCES || which < 0) return;

            this.basisFunctions[which].BasisType = newBasisType;
            this.basisFunctions[which].InterpolationType = newInterpolationType;
        }

        public void SetSourceOverride(Int32 which, ImplicitModuleBase newSource)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return;

            this.sources[which] = newSource;
        }

        public void ResetSource(Int32 which)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return;

            this.sources[which] = this.basisFunctions[which];
        }

        public void ResetAllSources()
        {
            for (var c = 0; c < Noise.MAX_SOURCES; ++c)
                this.sources[c] = this.basisFunctions[c];
        }

        public ImplicitBasisFunction GetBasis(Int32 which)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return null;

            return this.basisFunctions[which];
        }

        public override Double Get(Double x, Double y)
        {
            Double v;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;
                case FractalType.RidgedMulti:
                    v = RidgedMulti_Get(x, y);
                    break;
                case FractalType.Billow:
                    v = Billow_Get(x, y);
                    break;
                case FractalType.Multi:
                    v = Multi_Get(x, y);
                    break;
                case FractalType.HybridMulti:
                    v = HybridMulti_Get(x, y);
                    break;
                default:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;
            }
            return Utilities.Clamp(v, -1.0, 1.0);
        }

        public override Double Get(Double x, Double y, Double z)
        {
            Double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            Double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            Double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w, u, v);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w, u, v);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w, u, v);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w, u, v);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;
            }

            return Utilities.Clamp(val, -1.0, 1.0);
        }


        private void FractionalBrownianMotion_CalculateWeights()
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.00;
            var maxvalue = 0.00;
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const Double a = -1.0;
                const Double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void RidgedMulti_CalculateWeights()
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.00;
            var maxvalue = 0.00;
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += (Offset - 1.0) * (Offset - 1.0) * expArray[i];
                maxvalue += (Offset) * (Offset) * expArray[i];

                const Double a = -1.0;
                const Double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void Billow_CalculateWeights()
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 0.0;
            var maxvalue = 0.0;
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const Double a = -1.0;
                const Double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void Multi_CalculateWeights()
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            var minvalue = 1.0;
            var maxvalue = 1.0;
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue *= -1.0 * expArray[i] + 1.0;
                maxvalue *= 1.0 * expArray[i] + 1.0;

                const Double a = -1.0;
                const Double b = 1.0;
                var scale = (b - a) / (maxvalue - minvalue);
                var bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void HybridMulti_CalculateWeights()
        {
            for (var i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                expArray[i] = Math.Pow(Lacunarity, -i * H);
            }

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            const double a = -1.0;
            const double b = 1.0;

            var minvalue = Offset - 1.0;
            var maxvalue = Offset + 1.0;
            var weightmin = Gain * minvalue;
            var weightmax = Gain * maxvalue;

            var scale = (b - a) / (maxvalue - minvalue);
            var bias = a - minvalue * scale;
            correct[0, 0] = scale;
            correct[0, 1] = bias;


            for (var i = 1; i < Noise.MAX_SOURCES; ++i)
            {
                if (weightmin > 1.00) weightmin = 1.00;
                if (weightmax > 1.00) weightmax = 1.00;

                var signal = (Offset - 1.0) * expArray[i];
                minvalue += signal * weightmin;
                weightmin *= Gain * signal;

                signal = (Offset + 1.0) * expArray[i];
                maxvalue += signal * weightmax;
                weightmax *= Gain * signal;


                scale = (b - a) / (maxvalue - minvalue);
                bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }


        private Double FractionalBrownianMotion_Get(Double x, Double y)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;


            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
            }

            return value;
        }

        private Double FractionalBrownianMotion_Get(Double x, Double y, Double z)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value;
        }

        private Double FractionalBrownianMotion_Get(Double x, Double y, Double z, Double w)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double FractionalBrownianMotion_Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private Double Multi_Get(Double x, Double y)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;

            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Multi_Get(Double x, Double y, Double z, Double w)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Multi_Get(Double x, Double y, Double z)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Multi_Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w, u, v) * expArray[i] + 1.00;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private Double Billow_Get(Double x, Double y)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;

            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Billow_Get(Double x, Double y, Double z, Double w)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Billow_Get(Double x, Double y, Double z)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double Billow_Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private Double RidgedMulti_Get(Double x, Double y)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;

            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double RidgedMulti_Get(Double x, Double y, Double z, Double w)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double RidgedMulti_Get(Double x, Double y, Double z)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double RidgedMulti_Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (var i = 0; i < octaves; ++i)
            {
                var signal = sources[i].Get(x, y, z, w, u, v);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private Double HybridMulti_Get(Double x, Double y)
        {
            x *= Frequency;
            y *= Frequency;

            var value = sources[0].Get(x, y) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;

            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double HybridMulti_Get(Double x, Double y, Double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            var value = sources[0].Get(x, y, z) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double HybridMulti_Get(Double x, Double y, Double z, Double w)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            var value = sources[0].Get(x, y, z, w) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z, w) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private Double HybridMulti_Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            var value = sources[0].Get(x, y, z, w, u, v) + Offset;
            var weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;
            u *= Lacunarity;
            v *= Lacunarity;

            for (var i = 1; i < octaves; ++i)
            {
                if (weight > 1.0) weight = 1.0;
                var signal = (sources[i].Get(x, y, z, w, u, v) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }
    }
}
