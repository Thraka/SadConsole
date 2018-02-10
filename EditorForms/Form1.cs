using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadConsole.Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SadConsole.Settings.DoUpdate = !SadConsole.Settings.DoUpdate;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            
        }
    }
}
