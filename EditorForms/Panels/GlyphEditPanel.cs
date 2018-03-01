using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = Microsoft.Xna.Framework.Color;
using System.Drawing.Imaging;
using SadConsole.Editor.Model;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Editor.Panels
{
    internal partial class GlyphEditPanel : UserControl
    {
        //public static GlyphEditPanel SharedInstance = new GlyphEditPanel(new Model.GlyphItem());

        Model.GlyphItem dataObject;

        public Model.GlyphItem DataObject
        {
            get => dataObject;
            set
            {
                dataObject = value;
                UpdateFont();
            }
        }
        
        public Image recoloredImage;

        public Color ForegroundColor
        {
            get { return DataObject.Foreground; }
            set
            {
                DataObject.Foreground = value;
                UpdateFont();
            }
        }

        public Color BackgroundColor
        {
            get { return DataObject.Background; }
            set
            {
                DataObject.Background = value;
                UpdateFont();
            }
        }

        public GlyphEditPanel(Model.GlyphItem glyph)
        {
            InitializeComponent();
            DataObject = glyph;
            ForegroundColor = glyph.Foreground;
            BackgroundColor = glyph.Background;
        }

        private void UpdateFont()
        {
            picForeground.BackColor = DataObject.Foreground.ToDrawingColor();
            picBackground.BackColor = DataObject.Background.ToDrawingColor();

            Forms.GlyphPicker form = new Forms.GlyphPicker();
            form.SetFont(dataObject.Font, dataObject.Foreground, dataObject.Background);
            form.Glyph = dataObject.Glyph;
            picGlyph.Image = form.GetGlyphImage();
            picGlyph.BackColor = dataObject.Background.ToDrawingColor();
            form.Dispose();
        }

        public GlyphEditPanel()
        {
            InitializeComponent();
        }

        private void picForeground_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                    ForegroundColor = colorDialog.Color.ToMonoGameColor();
            }
        }

        private void picBackground_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                    BackgroundColor = colorDialog.Color.ToMonoGameColor();
            }
        }

        private void picGlyph_Click(object sender, EventArgs e)
        {
            using (var form = new Forms.GlyphPicker())
            {
                form.SetFont(dataObject.Font, ForegroundColor, BackgroundColor);
                if (form.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    dataObject.Glyph = form.Glyph;
                    UpdateFont();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
