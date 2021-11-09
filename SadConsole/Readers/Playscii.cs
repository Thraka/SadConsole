using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SadConsole;
using SadRogue.Primitives;

namespace SadConsole.Readers
{
    public class Playscii
    {
        readonly string[] PALETTE_EXTENSIONS;
        const int MAX_COLORS = 1024;

        public string charset;
        public Frame[] frames;
        public int height;
        public string palette;
        public int width;

        public Playscii()
        {
            PALETTE_EXTENSIONS = new string[] { "png", "gif", "bmp" };
        }

        public struct Frame
        {
            public float delay;
            public Layer[] layers;

            public ScreenObject ToSurface(int width, int height, IFont font, Palette colors)
            {
                var output = new ScreenObject();
                if (layers.Length > 0)
                {
                    foreach (var layer in layers)
                        output.Children.Add(layer.ToSurface(width, height, font, colors));
                }
                return output;
            }
        }

        public struct Layer
        {
            public string name;
            public Tile[] tiles;
            public bool visible;

            public ScreenSurface ToSurface(int width, int height, IFont font, Palette colors)
            {
                var output = new ScreenSurface(width, height) { Font = font, IsVisible = visible };
                if (output.Surface.Count != tiles.Length) throw new ArgumentException("Surface size must match number of tiles.");

                for (int y = 0; y < output.Surface.Width; y++)
                    for (int x = 0; x < output.Surface.Height; x++)
                    {
                        var coloredGlyph = tiles[y * output.Surface.Width + x].ToColoredGlyph(output.Font, colors);
                        output.Surface.SetCellAppearance(x, y, coloredGlyph);
                    }

                return output;
            }
        }

        public struct Tile
        {
            public int bg;
            [JsonProperty("char")]
            public int glyph;
            public int fg;
            public byte xform;

            public ColoredGlyph ToColoredGlyph(IFont font, Palette colors)
            {
                if (bg >= colors.Length || fg >= colors.Length) throw new IndexOutOfRangeException("Glyph color out of palette range.");
                if (glyph < 0 || glyph >= font.TotalGlyphs) throw new IndexOutOfRangeException("Glyph index out of font range.");

                Mirror mirror = xform switch
                {
                    1 => Mirror.Horizontal,
                    2 => Mirror.Vertical,
                    _ => Mirror.None
                };
                Color foreground = colors[fg];
                Color background = colors[bg];
                return new ColoredGlyph(foreground, background, glyph, mirror);
            }
        }

        static Palette GetPalette(string fileName)
        {
            var colors = new List<Color>() { new Color(0, 0, 0, 0) };
            using (ITexture image = GameHost.Instance.GetTexture(fileName))
            {
                Point imageSize = (image.Width, image.Height);

                for (var y = 0; y < imageSize.Y; y++)
                {
                    for (var x = 0; x < imageSize.X; x++)
                    {
                        if (colors.Count >= MAX_COLORS)
                            throw new ArgumentException("Playscii palette file contains too many colors.");

                        var color = image.GetPixel(new Point(x, y));
                        if (!colors.Contains(color))
                            colors.Add(color);
                    }
                }
            }
            return new Palette(colors);
        }

        static Playscii GetPlayscii(string fileName)
        {
            using (StreamReader file = File.OpenText(fileName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);
                return (Playscii)o2.ToObject(typeof(Playscii));
            }
        }

        public static (ScreenObject, Palette) ToSurface(string fileName, IFont font)
        {
            ScreenObject output = null;
            Palette palette = null;

            // get the dir name from the file name
            string dirName = Path.GetDirectoryName(fileName);

            // read playscii json file
            if (GetPlayscii(fileName) is Playscii p)
            {
                // read pallete file and generate colors
                palette = GetPalette($"{dirName}/{p.palette}.png");

                // convert the first frame to a screen surface
                if (p.frames.Length > 0)
                    output = p.frames[0].ToSurface(p.width, p.height, font, palette);
            }

            return (output, palette);
        }
    }
}
