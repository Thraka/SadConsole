using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitScaleDomain : ImplicitModuleBase
    {
        public ImplicitScaleDomain(ImplicitModuleBase source, 
            Double xScale = 1.00, Double yScale = 1.00, Double zScale = 1.00,
            Double wScale = 1.00, Double uScale = 1.00, Double vScale = 1.00)
        {
            this.Source = source;
            this.XScale = new ImplicitConstant(xScale);
            this.YScale = new ImplicitConstant(yScale);
            this.ZScale = new ImplicitConstant(zScale);
            this.WScale = new ImplicitConstant(wScale);
            this.UScale = new ImplicitConstant(uScale);
            this.VScale = new ImplicitConstant(vScale);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase XScale { get; set; }

        public ImplicitModuleBase YScale { get; set; }

        public ImplicitModuleBase ZScale { get; set; }

        public ImplicitModuleBase WScale { get; set; }

        public ImplicitModuleBase UScale { get; set; }

        public ImplicitModuleBase VScale { get; set; }

        public void SetScales(
            Double xScale = 1.00, Double yScale = 1.00, Double zScale = 1.00,
            Double wScale = 1.00, Double uScale = 1.00, Double vScale = 1.00)
        {
            this.XScale = new ImplicitConstant(xScale);
            this.YScale = new ImplicitConstant(yScale);
            this.ZScale = new ImplicitConstant(zScale);
            this.WScale = new ImplicitConstant(wScale);
            this.UScale = new ImplicitConstant(uScale);
            this.VScale = new ImplicitConstant(vScale);
        }

        public override Double Get(Double x, Double y)
        {
            return this.Source.Get(
                x * this.XScale.Get(x, y), 
                y * this.YScale.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return this.Source.Get(
                x * this.XScale.Get(x, y, z), 
                y * this.YScale.Get(x, y, z), 
                z * this.ZScale.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return this.Source.Get(
                x * this.XScale.Get(x, y, z, w), 
                y * this.YScale.Get(x, y, z, w), 
                z * this.ZScale.Get(x, y, z, w),
                w * this.WScale.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return this.Source.Get(
                x * this.XScale.Get(x, y, z, w, u, v),
                y * this.YScale.Get(x, y, z, w, u, v),
                z * this.ZScale.Get(x, y, z, w, u, v),
                w * this.WScale.Get(x, y, z, w, u, v),
                u * this.UScale.Get(x, y, z, w, u, v),
                v * this.VScale.Get(x, y, z, w, u, v));
        }
    }
}