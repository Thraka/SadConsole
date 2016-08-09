using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public static class Mapping
    {
        private const Double PI2 = Math.PI * 2.0;

        public static void Map2D(MappingMode mappingMode, Double[,] array, ImplicitModuleBase module, MappingRanges ranges, Double z)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var p = x / (Double)width;
                    var q = y / (Double)height;
                    Double r;
                    Double nx;
                    Double ny;
                    Double nz;
                    Double nw;
                    Double nu;
                    Double dx;
                    Double dy;
                    Double dz;
                    var val = 0.00;
                    switch (mappingMode)
                    {
                        case MappingMode.SeamlessNone:
                            nx = ranges.MapX0 + p * (ranges.MapX1 - ranges.MapX0);
                            ny = ranges.MapY0 + q * (ranges.MapY1 - ranges.MapY0);
                            nz = z;
                            val = module.Get(nx, ny, nz);
                            break;
                        case MappingMode.SeamlessX:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.MapY1 - ranges.MapY0;
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.MapY0 + q * dy;
                            nw = z;
                            val = module.Get(nx, ny, nz, nw);
                            break;
                        case MappingMode.SeamlessY:
                            dx = ranges.MapX1 - ranges.MapX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            nx = ranges.MapX0 + p * dx;
                            ny = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            nz = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            nw = z;
                            val = module.Get(nx, ny, nz, nw);
                            break;
                        case MappingMode.SeamlessZ:
                            dx = ranges.MapX1 - ranges.MapX0;
                            dy = ranges.MapY1 - ranges.MapY0;
                            dz = ranges.LoopZ1 - ranges.LoopZ0;
                            nx = ranges.MapX0 + p * dx;
                            ny = ranges.MapY0 + p * dy;
                            r = (z - ranges.MapZ0) / (ranges.MapZ1 - ranges.MapZ0);
                            var zval = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                            nz = ranges.LoopZ0 + Math.Cos(zval * PI2) * dz / PI2;
                            nw = ranges.LoopZ0 + Math.Sin(zval * PI2) * dz / PI2;
                            val = module.Get(nx, ny, nz, nw);
                            break;
                        case MappingMode.SeamlessXY:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            nw = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            nu = z;
                            val = module.Get(nx, ny, nz, nw, nu, 0);
                            break;
                        case MappingMode.SeamlessXZ:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.MapY1 - ranges.MapY0;
                            dz = ranges.LoopZ1 - ranges.LoopZ0;
                            r = (z - ranges.MapZ0) / (ranges.MapZ1 - ranges.MapZ0);
                            var xzval = r * (ranges.MapX1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.MapY0 + q * dy;
                            nw = ranges.LoopZ0 + Math.Cos(xzval * PI2) * dz / PI2;
                            nu = ranges.LoopZ0 + Math.Sin(xzval * PI2) * dz / PI2;
                            val = module.Get(nx, ny, nz, nw, nu, 0);
                            break;
                        case MappingMode.SeamlessYZ:
                            dx = ranges.MapX1 - ranges.MapX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            dz = ranges.LoopZ1 - ranges.LoopZ0;
                            r = (z - ranges.MapZ0) / (ranges.MapZ1 - ranges.MapZ0);
                            var yzval = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            nx = ranges.MapX0 + p * dx;
                            ny = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            nz = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            nw = ranges.LoopZ0 + Math.Cos(yzval * PI2) * dz / PI2;
                            nu = ranges.LoopZ0 + Math.Sin(yzval * PI2) * dz / PI2;
                            val = module.Get(nx, ny, nz, nw, nu, 0);
                            break;
                        case MappingMode.SeamlessXYZ:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            dz = ranges.LoopZ1 - ranges.LoopZ0;
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            r = (z - ranges.MapZ0) / (ranges.MapZ1 - ranges.MapZ0);
                            var xyzval = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            nw = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            nu = ranges.LoopZ0 + Math.Cos(xyzval * PI2) * dz / PI2;
                            double nv = ranges.LoopZ0 + Math.Sin(xyzval * PI2) * dz / PI2;
                            val = module.Get(nx, ny, nz, nw, nu, nv);
                            break;
                    }
                    array[x, y] = val;
                }
            }
        }

        public static void Map2DNoZ(MappingMode mappingMode, Double[,] array, ImplicitModuleBase module, MappingRanges ranges)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    var p = x / (Double)width;
                    var q = y / (Double)height;
                    Double nx;
                    Double ny;
                    Double nz;
                    Double dx;
                    Double dy;
                    var val = 0.00;
                    switch (mappingMode)
                    {
                        case MappingMode.SeamlessNone:
                            nx = ranges.MapX0 + p * (ranges.MapX1 - ranges.MapX0);
                            ny = ranges.MapY0 + q * (ranges.MapY1 - ranges.MapY0);
                            val = module.Get(nx, ny);
                            break;
                        case MappingMode.SeamlessX:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.MapY1 - ranges.MapY0;
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.MapY0 + q * dy;
                            val = module.Get(nx, ny, nz);
                            break;
                        case MappingMode.SeamlessY:
                            dx = ranges.MapX1 - ranges.MapX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            nx = ranges.MapX0 + p * dx;
                            ny = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            nz = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            val = module.Get(nx, ny, nz);
                            break;

                        case MappingMode.SeamlessXY:
                            dx = ranges.LoopX1 - ranges.LoopX0;
                            dy = ranges.LoopY1 - ranges.LoopY0;
                            p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                            q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                            nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                            ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                            nz = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                            Double nw = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                            val = module.Get(nx, ny, nz, nw);
                            break;
                    }
                    array[x, y] = val;
                }
            }
        }

        public static void Map3D(MappingMode mappingMode, Double[, ,] array, ImplicitModuleBase module, MappingRanges ranges)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            var depth = array.GetLength(2);

            for (var x = 0; x < width; ++x)
            {
                for (var y = 0; y < height; ++y)
                {
                    for (var z = 0; z < depth; ++z)
                    {
                        var p = x / (Double)width;
                        var q = y / (Double)height;
                        var r = z / (Double)depth;
                        Double nx;
                        Double ny;
                        Double nz;
                        Double nw;
                        Double nu;
                        Double dx;
                        Double dy;
                        Double dz;
                        var val = 0.0;

                        switch (mappingMode)
                        {
                            case MappingMode.SeamlessNone:
                                dx = ranges.MapX1 - ranges.MapX0;
                                dy = ranges.MapY1 - ranges.MapY0;
                                dz = ranges.MapZ1 - ranges.MapZ0;
                                nx = ranges.MapX0 + p * dx;
                                ny = ranges.MapY0 + q * dy;
                                nz = ranges.MapZ0 + r * dz;
                                val = module.Get(nx, ny, nz);
                                break;
                            case MappingMode.SeamlessX:
                                dx = ranges.LoopX1 - ranges.LoopX0;
                                dy = ranges.MapY1 - ranges.MapY0;
                                dz = ranges.MapZ1 - ranges.MapZ0;
                                p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                                nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                                ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                                nz = ranges.MapY0 + q * dy;
                                nw = ranges.MapZ0 + depth * dz;
                                val = module.Get(nx, ny, nz, nw);
                                break;
                            case MappingMode.SeamlessY:
                                dx = ranges.MapX1 - ranges.MapX0;
                                dy = ranges.LoopY1 - ranges.LoopY0;
                                dz = ranges.MapZ1 - ranges.MapZ0;
                                q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                                nx = ranges.MapX0 + p * dx;
                                ny = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                                nz = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                                nw = ranges.MapZ0 + r * dz;
                                val = module.Get(nx, ny, nz, nw);
                                break;
                            case MappingMode.SeamlessZ:
                                dx = ranges.MapX1 - ranges.MapX0;
                                dy = ranges.MapY1 - ranges.MapY0;
                                dz = ranges.LoopZ1 - ranges.LoopZ0;
                                r = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                                nx = ranges.MapX0 + p * dx;
                                ny = ranges.MapY0 + q * dy;
                                nz = ranges.LoopZ0 + Math.Cos(r * PI2) * dz / PI2;
                                nw = ranges.LoopZ0 + Math.Sin(r * PI2) * dz / PI2;
                                val = module.Get(nx, ny, nz, nw);
                                break;
                            case MappingMode.SeamlessXY:
                                dx = ranges.LoopX1 - ranges.LoopX0;
                                dy = ranges.LoopY1 - ranges.LoopY0;
                                dz = ranges.MapZ1 - ranges.MapZ0;
                                p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                                q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                                nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                                ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                                nz = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                                nw = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                                nu = ranges.MapZ0 + r * dz;
                                val = module.Get(nx, ny, nz, nw, nu, 0);
                                break;
                            case MappingMode.SeamlessXZ:
                                dx = ranges.LoopX1 - ranges.LoopX0;
                                dy = ranges.MapY1 - ranges.MapY0;
                                dz = ranges.LoopZ1 - ranges.LoopZ0;
                                p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                                r = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                                nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                                ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                                nz = ranges.MapY0 + q * dy;
                                nw = ranges.LoopZ0 + Math.Cos(r * PI2) * dz / PI2;
                                nu = ranges.LoopZ0 + Math.Sin(r * PI2) * dz / PI2;
                                val = module.Get(nx, ny, nz, nw, nu, 0);
                                break;
                            case MappingMode.SeamlessYZ:
                                dx = ranges.MapX1 - ranges.MapX0;
                                dy = ranges.LoopY1 - ranges.LoopY0;
                                dz = ranges.LoopZ1 - ranges.LoopZ0;
                                q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                                r = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                                nx = ranges.MapX0 + p * dx;
                                ny = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                                nz = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                                nw = ranges.LoopZ0 + Math.Cos(r * PI2) * dz / PI2;
                                nu = ranges.LoopZ0 + Math.Sin(r * PI2) * dz / PI2;
                                val = module.Get(nx, ny, nz, nw, nu, 0);
                                break;
                            case MappingMode.SeamlessXYZ:
                                dx = ranges.LoopX1 - ranges.LoopX0;
                                dy = ranges.LoopY1 - ranges.LoopY0;
                                dz = ranges.LoopZ1 - ranges.LoopZ0;
                                p = p * (ranges.MapX1 - ranges.MapX0) / (ranges.LoopX1 - ranges.LoopX0);
                                q = q * (ranges.MapY1 - ranges.MapY0) / (ranges.LoopY1 - ranges.LoopY0);
                                r = r * (ranges.MapZ1 - ranges.MapZ0) / (ranges.LoopZ1 - ranges.LoopZ0);
                                nx = ranges.LoopX0 + Math.Cos(p * PI2) * dx / PI2;
                                ny = ranges.LoopX0 + Math.Sin(p * PI2) * dx / PI2;
                                nz = ranges.LoopY0 + Math.Cos(q * PI2) * dy / PI2;
                                nw = ranges.LoopY0 + Math.Sin(q * PI2) * dy / PI2;
                                nu = ranges.LoopZ0 + Math.Cos(r * PI2) * dz / PI2;
                                Double nv = ranges.LoopZ0 + Math.Sin(r * PI2) * dz / PI2;
                                val = module.Get(nx, ny, nz, nw, nu, nv);
                                break;
                        }
                        array[x, y, z] = val;
                    }
                }
            }
        }
    }
}