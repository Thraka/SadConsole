using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SadConsole.Editor
{
    internal partial class Form1 : Form
    {
        public static DataContext context = new DataContext();
        int panelSplitterPosition = 0;
        Control selectedToolPanel;

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

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void editorBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void editorBindingSource_DataSourceChanged(object sender, EventArgs e)
        {
        }

        private void UpdateTree()
        {
            if (treeView1.SelectedNode != null)
            {
                var previousSelectedItem = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(new TreeViewNodeScreen(SadConsole.Global.CurrentScreen));
                var newNodeSelected = FindScreenInNodes(previousSelectedItem, treeView1.Nodes);

                if (newNodeSelected == null)
                    treeView1.SelectedNode = treeView1.Nodes[0];
                else
                    treeView1.SelectedNode = newNodeSelected;
            }
            else
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(new TreeViewNodeScreen(SadConsole.Global.CurrentScreen));
                treeView1.SelectedNode = treeView1.Nodes[0];
            }
        }

        private TreeViewNodeScreen FindScreenInNodes(IScreen screen, TreeNodeCollection nodes)
        {
            foreach (var item in nodes)
            {
                TreeViewNodeScreen castedItem = (TreeViewNodeScreen)item;
                if (castedItem.Screen == screen)
                    return castedItem;
                else
                {
                    var result = FindScreenInNodes(screen, castedItem.Nodes);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private class TreeViewNodeScreen: TreeNode
        {
            public IScreen Screen;

            public TreeViewNodeScreen(IScreen screen)
            {
                var debugger = screen.GetType().GetTypeInfo().GetCustomAttributes<System.Diagnostics.DebuggerDisplayAttribute>().FirstOrDefault();

                if (debugger != null)
                    Text = debugger.Value;
                else
                    Text = screen.ToString();

                Screen = screen;

                foreach (var child in screen.Children)
                {
                    Nodes.Add(new TreeViewNodeScreen(child));
                }

                Expand();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {

            }
            else
            {
                context.Screen = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            editorBindingSource.DataSource = context;
            panelSplitterPosition = splitContainer1.Panel2MinSize;
            splitContainer1.Panel2Collapsed = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            UpdateTree();
        }

        

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SadConsole.Settings.DoUpdate = false;
                UpdateTree();
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.SplitterDistance = splitContainer1.Width - panelSplitterPosition;
            }
            else
            {
                panelSplitterPosition = splitContainer1.Width - splitContainer1.SplitterDistance;
                SadConsole.Settings.DoUpdate = true;
                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void editorBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (context.SelectedToolPanel != null && selectedToolPanel != context.SelectedToolPanel)
            {
                if (selectedToolPanel != null)
                    selectedToolPanel.Parent.Controls.Remove(selectedToolPanel);

                selectedToolPanel = context.SelectedToolPanel;

                //Controls.Add(selectedToolPanel);
                pnlToolsList.Parent.Controls.Add(selectedToolPanel);
                selectedToolPanel.Location = new Point(pnlToolsList.Left, pnlToolsList.Bottom + pnlToolsList.Margin.Bottom);
                selectedToolPanel.Width = pnlToolsList.Width;
                //selectedToolPanel.Anchor = pnlToolsList.Anchor;
            }
        }

        private void cboToolsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboToolsList.DataBindings[0].WriteValue();
        }
    }
}
