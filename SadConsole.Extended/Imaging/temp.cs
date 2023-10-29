//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Processing;
//using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp.Drawing.Processing;
//using SixLabors.ImageSharp.Drawing;

//namespace SadConsole.Imaging;

//public class FontEditor
//{
//    public static void Test()
//    {
//        IFont font = SadConsole.GameHost.Instance.DefaultFont;

//        SadRogue.Primitives.Color[] pixels = font.Image.GetPixels();

//        Span<Rgba32> transformedPixels = System.Runtime.InteropServices.MemoryMarshal.Cast<SadRogue.Primitives.Color, Rgba32>(pixels);
        
//        using Image<Rgba32> texture = Image<Rgba32>.LoadPixelData((ReadOnlySpan<Rgba32>)transformedPixels, font.Image.Width, font.Image.Height);

//        texture.Mutate(context =>
//        {
//            var options = context.Configuration.GetGraphicsOptions();
//            options.Antialias = false;
//            context.Configuration.SetGraphicsOptions(options);


//            // Rectangle from other library I'm using
//            var fontGlyphRect = font.GetGlyphSourceRectangle(1);

//            // Transform into SixLabors rectangle
//            RectangleF rect = new(fontGlyphRect.X, fontGlyphRect.Y, fontGlyphRect.Width, fontGlyphRect.Height);

//            // Convert rect to IPath to clip, then flip that clipped area
//            context.Clip(new RectangularPolygon(rect), context2 => context2.Flip(FlipMode.Vertical));

//        });

//        texture.SaveAsPng("Testingimage.png");
//    }
//}
