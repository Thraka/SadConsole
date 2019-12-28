using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Controls;
using SadConsoleEditor.Windows;
using SadConsole;
using SadConsole.Surfaces;

namespace SadConsoleEditor.Panels
{
    class LayersPanel : CustomPanel
    {
        private ListBox<LayerListBoxItem> layers;
        private Button removeSelected;
        private Button moveSelectedUp;
        private Button moveSelectedDown;
        private Button addNewLayer;
        private Button renameLayer;
        private Button addNewLayerFromFile;
        private Button saveLayerToFile;
        private CheckBox toggleHideShow;

        private LayeredSurface surface;

        public LayersPanel()
        {
            Title = "Layers";
            layers = new ListBox<LayerListBoxItem>(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 4);
            layers.HideBorder = true;
            layers.SelectedItemChanged += layers_SelectedItemChanged;
            layers.CompareByReference = true;

            removeSelected = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            removeSelected.Text = "Remove";
            removeSelected.Click += removeSelected_Click;

            moveSelectedUp = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            moveSelectedUp.Text = "Move Up";
            moveSelectedUp.Click += moveSelectedUp_Click;

            moveSelectedDown = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            moveSelectedDown.Text = "Move Down";
            moveSelectedDown.Click += moveSelectedDown_Click;

            toggleHideShow = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            toggleHideShow.Text = "Show/Hide";
            toggleHideShow.TextAlignment = System.Windows.HorizontalAlignment.Center;
            toggleHideShow.IsSelectedChanged += toggleHideShow_IsSelectedChanged;

            addNewLayer = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            addNewLayer.Text = "Add New";
            addNewLayer.Click += addNewLayer_Click;

            renameLayer = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            renameLayer.Text = "Rename";
            renameLayer.Click += renameLayer_Click;

            addNewLayerFromFile = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            addNewLayerFromFile.Text = "Load From File";
            addNewLayerFromFile.Click += addNewLayerFromFile_Click;

            saveLayerToFile = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            saveLayerToFile.Text = "Save Layer to File";
            saveLayerToFile.Click += saveLayerToFile_Click;

            Controls = new ControlBase[] { layers, toggleHideShow, removeSelected, moveSelectedUp, moveSelectedDown, addNewLayer, renameLayer, addNewLayerFromFile, saveLayerToFile };
        }

        public void SetLayeredSurface(LayeredSurface surface)
        {
            this.surface = surface;

            // Do updates
            RebuildListBox();
        }

        void saveLayerToFile_Click(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;

            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    SadConsole.Surfaces.Basic newSurface = new SadConsole.Surfaces.Basic(surface.Width, surface.Height, layer.Cells, SadConsoleEditor.Settings.Config.ScreenFont, new Microsoft.Xna.Framework.Rectangle(0,0, surface.Width, surface.Height));
                    newSurface.Save(popup.SelectedFile);
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.SadConsole.Surfaces.Basic() };
            popup.SelectButtonText = "Save";
            popup.SkipFileExistCheck = true;
            popup.Show(true);
            popup.Center();
        }

