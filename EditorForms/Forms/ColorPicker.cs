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
    public partial class ColorPicker : Form
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        private void ColorPicker_Load(object sender, EventArgs e)
        {
            cslRainbow.ColorRange = new[] { Color.Red, Color.Yellow, Color.Green, Color.Turquoise, Color.Blue, Color.Purple, Color.Red };
            cslRed.ColorRange = new[] { Color.Black, Color.Red };
            cslGreen.ColorRange = new[] { Color.Black, Color.Green };
            cslBlue.ColorRange = new[] { Color.Black, Color.Blue };
        }
    }
}
