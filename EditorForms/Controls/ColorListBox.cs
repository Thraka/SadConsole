using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadConsole.Editor.Controls
{
    public partial class ColorListBox : ListBox
    {
        public ColorListBox()
        {
            InitializeComponent();
            DrawMode = DrawMode.OwnerDrawFixed;
        }
        
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1 || Items.Count == 0)
            {
                base.OnDrawItem(e);
                return;
            }
            Color item = (Color)Items[e.Index];
            var foreColor = Color.FromArgb(255, item.R > 127 ? 0 : 255, item.G > 127 ? 0 : 255, item.B > 127 ? 0 : 255);

            e.DrawBackground();
            var stringSize = e.Graphics.MeasureString(item.Name, Font);
            var stringBounds = e.Bounds;
            stringBounds.Inflate(-10, 0);
            e.Graphics.FillRectangle(new SolidBrush(item), stringBounds);
            e.Graphics.DrawString(item.Name, Font, new SolidBrush(foreColor), stringBounds);

            base.OnDrawItem(e);
        }
    }
}
