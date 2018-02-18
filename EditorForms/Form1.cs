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
        int panelSplitterPosition = 0;
        Control selectedToolPanel;

        public Form1()
        {
            InitializeComponent();
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
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {

            }
            else
            {
                DataContext.Instance.Screen = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            editorBindingSource.DataSource = DataContext.Instance;
            panelSplitterPosition = splitContainer1.Panel2MinSize;
            splitContainer1.Panel2Collapsed = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateTree();
        }

        

        private void chkEditMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEditMode.Checked)
            {
                UpdateTree();
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.SplitterDistance = splitContainer1.Width - panelSplitterPosition;
                DataContext.Instance.EnableEditMode();
                DataContext.Instance.Screen = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
            }
            else
            {
                panelSplitterPosition = splitContainer1.Width - splitContainer1.SplitterDistance;
                SadConsole.Settings.DoUpdate = true;
                splitContainer1.Panel2Collapsed = true;
                DataContext.Instance.DisableEditMode();
            }
        }

        private void editorBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (DataContext.Instance.SelectedToolPanel != null && selectedToolPanel != DataContext.Instance.SelectedToolPanel)
            {
                if (selectedToolPanel != null)
                    selectedToolPanel.Parent.Controls.Remove(selectedToolPanel);

                selectedToolPanel = DataContext.Instance.SelectedToolPanel;

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

        private class TreeViewNodeScreen : TreeNode
        {
            public IScreen Screen;
            public bool Hide;

            public TreeViewNodeScreen(IScreen screen)
            {
                var debugger = screen.GetType().GetTypeInfo().GetCustomAttributes<System.Diagnostics.DebuggerDisplayAttribute>().FirstOrDefault();
                Hide = debugger.Value.Contains("(hide)");

                if (debugger != null)
                    Text = debugger.Value;
                else
                    Text = screen.ToString();

                Screen = screen;

                foreach (var child in screen.Children)
                {
                    var node = new TreeViewNodeScreen(child);

                    if (!node.Hide)
                        Nodes.Add(node);
                }

                Expand();
            }
        }
        
    }
}
