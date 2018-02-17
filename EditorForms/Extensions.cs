using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadConsole.Editor
{
    internal static class Extensions
    {
        internal static void AddArrangeControls(this Control container, params Control[] controls)
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

        internal static ColorMatrix ToColorMatrix(this Color color)
        {
            float red = (float)color.R / 255;
            float green = (float)color.G / 255;
            float blue = (float)color.B / 255;
            float alpha = (float)color.A / 255;


            float[][] colorMatrixElements = {
                new float[] {1,  0,  0,  0, 0},
                new float[] {0,  1,  0,  0, 0},
                new float[] {0,  0,  1,  0, 0},
                new float[] {0,  0,  0,  1, 0},
                new float[] {red, green, blue, alpha, 1}};

            return new ColorMatrix(colorMatrixElements);
        }

        internal static Color ToDrawingColor(this Microsoft.Xna.Framework.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
