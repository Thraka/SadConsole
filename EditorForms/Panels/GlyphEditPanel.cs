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

namespace SadConsole.Editor.Panels
{
    internal partial class GlyphEditPanel : UserControl
    {
        public static GlyphEditPanel SharedInstance = new GlyphEditPanel(new Model.GlyphItem());

        Model.GlyphItem dataObject;

        public Model.GlyphItem DataObject
        {
            get => dataObject;
            set
            {
                if (dataObject != value)
                {
                    if (dataObject == null || dataObject.Font != value.Font)
                    {
                        dataObject = value;
                        UpdateFont();
                    }
                    else
                    {
                        dataObject = value;
                        RecolorImage();
                    }
                }
            }
        }


        public Image originalImage;
        public Image recoloredImage;

        public Color ForegroundColor
        {
            get { return DataObject.Foreground; }
            set
            {
                DataObject.Foreground = value;
                picForeground.BackColor = value.ToDrawingColor();
                RecolorImage();
            }
        }

        public Color BackgroundColor
        {
            get { return DataObject.Background; }
            set
            {
                DataObject.Background = value;
                picBackground.BackColor = value.ToDrawingColor();
                picFontSheet.BackColor = picBackground.BackColor;
            }
        }

        public GlyphEditPanel(Model.GlyphItem glyph)
        {
            InitializeComponent();
            DataObject = glyph;
            ForegroundColor = glyph.Foreground;
            BackgroundColor = glyph.Background;
            //SadConsole.Serializer
        }

        private void UpdateFont()
        {
            if (originalImage != null)
            {
                originalImage.Dispose();
                recoloredImage.Dispose();
                picFontSheet.Image = null;
            }

            originalImage = Image.FromFile(dataObject.Font.Master.LoadedFilePath);
            recoloredImage = new Bitmap(originalImage);

            RecolorImage();
        }

        private void RecolorImage()
        {
            recoloredImage.Save("test2.png");

            using (var g = Graphics.FromImage(recoloredImage))
            {
                ImageAttributes imageAttributes = new ImageAttributes();
                var color = ForegroundColor.ToDrawingColor();
                var matrix = ForegroundColor.ToDrawingColor().ToColorMatrix();

                matrix.Matrix40 = color.R / 255f;
                matrix.Matrix41 = color.G / 255f;
                matrix.Matrix42 = color.B / 255f;
                matrix.Matrix33 = color.A / 255f;

                imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                
                g.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            
            picFontSheet.Image = recoloredImage;
        }

        public GlyphEditPanel()
        {
            InitializeComponent();
        }

        private void picForeground_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new ColorDialog();
            colorDialog.SolidColorOnly = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                ForegroundColor = colorDialog.Color.ToMonoGameColor();

            //Forms.ColorPicker picker = new Forms.ColorPicker();
            //picker.ShowDialog(this.ParentForm);
        }

        private void picBackground_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new ColorDialog();
            colorDialog.SolidColorOnly = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                BackgroundColor = colorDialog.Color.ToMonoGameColor();
        }
    }
}
