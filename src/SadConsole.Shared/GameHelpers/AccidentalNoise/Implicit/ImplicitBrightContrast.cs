using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitBrightContrast : ImplicitModuleBase
    {
        public ImplicitBrightContrast(ImplicitModuleBase source, Double brightness = 0.00, Double contrastThreshold = 0.00, Double contrastFactor = 1.00)
        {
            this.Source = source;
            this.Brightness = new ImplicitConstant(brightness);
            this.ContrastThreshold = new ImplicitConstant(contrastThreshold);
            this.ContrastFactor = new ImplicitConstant(contrastFactor);
        }

        private ImplicitModuleBase Source { get; set; }

        private ImplicitModuleBase Brightness { get; set; }

        private ImplicitModuleBase ContrastThreshold { get; set; }

        private ImplicitModuleBase ContrastFactor { get; set; }

        public override Double Get(Double x, Double y)
        {
            var value = this.Source.Get(x, y);
            // Apply brightness
            value += this.Brightness.Get(x, y);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = this.ContrastThreshold.Get(x, y);
            value -= threshold;
            value *= this.ContrastFactor.Get(x, y);
            value += threshold;
            return value;
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var value = this.Source.Get(x, y, z);
            // Apply brightness
            value += this.Brightness.Get(x, y, z);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = this.ContrastThreshold.Get(x, y, z);
            value -= threshold;
            value *= this.ContrastFactor.Get(x, y, z);
            value += threshold;
            return value;
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var value = this.Source.Get(x, y, z, w);
            // Apply brightness
            value += this.Brightness.Get(x, y, z, w);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = this.ContrastThreshold.Get(x, y, z, w);
            value -= threshold;
            value *= this.ContrastFactor.Get(x, y, z, w);
            value += threshold;
            return value;
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var value = this.Source.Get(x, y, z, w, u, v);
            // Apply brightness
            value += this.Brightness.Get(x, y, z, w, u, v);

            // Subtract contrastThreshold, scale by contrastFactor, add contrastThreshold
            var threshold = this.ContrastThreshold.Get(x, y, z, w, u, v);
            value -= threshold;
            value *= this.ContrastFactor.Get(x, y, z, w, u, v);
            value += threshold;
            return value;
        }
    }
}
