using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Surfaces;
using SadConsole.Controls;
using SadConsole.Input;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Entities;

namespace SadConsoleEditor.Panels
{
    class HotspotToolPanel : CustomPanel
    {
        ListBox<HotspotListBoxItem> hotspotsListbox;
        Button createButton;
        Button editButton;
        Button deleteButton;
        Button exportListButton;
        Button importListButton;
        Button cloneHotspot;

        private CheckBox drawHotspotsCheckbox;

        public Hotspot SelectedObject;

        public bool DrawHotspots
        {
            get { return drawHotspotsCheckbox.IsSelected; }
            set { drawHotspotsCheckbox.IsSelected = value; }
        }

        public HotspotToolPanel()
        {
            Title = "Hotspots";

            hotspotsListbox = new ListBox<HotspotListBoxItem>(Consoles.ToolPane.PanelWidthControls, 7);
            createButton = new Button(Consoles.ToolPane.PanelWidthControls, 1);
            editButton = new Button(Consoles.ToolPane.PanelWidthControls, 1);
            deleteButton = new Button(Consoles.ToolPane.PanelWidthControls, 1);
            exportListButton = new Button(Consoles.ToolPane.PanelWidthControls, 1);
            cloneHotspot = new Button(Consoles.ToolPane.PanelWidthControls, 1);
            importListButton = new Button(Consoles.ToolPane.PanelWidthControls, 1);

            hotspotsListbox.SelectedItemChanged += hotspotsListbox_SelectedItemChanged;
            createButton.Click += _createNewObjectButton_Click;
            editButton.Click += _editObjectButton_Click;
            deleteButton.Click += _deleteObjectButton_Click;
            exportListButton.Click += _exportListButton_Click;
            cloneHotspot.Click += CloneHotspot_Click;
            importListButton.Click += ImportListButton_Click;

            editButton.IsEnabled = false;
            deleteButton.IsEnabled = false;
            cloneHotspot.IsEnabled = false;

            hotspotsListbox.HideBorder = true;

            createButton.Text = "Define New";
            editButton.Text = "Edit";
            deleteButton.Text = "Delete";
            exportListButton.Text = "Export";
            cloneHotspot.Text = "Clone";
            importListButton.Text = "Import";

            drawHotspotsCheckbox = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            drawHotspotsCheckbox.IsSelected = true;
            drawHotspotsCheckbox.Text = "Draw hotspots";

            Controls = new ControlBase[] { createButton, null, hotspotsListbox, null, editButton, cloneHotspot, deleteButton, null, exportListButton, importListButton, null, drawHotspotsCheckbox };

            // Load the known object types.
            //if (System.IO.File.Exists(Settings.FileObjectTypes))
            //{
            //    using (var fileObject = System.IO.File.OpenRead(Settings.FileObjectTypes))
            //    {
            //        var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Entity[]));

            //        var Entitys = serializer.ReadObject(fileObject) as Entity[];

            //        foreach (var item in Entitys)
            //        {
            //            var newItem = new EntityMeta(item, true);
            //            hotspotsListbox.Items.Add(newItem);
            //        }
            //    }
            //}

            exportListButton.IsEnabled = hotspotsListbox.Items.Count != 0;
        }
        
        private void CloneHotspot_Click(object sender, EventArgs e)
        {
            Windows.RenamePopup popup = new Windows.RenamePopup("Name", "Clone hotspot");
            popup.Closed += (o, ev) =>
            {
                if (popup.DialogResult)
                {
                    var hotspot = new Hotspot();
                    hotspot.Title = popup.NewName;
                    SelectedObject.DebugAppearance.CopyAppearanceTo(hotspot.DebugAppearance);
                    hotspot.Settings = new Dictionary<string, string>(SelectedObject.Settings);
                    ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).LoadHotspot(hotspot);
                }
            };
            popup.Show(true);
            popup.Center();
        }

