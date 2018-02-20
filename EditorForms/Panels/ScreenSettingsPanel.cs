using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadConsole.Editor.Panels
{
    public partial class ScreenSettingsPanel : UserControl
    {
        private string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };

        public bool IsError { set { pnlError.Visible = value; } }

        public ScreenSettingsPanel()
        {
            InitializeComponent();
        }

        private void btnChangeFont_Click(object sender, EventArgs e)
        {
            using (var form = new Forms.ChangeFont(DataContext.Instance.SelectedFont))
            {
                DataContext.Instance.PauseEditMode = true;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    var console = DataContext.Instance.Screen as SadConsole.Console;
                    DataContext.Instance.SelectedFont = form.SelectedFont;
                    console.TextSurface.Font = form.SelectedFont;
                    RefreshScreen();
                }

                DataContext.Instance.PauseEditMode = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }

        private void picForeground_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                DataContext.Instance.PauseEditMode = true;
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    var console = DataContext.Instance.Screen as SadConsole.Console;
                    picForeground.BackColor = console.TextSurface.DefaultForeground.ToDrawingColor();
                    RefreshScreen();
                }
                DataContext.Instance.PauseEditMode = false;
            }
        }

        private void picBackground_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                DataContext.Instance.PauseEditMode = true;
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    var console = DataContext.Instance.Screen as SadConsole.Console;
                    console.TextSurface.DefaultBackground = colorDialog.Color.ToMonoGameColor();
                    RefreshScreen();
                }
                DataContext.Instance.PauseEditMode = false;
            }
        }

        public void RefreshScreen()
        {
            var console = DataContext.Instance.Screen as SadConsole.Console;

            if (console != null)
            {
                IsError = false;
                txtFont.Text = console.TextSurface.Font.Name;
                txtFontSize.Text = FontSizeNames[(int)console.TextSurface.Font.SizeMultiple];
                txtWidth.Text = console.Width.ToString();
                txtHeight.Text = console.Height.ToString();
                picBackground.BackColor = console.TextSurface.DefaultBackground.ToDrawingColor();
                picForeground.BackColor = console.TextSurface.DefaultForeground.ToDrawingColor();
            }
            else
                IsError = true;
        }
    }
}
