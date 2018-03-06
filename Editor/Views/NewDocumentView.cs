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
using MugenMvvmToolkit.Binding.Builders;
using MugenMvvmToolkit.Binding;

namespace SadConsole.Editor.Views
{
    public partial class NewDocumentView : Form
    {
        //private string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };
        //private Font documentFont;

        //public int DocumentWidth { get => (int)numWidth.Value; set => numWidth.Value = value; }

        //public int DocumentHeight { get => (int)numHeight.Value; set => numHeight.Value = value; }

        //public Font DocumentFont
        //{
        //    get => documentFont;
        //    set
        //    {
        //        documentFont = value;

        //        txtFont.Text = documentFont.Name;
        //        txtFontSize.Text = FontSizeNames[(int)documentFont.SizeMultiple];
        //    }
        //}

        //public Color DocumentBackground { get => clrBackground.Color; set => clrBackground.Color = value; }

        public Color DocumentForeground { get => clrForeground.Color; set => clrForeground.Color = value; }


        public NewDocumentView()
        {
            InitializeComponent();

            using (var set = new BindingSet<ViewModels.NewDocumentViewModel>())
            {
                //set.Bind(label, () => l => l.Text).To(() => (vm, ctx) => vm.Text);
                set.Bind(numWidth, () => num => num.Value).To(() => (vm, ctx) => vm.DocumentWidth).TwoWay();
                set.Bind(numHeight, () => num => num.Value).To(() => (vm, ctx) => vm.DocumentHeight).TwoWay();
                set.Bind(clrBackground, () => clr => clr.Color).To(() => (vm, ctx) => vm.DocumentBackground).TwoWay();
                set.Bind(this, () => clr => clr.DocumentForeground).To(() => (vm, ctx) => vm.DocumentForeground).TwoWay();
            }
        }

        private void NewDocument_Load(object sender, EventArgs e)
        {
            //DocumentFont = SadConsole.Global.FontDefault;
            //DocumentBackground = Color.Transparent;
            //DocumentForeground = Color.Black;
        }

        private void btnChangeFont_Click(object sender, EventArgs e)
        {
            //using (var form = new Forms.ChangeFont(documentFont))
            //{
            //    if (form.ShowDialog(this) == DialogResult.OK)
            //        DocumentFont = form.SelectedFont;
            //}
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
                //var editor = new SurfaceEditor(DocumentWidth, DocumentHeight);
                //editor.Font = documentFont;
                //return editor;
            }

            return null;
        }
    }
}
