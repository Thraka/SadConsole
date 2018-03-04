using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = Microsoft.Xna.Framework.Color;

namespace SadConsole.Editor
{
    public partial class NewDocument : Form
    {
        private string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };
        private Font documentFont;

        public int DocumentWidth { get => (int)numWidth.Value; set => numWidth.Value = value; }

        public int DocumentHeight { get => (int)numHeight.Value; set => numHeight.Value = value; }

        public Font DocumentFont
        {
            get => documentFont;
            set
            {
                documentFont = value;

                txtFont.Text = documentFont.Name;
                txtFontSize.Text = FontSizeNames[(int)documentFont.SizeMultiple];
            }
        }

        public Color DocumentBackground { get => clrBackground.Color; set => clrBackground.Color = value; }

        public Color DocumentForeground { get => clrForeground.Color; set => clrForeground.Color = value; }


        public NewDocument()
        {
            InitializeComponent();
        }

        private void NewDocument_Load(object sender, EventArgs e)
        {
            DocumentFont = SadConsole.Global.FontDefault;
            DocumentBackground = Color.Transparent;
            DocumentForeground = Color.Black;
        }

        private void btnChangeFont_Click(object sender, EventArgs e)
        {
            using (var form = new Forms.ChangeFont(documentFont))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    DocumentFont = form.SelectedFont;
            }
        }

        public IEditor GetEditor()
        {
            if (optGameScene.Checked)
            {
                
            }
            else if (optObject.Checked)
            {

            }
            else
            {
                var editor = new SurfaceEditor(DocumentWidth, DocumentHeight);
                editor.Font = documentFont;
                return editor;
            }

            return null;
        }
    }
}
