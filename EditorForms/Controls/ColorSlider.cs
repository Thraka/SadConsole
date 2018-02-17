using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SadConsole.Editor.Controls
{
    public partial class ColorSlider : UserControl
    {
        public ColorSlider()
        {
            InitializeComponent();
        }

        public Color SelectedColor;
        private Color[] colors = new[] { Color.White, Color.Black };

        public Color[] ColorRange
        {
            set
            {
                colors = value;
                pictureBox4.Invalidate();
            }
        }

        private void ColorSlider_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush br = new LinearGradientBrush(e.ClipRectangle, Color.Black, Color.Black, 0, false);
            ColorBlend cb = new ColorBlend();
            //var index = colors.Length;
            //cb.Positions = (from c in colors select index - colors.Length * 1f).ToArray();

            if (colors.Length == 0)
                colors = new[] { Color.Black, Color.White };

            var Stops = new float[colors.Length];
            float stopStrength = 1f / (colors.Length - 1);

            for (int i = 0; i < colors.Length; i++)
            {
                Stops[i] = i * stopStrength;
            }
            cb.Positions = Stops;
            cb.Colors = colors;
            br.InterpolationColors = cb;

            // paint
            e.Graphics.FillRectangle(br, e.ClipRectangle);
            
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            using (var g = pictureBox4.CreateGraphics())
            using (var b = new Bitmap(pictureBox4.Width, pictureBox4.Height, g))
            {
                SelectedColor = b.GetPixel((int)(trackBar1.Value * (1f / pictureBox4.Width)), 2);
            }
        }
    }
}