        void _exportListButton_Click(object sender, EventArgs e)
        {
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            if (editor.Hotspots.Count == 0)
                return;

            Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
            popup.Center();
            popup.Closed += (s, e2) => 
            {
                if (popup.DialogResult)
                {
                    List<Hotspot> clonedSpots = new List<Hotspot>(editor.Hotspots.Count);

                    foreach (var spot in editor.Hotspots)
                    {
                        Hotspot newSpot = new Hotspot();
                        newSpot.Title = spot.Title;
                        spot.DebugAppearance.CopyAppearanceTo(newSpot.DebugAppearance);
                        newSpot.Settings = new Dictionary<string, string>(spot.Settings);
                        clonedSpots.Add(newSpot);
                    }

                    popup.SelectedLoader.Save(clonedSpots, popup.SelectedFile);
                }
            };
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Hotspots() };
            popup.SkipFileExistCheck = true;
            popup.SelectButtonText = "Save";
            popup.Show(true);
        }

        private void ImportListButton_Click(object sender, EventArgs e)
        {
            Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
            popup.Center();
            popup.Closed += (s, e2) =>
            {
                if (popup.DialogResult)
                {
                    var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;
                    Dictionary<string, Hotspot> titleKeys = new Dictionary<string, Hotspot>();
                    List<Hotspot> loadedSpots = (List<Hotspot>)popup.SelectedLoader.Load(popup.SelectedFile);

                    var titleCount = loadedSpots.Select(h => h.Title).Intersect(editor.Hotspots.Select(h => h.Title)).Count();

                    if (titleCount != 0)
                    {
                        titleKeys = editor.Hotspots.ToDictionary((h) => h.Title, (h) => h);
                        Window.Prompt(new ColoredString($"{titleCount} will be overwritten, continue?"), "Yes", "No", (result) =>
                        {
                            if (result)
                                RunImportLogic(loadedSpots, titleKeys);
                        });
                    }
                    else
                        RunImportLogic(loadedSpots, titleKeys);

                }
            };
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Hotspots() };
            popup.Show(true);
        }

        void RunImportLogic(List<Hotspot> importedSpots, Dictionary<string, Hotspot> titleKeys)
        {
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            foreach (var spot in importedSpots)
            {
                if (titleKeys.ContainsKey(spot.Title))
                {
                    var oldSpot = titleKeys[spot.Title];
                    spot.DebugAppearance.CopyAppearanceTo(oldSpot.DebugAppearance);
                    spot.Settings = oldSpot.Settings;
                }
                else
                    editor.Hotspots.Add(spot);
            }

            RebuildListBox();
        }

        void _deleteObjectButton_Click(object sender, EventArgs e)
        {
            Window.Prompt(new ColoredString("Are you sure? This will delete all hotspots of this type from your scene."), "Yes", "No", (r) =>
            {
                if (r)
                {
                    ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).Hotspots.Remove((Hotspot)hotspotsListbox.SelectedItem);
                    RebuildListBox();
                }
            });
        }

        void _editObjectButton_Click(object sender, EventArgs e)
        {
            Windows.EditHotspotPopup popup = new Windows.EditHotspotPopup((Hotspot)hotspotsListbox.SelectedItem);
            popup.Closed += (o, e2) =>
                {
                    if (popup.DialogResult)
                    {
                        var hotSpot = (Hotspot)hotspotsListbox.SelectedItem;
                        hotSpot.Title = popup.CreatedHotspot.Title;
                        popup.CreatedHotspot.DebugAppearance.CopyAppearanceTo(hotSpot.DebugAppearance);
                        hotSpot.Settings = popup.CreatedHotspot.Settings;

                        hotspotsListbox.GetContainer(hotspotsListbox.SelectedItem).IsDirty = true;
                        //MainScreen.Instance.ToolPane.SelectedTool.RefreshTool();
                    }
                };
            popup.Show(true);
        }

        void _createNewObjectButton_Click(object sender, EventArgs e)
        {
            Hotspot hotSpot = new Hotspot();
            Windows.EditHotspotPopup popup = new Windows.EditHotspotPopup(hotSpot);
            popup.Closed += (o, e2) =>
                {
                    if (popup.DialogResult)
                    {
                        hotSpot = popup.CreatedHotspot;
                        hotspotsListbox.Items.Add(hotSpot);
                        hotspotsListbox.SelectedItem = hotSpot;
                        exportListButton.IsEnabled = true;
                        ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).Hotspots.Add(hotSpot);
                    }
                };

            popup.Show(true);
        }

        void hotspotsListbox_SelectedItemChanged(object sender, ListBox<HotspotListBoxItem>.SelectedItemEventArgs e)
        {
            if (hotspotsListbox.SelectedItem == null)
                SelectedObject = null;
            else
                SelectedObject = (Hotspot)hotspotsListbox.SelectedItem;
            
            editButton.IsEnabled = SelectedObject != null;
            deleteButton.IsEnabled = SelectedObject != null;
            cloneHotspot.IsEnabled = SelectedObject != null;
            //MainScreen.Instance.Instance.ToolPane.SelectedTool.RefreshTool();
        }

        public void RebuildListBox()
        {
            hotspotsListbox.Items.Clear();

            if (MainScreen.Instance.ActiveEditor is Editors.SceneEditor)
            {
                var spots = ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).Hotspots;

                if (spots.Count != 0)
                {
                    foreach (var item in spots)
                        hotspotsListbox.Items.Add(item);

                    hotspotsListbox.SelectedItem = hotspotsListbox.Items[0];
                }
            }

            exportListButton.IsEnabled = hotspotsListbox.Items.Count != 0;
        }

        public override void ProcessMouse(MouseConsoleState info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            return 0;
        }
        public override void Loaded()
        {
            var previouslySelected = hotspotsListbox.SelectedItem;
            RebuildListBox();
            if (previouslySelected != null)
                hotspotsListbox.SelectedItem = previouslySelected;
        }

        

        private class HotspotListBoxItem : ListBoxItem
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="surface"></param>
            /// <param name="area"></param>
            public override void Draw(SurfaceBase surface, Microsoft.Xna.Framework.Rectangle area)
            {
                var hotSpot = ((Hotspot)Item);
                ColoredString value = ((char)hotSpot.DebugAppearance.Glyph).ToString().CreateColored(hotSpot.DebugAppearance.Foreground, hotSpot.DebugAppearance.Background, hotSpot.DebugAppearance.Mirror) + " ".CreateColored(_currentAppearance.Foreground, _currentAppearance.Background) + hotSpot.Title.CreateColored(_currentAppearance.Foreground, _currentAppearance.Background);

                if (value.Count < area.Width)
                    value += new string(' ', area.Width - value.Count).CreateColored(_currentAppearance.Foreground, _currentAppearance.Background);
                else if (value.Count > area.Width)
                    value = new ColoredString(value.Take(area.Width).ToArray());
                var editor = new SurfaceEditor(surface);
                editor.Print(area.X, area.Y, value);
                _isDirty = false;
            }
        }
    }

}
