using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadConsole.Editor;

namespace SadConsole.Editor.Forms
{
    public partial class GlyphPicker : Form
    {
        public GlyphPicker()
        {
            InitializeComponent();
        }
        
        public int Glyph;
        private Microsoft.Xna.Framework.Rectangle[] glyphRectangles;

        public Image GetGlyphImage()
        {
            var monoRect = glyphRectangles[Glyph];
            Image glyphImage = new Bitmap(monoRect.Width, monoRect.Height);

            using (var g = Graphics.FromImage(glyphImage))
            {
                g.DrawImage(picFontSheet.Image, 0, 0, monoRect.ToDrawingRectangle(), GraphicsUnit.Pixel);
            }

            return glyphImage;
        }

        public void SetFont(Font font, Microsoft.Xna.Framework.Color foreground, Microsoft.Xna.Framework.Color background)
        {
            var surface = DataContext.Instance.GetFontSurface(font);
            surface.DefaultBackground = background;
            surface.DefaultForeground = foreground;
            glyphRectangles = surface.RenderRects;
            for (int i = 0; i < surface.CellCount; i++)
            {
                surface[i].Foreground = foreground;
                surface[i].Background = background;
            }
            var renderer = new Renderers.SurfaceRenderer();
            renderer.Render(surface, true);

            int resizeWidth = this.Width - picFontSheet.Right;
            int resizeHeight = this.Height - picFontSheet.Bottom;
            int buttonResizeHeight = this.Height - btnCancel.Top;
            int buttonResizeWidth = this.Width - btnCancel.Left;

            picFontSheet.Image = surface.LastRenderResult.ToImage();

            this.Width = picFontSheet.Right + resizeWidth;
            this.Height = picFontSheet.Bottom + resizeHeight;
            btnCancel.Location = new Point(this.Width - buttonResizeWidth, this.Height - buttonResizeHeight);
        }

        private void picFontSheet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Glyph = Microsoft.Xna.Framework.PointExtensions.ToIndex(
                            new Microsoft.Xna.Framework.Point(e.X / glyphRectangles[0].Width, e.Y / glyphRectangles[0].Height),
                            16);

                DialogResult = DialogResult.OK;
                Hide();
            }
        }
    }
}
