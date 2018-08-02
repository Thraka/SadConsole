using Microsoft.Xna.Framework;
using SadConsole.Renderers;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadConsole
{
    public static class SurfaceBaseExtensions
    {
        public static Matrix GetPositionTransform(this SurfaceBase surface, Point position, bool usePixelPositioning = false)
        {
            Point worldLocation;

            if (usePixelPositioning)
                worldLocation = position;
            else
                worldLocation = new Point(position.X * surface.Font.Size.X, position.Y * surface.Font.Size.Y);

            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
        }
    }
}
