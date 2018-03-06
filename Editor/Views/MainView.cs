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
using MugenMvvmToolkit.Binding.Builders;
using MugenMvvmToolkit.Binding;

namespace SadConsole.Editor.Views
{
    internal partial class MainView : Form
    {
        int panelSplitterPosition = 0;
        Control selectedToolPanel;

        public MainView()
        {
            InitializeComponent();

            using (var set = new BindingSet<ViewModels.MainViewModel>())
            {
                //set.Bind(label, () => l => l.Text).To(() => (vm, ctx) => vm.Text);

                set.Bind(newToolStripMenuItem).To(() => (vm, ctx) => vm.NewDocumentCommand);

            }
        }
        
        private void UpdateTree()
        {
        //    if (treeView1.SelectedNode != null)
        //    {
        //        var previousSelectedItem = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
        //        treeView1.Nodes.Clear();
        //        treeView1.Nodes.Add(new TreeViewNodeScreen(SadConsole.Global.CurrentScreen));
        //        var newNodeSelected = FindScreenInNodes(previousSelectedItem, treeView1.Nodes);

        //        if (newNodeSelected == null)
        //            treeView1.SelectedNode = treeView1.Nodes[0];
        //        else
        //            treeView1.SelectedNode = newNodeSelected;
        //    }
        //    else
        //    {
        //        treeView1.Nodes.Clear();
        //        treeView1.Nodes.Add(new TreeViewNodeScreen(SadConsole.Global.CurrentScreen));
        //        treeView1.SelectedNode = treeView1.Nodes[0];
        //    }
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
            //if (treeView1.SelectedNode == null)
            //{
            //    pnlScreenSettings.IsError = true;
            //}
            //else
            //{
            //    DataContext.Instance.Screen = ((TreeViewNodeScreen)treeView1.SelectedNode).Screen;
            //    pnlScreenSettings.IsError = DataContext.Instance.IsScreenConsole;
            //    pnlScreenSettings.RefreshScreen();
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //editorBindingSource.DataSource = DataContext.Instance;
            //DataContext.Instance.SelectedTool = DataContext.Instance.Tools[0];
            //panelSplitterPosition = splitContainer1.Panel2MinSize;
            //splitContainer1.Panel2Collapsed = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateTree();
        }

        

        private void editorBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            //if (DataContext.Instance.SelectedToolPanel != null && selectedToolPanel != DataContext.Instance.SelectedToolPanel)
            //{
            //    if (selectedToolPanel != null)
            //        selectedToolPanel.Parent.Controls.Remove(selectedToolPanel);

            //    selectedToolPanel = DataContext.Instance.SelectedToolPanel;

            //    //Controls.Add(selectedToolPanel);
            //    pnlToolsList.Parent.Controls.Add(selectedToolPanel);
            //    selectedToolPanel.Location = new Point(pnlToolsList.Left, pnlToolsList.Bottom + pnlToolsList.Margin.Bottom);
            //    selectedToolPanel.Width = pnlToolsList.Width;
            //    //selectedToolPanel.Anchor = pnlToolsList.Anchor;
            //}
        }

        private void cboToolsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cboToolsList.DataBindings[0].WriteValue();
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            //using (var form = new Forms.EditorSettings())
            //{
            //    form.ShowDialog(this);
            //}
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            //DataContext.Instance.PauseEditMode = true;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //DataContext.Instance.PauseEditMode = false;
        }

        private void lstDocuments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
