using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadConsole.Editor.Forms
{
    public partial class EditorSettings : Form
    {
        private bool Loaded = false;

        public EditorSettings()
        {
            InitializeComponent();
        }

        private void picInnerColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    picInnerColor.BackColor = colorDialog.Color;
                    Config.Instance.EditClearColorInner = colorDialog.Color.ToMonoGameColor();
                }
            }
        }

        private void picOuterColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.SolidColorOnly = true;
                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    picOuterColor.BackColor = colorDialog.Color;
                    Config.Instance.EditClearColorOuter = colorDialog.Color.ToMonoGameColor();
                }
            }
        }


        private void CheckChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                Config.Instance.DrawEditClearColorInner = chkUseInner.Checked;
                Config.Instance.DrawEditClearColorOuter = chkUseOuter.Checked;
                Config.Instance.DrawOuterBorder = chkDrawBorder.Checked;

                picInnerColor.Enabled = lblInnerColor.Enabled = chkUseInner.Checked;
                picOuterColor.Enabled = lblOuterColor.Enabled = chkUseOuter.Checked;

                Invalidate();
            }
        }

        private void EditorSettings_Load(object sender, EventArgs e)
        {
            chkUseInner.Enabled = true;
            chkUseOuter.Enabled = true;
            picInnerColor.Enabled = true;
            picOuterColor.Enabled = true;

            chkUseInner.Checked = Config.Instance.DrawEditClearColorInner;
            picInnerColor.BackColor = Config.Instance.EditClearColorInner.ToDrawingColor();

            chkUseOuter.Checked = Config.Instance.DrawEditClearColorOuter;
            picOuterColor.BackColor = Config.Instance.EditClearColorOuter.ToDrawingColor();
            chkDrawBorder.Checked = Config.Instance.DrawOuterBorder;

            Loaded = true;
        }
    }
}
