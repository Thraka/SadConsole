using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitSphere : ImplicitModuleBase
    {
        public ImplicitSphere(
            Double xCenter = 0.00, Double yCenter = 0.00, Double zCenter = 0.00,
            Double wCenter = 0.00, Double uCenter = 0.00, Double vCenter = 0.00,
            Double radius = 1.00)
        {
            this.XCenter = new ImplicitConstant(xCenter);
            this.YCenter = new ImplicitConstant(yCenter);
            this.ZCenter = new ImplicitConstant(zCenter);
            this.WCenter = new ImplicitConstant(wCenter);
            this.UCenter = new ImplicitConstant(uCenter);
            this.VCenter = new ImplicitConstant(vCenter);
            this.Radius = new ImplicitConstant(radius);
        }

        public ImplicitModuleBase XCenter { get; set; }

        public ImplicitModuleBase YCenter { get; set; }

        public ImplicitModuleBase ZCenter { get; set; }

        public ImplicitModuleBase WCenter { get; set; }

        public ImplicitModuleBase UCenter { get; set; }

        public ImplicitModuleBase VCenter { get; set; }

        public ImplicitModuleBase Radius { get; set; }

        public override Double Get(Double x, Double y)
        {
            var dx = x - this.XCenter.Get(x, y);
            var dy = y - this.YCenter.Get(x, y);
            var len = Math.Sqrt(dx * dx + dy * dy);
            var rad = this.Radius.Get(x, y);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override Double Get(Double x, Double y, Double z)
        {
            var dx = x - this.XCenter.Get(x, y, z);
            var dy = y - this.YCenter.Get(x, y, z);
            var dz = z - this.ZCenter.Get(x, y, z);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            var rad = this.Radius.Get(x, y, z);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
            var dx = x - this.XCenter.Get(x, y, z, w);
            var dy = y - this.YCenter.Get(x, y, z, w);
            var dz = z - this.ZCenter.Get(x, y, z, w);
            var dw = w - this.WCenter.Get(x, y, z, w);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
            var rad = this.Radius.Get(x, y, z, w);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
            var dx = x - this.XCenter.Get(x, y, z, w, u, v);
            var dy = y - this.YCenter.Get(x, y, z, w, u, v);
            var dz = z - this.ZCenter.Get(x, y, z, w, u, v);
            var dw = w - this.WCenter.Get(x, y, z, w, u, v);
            var du = u - this.UCenter.Get(x, y, z, w, u, v);
            var dv = v - this.VCenter.Get(x, y, z, w, u, v);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw + du * du + dv * dv);
            var rad = this.Radius.Get(x, y, z, w, u, v);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }
    }
}
