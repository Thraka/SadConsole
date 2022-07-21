using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Myra.Assets;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
//using ReactiveUI;

namespace SadConsole.MyraUI
{
    public class DebuggerPage: Page//<ScreenObjectViewModel>
    {
        public MenuItem mnuClose;
        public Tree treeScreenObjects;
        public Label lblSelectedObjectTitle;
        public SpinButton txtSelectedObjectPosX;
        public SpinButton txtSelectedObjectPosY;
        public CheckBox chkSelectedObjectIsVisible;
        public CheckBox chkSelectedObjectIsEnabled;
        public Image imgOutput;

        public DebuggerPage(): base("Debugger.xmmp", InternalAssetManager)
        {
        }

        protected override void OnPageLoaded(Stylesheet stylesheet)
        {
            // Load all of the controls
            mnuClose = ((HorizontalMenu)Root.FindWidgetById("mainMenu")).FindMenuItemById("mnuClose");
            treeScreenObjects = (Tree)Root.FindWidgetById("ScreenObjectTree");
            lblSelectedObjectTitle = (Label)Root.FindWidgetById("lblSelectedObjectTitle");
            txtSelectedObjectPosX = (SpinButton)Root.FindWidgetById("txtSelectedObjectPosX");
            txtSelectedObjectPosY = (SpinButton)Root.FindWidgetById("txtSelectedObjectPosY");
            chkSelectedObjectIsVisible = (CheckBox)Root.FindWidgetById("chkSelectedObjectIsVisible");
            chkSelectedObjectIsEnabled = (CheckBox)Root.FindWidgetById("chkSelectedObjectIsEnabled");
            imgOutput = (Image)Root.FindWidgetById("Output");

            // Configure global render output
            imgOutput.Renderable = new Myra.Graphics2D.TextureAtlases.TextureRegion(SadConsole.Host.Global.RenderOutput);

            // Load the tree
            treeScreenObjects.HasRoot = false;
            treeScreenObjects.SelectionChanged += Tree_SelectionChanged;

            mnuClose.Selected += MnuClose_Selected;
            chkSelectedObjectIsEnabled.Click += ChkSelectedObjectIsEnabled_Click;
            chkSelectedObjectIsVisible.Click += ChkSelectedObjectIsVisible_Click;
            txtSelectedObjectPosX.TextBox.ValueChanging += txtSelectedObjectPosX_ValueChanging;
            txtSelectedObjectPosY.TextBox.ValueChanging += txtSelectedObjectPosY_ValueChanging;
            
            AddConsoleToList(SadConsole.Game.Instance.Screen, treeScreenObjects);

            // Load the screen objects into the tree
            void AddConsoleToList(IScreenObject screenObject, TreeNode parentNode)
            {
                var node = parentNode.AddSubNode(screenObject.GetDebuggerDisplayValue());
                node.Tag = screenObject;
                node.ApplyTreeNodeStyle(parentNode.TreeStyle);

                foreach (var child in screenObject.Children)
                    AddConsoleToList(child, node);
            }
            treeScreenObjects.AllNodes[0].IsExpanded = true;
            treeScreenObjects.SelectedRow = treeScreenObjects.AllNodes[0];

            //this.Bind(ViewModel, vm => vm.IsVisible, v => v.chkSelectedObjectIsVisible.IsChecked);
            
        }
        
        private void MnuClose_Selected(object sender, EventArgs e)
        {
            SadConsole.Debug.MonoGame.Debugger.Stop();
        }

        private void txtSelectedObjectPosY_ValueChanging(object sender, Myra.Utility.ValueChangingEventArgs<string> e)
        {
            if (treeScreenObjects.SelectedRow.Tag is IScreenSurface screenObj)
                if (int.TryParse(e.NewValue, out int y))
                    screenObj.Position = screenObj.Position.WithY(y);
        }

        private void txtSelectedObjectPosX_ValueChanging(object sender, Myra.Utility.ValueChangingEventArgs<string> e)
        {
            if (treeScreenObjects.SelectedRow.Tag is IScreenObject screenObj)
                if (int.TryParse(e.NewValue, out int x))
                    screenObj.Position = screenObj.Position.WithX(x);
        }

        private void ChkSelectedObjectIsVisible_Click(object sender, EventArgs e)
        {
            if (treeScreenObjects.SelectedRow.Tag is IScreenObject screenObj)
                screenObj.IsVisible = chkSelectedObjectIsVisible.IsChecked;
        }

        private void ChkSelectedObjectIsEnabled_Click(object sender, EventArgs e)
        {
            if (treeScreenObjects.SelectedRow.Tag is IScreenObject screenObj)
                screenObj.IsEnabled = chkSelectedObjectIsEnabled.IsChecked;
        }

        private void Tree_SelectionChanged(object sender, EventArgs e)
        {
            var screenObject = (IScreenObject)(((Tree)sender).SelectedRow.Tag);

            //ViewModel = new ScreenObjectViewModel(screenObject);

            lblSelectedObjectTitle.Text = screenObject.GetDebuggerDisplayValue();
            chkSelectedObjectIsEnabled.IsChecked = screenObject.IsEnabled;
            chkSelectedObjectIsVisible.IsChecked = screenObject.IsVisible;

            txtSelectedObjectPosX.Value = screenObject.Position.X;
            txtSelectedObjectPosY.Value = screenObject.Position.Y;

            if (screenObject is IScreenSurface surface)
            {

            }
            else
            {
            }
        }

        private void RefreshObjectUI()
        {

        }
    }
}
