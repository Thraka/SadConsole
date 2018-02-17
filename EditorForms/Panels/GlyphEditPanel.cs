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

            //originalImage = Image.FromFile(dataObject.Font.Master.LoadedFilePath);
            //recoloredImage = new Bitmap(originalImage);

            RecolorImage();
        }

        private void RecolorImage()
        {
            if (recoloredImage != null)
                recoloredImage.Dispose();

            using (var stream = new System.IO.FileStream(dataObject.Font.Master.LoadedFilePath, System.IO.FileMode.Open))
            {
                Texture2D font = Texture2D.FromStream(Global.GraphicsDevice, stream);
                RenderTarget2D output = new RenderTarget2D(Global.GraphicsDevice, font.Width, font.Height);
                Global.GraphicsDevice.SetRenderTarget(output);

                using (var batch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(SadConsole.Global.GraphicsDevice))
                {
                    batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    batch.Draw(font, Microsoft.Xna.Framework.Vector2.Zero, ForegroundColor);
                    batch.End();
                }

                Global.GraphicsDevice.SetRenderTarget(Global.OriginalRenderTarget);
                font.Dispose();

                using (var outputStream = new System.IO.MemoryStream())
                {
                    output.SaveAsPng(outputStream, output.Width, output.Height);
                    recoloredImage = new Bitmap(outputStream);
                    picFontSheet.Image = recoloredImage;
                }

            }
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
