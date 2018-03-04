using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonoColor = Microsoft.Xna.Framework.Color;

namespace SadConsole
{
    internal static class Extensions
    {
        private static Dictionary<Font, Surfaces.BasicSurface> FontSurfaces = new Dictionary<Font, Surfaces.BasicSurface>();

        public static Surfaces.BasicSurface GetFontSurface(this Font font)
        {
            if (!FontSurfaces.ContainsKey(font))
            {
                var surface = new Surfaces.BasicSurface(16, font.Rows, font);
                for (int i = 0; i < surface.Cells.Length; i++)
                {
                    surface.Cells[i].Glyph = i;
                    surface.Cells[i].Background = MonoColor.Transparent;
                }
                FontSurfaces[font] = surface;
                return surface;
            }
            else
                return FontSurfaces[font];
        }

        public static void AddArrangeControls(this Control container, params Control[] controls)
        {
            int heightCalculator = 0;

            container.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            foreach (var control in controls)
            {
                control.Top = heightCalculator + control.Margin.Top;
                control.Left = control.Margin.Left;

                heightCalculator = container.Height = control.Bottom + control.Margin.Bottom;
                control.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;   
            }

            foreach (var control in controls)
            {
                container.Controls.Add(control);
                control.Width = container.Width - control.Left - control.Margin.Right;
            }
        }

        public static ColorMatrix ToColorMatrix(this Color color)
        {
            float red = (float)color.R / 255;
            float green = (float)color.G / 255;
            float blue = (float)color.B / 255;
            float alpha = (float)color.A / 255;

            // THIS is still not correct with the "white" foreground font.
            // System.Drawing wants it black to color properly. Hrm...
            float[][] colorMatrixElements = {
                new float[] {red,  0,  0,  0, 0},
                new float[] {0,  green,  0,  0, 0},
                new float[] {0,  0,  blue,  0, 0},
                new float[] {0,  0,  0, alpha, 0},
                new float[] {red, green, blue, 0, 1}};

            return new ColorMatrix(colorMatrixElements);
        }

        public static Color ToDrawingColor(this Microsoft.Xna.Framework.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Microsoft.Xna.Framework.Color ToMonoGameColor(this Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }

        public static Image ToImage(this Microsoft.Xna.Framework.Graphics.RenderTarget2D renderTarget)
        {
            using (var outputStream = new System.IO.MemoryStream())
            {
                renderTarget.SaveAsPng(outputStream, renderTarget.Width, renderTarget.Height);
                return new Bitmap(outputStream);
            }
        }

        public static Rectangle ToDrawingRectangle(this Microsoft.Xna.Framework.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
