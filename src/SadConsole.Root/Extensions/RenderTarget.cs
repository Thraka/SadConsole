using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class RenderTargetExtensions
    {
        public static void Save(this RenderTarget2D target, string path)
        {
            using (var stream = System.IO.File.OpenWrite(path))
                target.SaveAsPng(stream, target.Bounds.Width, target.Bounds.Height);
        }
    }
}
