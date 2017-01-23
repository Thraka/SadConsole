using Microsoft.Xna.Framework;
using SadConsole.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public static class TextSurfaceExtensions
    {
        public static Matrix GetPositionTransform(this Text.ITextSurfaceRendered surface, Point position, bool usePixelPositioning = false)
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
