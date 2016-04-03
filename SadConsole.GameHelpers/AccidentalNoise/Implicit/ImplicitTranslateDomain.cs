using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitTranslateDomain : ImplicitModuleBase
    {
        public ImplicitTranslateDomain(
            ImplicitModuleBase source,
            Double xAxis = 0.00, Double yAxis = 0.00, Double zAxis = 0.00,
            Double wAxis = 0.00, Double uAxis = 0.00, Double vAxis = 0.00)
        {
            this.Source = source;
            this.XAxis = new ImplicitConstant(xAxis);
            this.YAxis = new ImplicitConstant(yAxis);
            this.ZAxis = new ImplicitConstant(zAxis);
            this.WAxis = new ImplicitConstant(wAxis);
            this.UAxis = new ImplicitConstant(uAxis);
            this.VAxis = new ImplicitConstant(vAxis);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase XAxis { get; set; }

        public ImplicitModuleBase YAxis { get; set; }

        public ImplicitModuleBase ZAxis { get; set; }

        public ImplicitModuleBase WAxis { get; set; }

        public ImplicitModuleBase UAxis { get; set; }

        public ImplicitModuleBase VAxis { get; set; }

        public override Double Get(Double x, Double y)
        {
            return this.Source.Get(
                x + this.XAxis.Get(x, y),
                y + this.YAxis.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
            return this.Source.Get(
                x + this.XAxis.Get(x, y, z),
                y + this.YAxis.Get(x, y, z),
                z + this.ZAxis.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            return Source.Get(
                x + this.XAxis.Get(x, y, z, w),
                y + this.YAxis.Get(x, y, z, w),
                z + this.ZAxis.Get(x, y, z, w),
                w + this.WAxis.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return this.Source.Get(
                x + this.XAxis.Get(x, y, z, w, u, v),
                y + this.YAxis.Get(x, y, z, w, u, v),
                z + this.ZAxis.Get(x, y, z, w, u, v),
                w + this.WAxis.Get(x, y, z, w, u, v),
                u + this.UAxis.Get(x, y, z, w, u, v),
                v + this.VAxis.Get(x, y, z, w, u, v));
        }
    }
}