        void addNewLayerFromFile_Click(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    SadConsole.Surfaces.Basic newSurface = SadConsole.Surfaces.Basic.Load(popup.SelectedFile);

                    if (newSurface.Width != surface.Width || newSurface.Height != surface.Height)
                    {
                        var tempSurface = new SadConsole.Surfaces.Basic(surface.Width, surface.Height, surface.Font);
                        newSurface.Copy(tempSurface);
                        var newLayer = surface.Add(tempSurface);
                        LayerMetadata.Create("Loaded", true, true, true, newLayer);
                    }
                    else
                    {
                        var layer = surface.Add(newSurface);
                        LayerMetadata.Create("Loaded", true, true, true, layer);
                    }

                    surface.IsDirty = true;
                    RebuildListBox();
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.SadConsole.Surfaces.Basic() };
            popup.Show(true);
            popup.Center();
        }

        void renameLayer_Click(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;
            var meta = (LayerMetadata)layer.Metadata;
            RenamePopup popup = new RenamePopup(meta.Name);
            popup.Closed += (o, e2) => { if (popup.DialogResult) meta.Name = popup.NewName; layers.IsDirty = true; };
            popup.Show(true);
            popup.Center();
        }

        void moveSelectedDown_Click(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;
            surface.Move(layer, layer.Index - 1);
            RebuildListBox();
            layers.SelectedItem = layer;
            surface.IsDirty = true;
        }

        void moveSelectedUp_Click(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;
            surface.Move(layer, layer.Index + 1);
            RebuildListBox();
            layers.SelectedItem = layer;
            surface.IsDirty = true;
        }

        void removeSelected_Click(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;
            surface.Remove(layer);
            RebuildListBox();
            layers.SelectedItem = layers.Items[0];
            surface.IsDirty = true;
        }

        void addNewLayer_Click(object sender, EventArgs e)
        {
            var previouslySelected = layers.SelectedItem;
            LayerMetadata.Create("new", true, true, true, surface.Add());
            RebuildListBox();
            layers.SelectedItem = previouslySelected;
            surface.IsDirty = true;
        }

        void layers_SelectedItemChanged(object sender, ListBox<LayerListBoxItem>.SelectedItemEventArgs e)
        {
            removeSelected.IsEnabled = layers.Items.Count != 1;

            moveSelectedUp.IsEnabled = true;
            moveSelectedDown.IsEnabled = true;
            renameLayer.IsEnabled = true;

            if (layers.SelectedItem != null)
            {
                var layer = (LayeredSurface.Layer)layers.SelectedItem;
                var meta = (LayerMetadata)layer.Metadata;

                moveSelectedUp.IsEnabled = meta.IsMoveable && layers.Items.Count != 1 && layer.Index != layers.Items.Count - 1;
                moveSelectedDown.IsEnabled = meta.IsMoveable && layers.Items.Count != 1 && layer.Index != 0;
                removeSelected.IsEnabled = meta.IsRemoveable && layers.Items.Count != 1;
                renameLayer.IsEnabled = meta.IsRenamable;

                toggleHideShow.IsSelected = layer.IsVisible;

                surface.SetActiveLayer(layer.Index);
                MainScreen.Instance.LayerName = meta.Name;
                surface.IsDirty = true;

            }
            else
                MainScreen.Instance.LayerName = "None";
        }

        void toggleHideShow_IsSelectedChanged(object sender, EventArgs e)
        {
            var layer = (LayeredSurface.Layer)layers.SelectedItem;
            layer.IsVisible = toggleHideShow.IsSelected;
            surface.IsDirty = true;
        }

        public void RebuildListBox()
        {
            layers.SelectedItem = null;
            layers.Items.Clear();

            for (int i = surface.LayerCount - 1; i >= 0; i--)
                layers.Items.Add(surface.GetLayer(i));

            layers.SelectedItem = layers.Items[0];
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            return control == layers || control == toggleHideShow ? 1 : 0;
        }

        public override void Loaded()
        {
            var previouslySelected = layers.SelectedItem;
            RebuildListBox();
            if (previouslySelected == null || !layers.Items.Contains(previouslySelected))
                layers.SelectedItem = layers.Items[0];
            else
                layers.SelectedItem = previouslySelected;
        }

        private class LayerListBoxItem : ListBoxItem
        {
            public override void Draw(SurfaceBase surface, Microsoft.Xna.Framework.Rectangle area)
            {
                string value = ((LayerMetadata)((LayeredSurface.Layer)Item).Metadata).Name;

                if (value.Length < area.Width)
                    value += new string(' ', area.Width - value.Length);
                else if (value.Length > area.Width)
                    value = value.Substring(0, area.Width);
                var editor = new SurfaceEditor(surface);
                editor.Print(area.X, area.Y, value, _currentAppearance);
                _isDirty = false;
            }
        }
    }
}
