using SadConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditorForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Add(CreateNode(SadConsole.Global.CurrentScreen));
            treeView1.ExpandAll();
        }

        private TreeNode CreateNode(IScreen screen)
        {
            TreeNode node = new TreeNode(screen.GetType().ToString());

            foreach (var child in screen.Children)
            {
                node.Nodes.Add(CreateNode(child));
            }
            
            return node;
        }
    }

    public static class Editor
    {
        private static Form1 form;

        public static void ShowEditor()
        {
            if (form == null)
                form = new Form1();

            form.Show();
            
        }
    }
}
