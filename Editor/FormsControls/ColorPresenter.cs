using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.Windows.Forms.Design;
using MugenMvvmToolkit.Binding.Builders;

namespace SadConsole.Editor.FormsControls
{
    [Designer(typeof(FixedHeightUserControlDesigner))]
    public partial class ColorPresenter : UserControl
    {
        private Microsoft.Xna.Framework.Color color;
        private const int FIXED_HEIGHT = 26;

        public event EventHandler ColorChanged;

        /// <summary>
        /// The caption displayed with the color box.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always),
        Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text { get => lblCaption.Text; set => lblCaption.Text = value; }

        /// <summary>
        /// The selected color.
        /// </summary>
        
        public Microsoft.Xna.Framework.Color Color
        {
            get => color;
            set
            {
                color = value;
                picColor.BackColor = value.ToDrawingColor();
                ColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private void picColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.SolidColorOnly = false;
                colorDialog.AnyColor = true;
                colorDialog.Color = picColor.BackColor;

                if (colorDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
                    Color = colorDialog.Color.ToMonoGameColor();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Size.Height != FIXED_HEIGHT)
                this.Size = new Size(this.Size.Width, FIXED_HEIGHT);

            base.OnSizeChanged(e);
        }

        public ColorPresenter()
        {
            InitializeComponent();
        }
    }

    public class FixedHeightUserControlDesigner : ControlDesigner
    {
        private static string[] _propsToRemove = new string[] { "Height", "Size" };

        public override SelectionRules SelectionRules
        {
            get { return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable; }
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);
            foreach (string p in _propsToRemove)
                if (properties.Contains(p))
                    properties.Remove(p);
        }
    }
}
