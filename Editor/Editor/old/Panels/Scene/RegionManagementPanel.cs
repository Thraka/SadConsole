using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Controls;
using SadConsoleEditor.Windows;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Entities;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace SadConsoleEditor.Panels
{
    class RegionManagementPanel : CustomPanel
    {
        private ListBox<EntityListBoxItem> EntityList;
        private Button removeSelected;
        private Button moveSelectedUp;
        private Button moveSelectedDown;
        private Button renameZoneButton;
        private Button addZoneButton;
        private Button editSettings;
        private Controls.ColorPresenter zoneColorPresenter;
        private CheckBox drawZonesCheckbox;
        private DrawingSurface propertySurface;

        private Dictionary<string, string> previousProperties = new Dictionary<string, string>();

        private bool rebuilding;

        public bool DrawZones
        {
            get { return drawZonesCheckbox.IsSelected; }
            set { drawZonesCheckbox.IsSelected = value; }
        }

        public ResizableObject SelectedEntity
        {
            get { return EntityList.SelectedItem as ResizableObject; }
            set { EntityList.SelectedItem = value; }
        }

        public RegionManagementPanel()
        {
            Title = "Zones";
            EntityList = new ListBox<EntityListBoxItem>(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 4);
            EntityList.HideBorder = true;
            EntityList.SelectedItemChanged += Entity_SelectedItemChanged;
            EntityList.CompareByReference = true;

            removeSelected = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            removeSelected.Text = "Remove";
            removeSelected.Click += RemoveSelected_Click;

            moveSelectedUp = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            moveSelectedUp.Text = "Move Up";
            moveSelectedUp.Click += MoveSelectedUp_Click;

            moveSelectedDown = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            moveSelectedDown.Text = "Move Down";
            moveSelectedDown.Click += MoveSelectedDown_Click;
            
            renameZoneButton = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            renameZoneButton.Text = "Rename";
            renameZoneButton.Click += RenameZone_Click;

            addZoneButton = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            addZoneButton.Text = "Add New";
            addZoneButton.Click += AddZone_Click;

            editSettings = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            editSettings.Text = "Edit Settings";
            editSettings.Click += EditSettings_Click;

            zoneColorPresenter = new SadConsoleEditor.Controls.ColorPresenter("Zone Color", Settings.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            zoneColorPresenter.SelectedColor = Color.Aqua;
            zoneColorPresenter.IsEnabled = false;
            zoneColorPresenter.ColorChanged += ZoneColorPresenter_ColorChanged;

            drawZonesCheckbox = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            drawZonesCheckbox.IsSelected = true;
            drawZonesCheckbox.Text = "Draw zones";

            propertySurface = new DrawingSurface(Consoles.ToolPane.PanelWidthControls, 2);

            Controls = new ControlBase[] { addZoneButton, null, EntityList, removeSelected, moveSelectedUp, moveSelectedDown, renameZoneButton, editSettings, null, drawZonesCheckbox, null, zoneColorPresenter, propertySurface };

            Entity_SelectedItemChanged(null, null);
        }

        private void EditSettings_Click(object sender, EventArgs e)
        {
            var zone = ((ResizableObject<Zone>)EntityList.SelectedItem).Data;
            Windows.KeyValueEditPopup popup = new Windows.KeyValueEditPopup(zone.Settings);
            popup.Closed += (o, e2) =>
            {
                if (popup.DialogResult)
                {
                    //_objectTypesListbox.GetContainer(_objectTypesListbox.SelectedItem).IsDirty = true;
                    zone.Settings = popup.SettingsDictionary;
                    RebuildProperties(zone);
                    MainScreen.Instance.ToolsPane.RedrawPanels();
                }
            };
            popup.Show(true);
        }

        private void ZoneColorPresenter_ColorChanged(object sender, EventArgs e)
        {
            var entity = EntityList.SelectedItem as ResizableObject<Zone>;

            if (entity != null)
            {
                entity.Recolor(zoneColorPresenter.SelectedColor);
                entity.Data.DebugAppearance.Background = zoneColorPresenter.SelectedColor;
            }
        }

        void AddZone_Click(object sender, EventArgs e)
        {
            (MainScreen.Instance.ActiveEditor as Editors.SceneEditor)?.LoadZone(new Zone() {
                                                                                    Area = new Rectangle(1, 1, 10, 10),
                                                                                    DebugAppearance = new Cell(Color.White, Color.White.GetRandomColor(Global.Random), 0),
                                                                                    Title = "Zone" });
        }

        void RenameZone_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject)EntityList.SelectedItem;
            RenamePopup popup = new RenamePopup(entity.Name);
            popup.Closed += (o, e2) => { if (popup.DialogResult) entity.Name = popup.NewName; EntityList.IsDirty = true; };
            popup.Show(true);
            popup.Center();
        }

        void MoveSelectedDown_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject<Zone>)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            int index = editor.Zones.IndexOf(entity);
            editor.Zones.Remove(entity);
            editor.Zones.Insert(index + 1, entity);
            RebuildListBox();
            EntityList.SelectedItem = entity;
        }

        void MoveSelectedUp_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject<Zone>)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            int index = editor.Zones.IndexOf(entity);
            editor.Zones.Remove(entity);
            editor.Zones.Insert(index - 1, entity);
            RebuildListBox();
            EntityList.SelectedItem = entity;
        }

        void RemoveSelected_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject<Zone>)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            editor.Zones.Remove(entity);

            RebuildListBox();

            if (EntityList.Items.Count != 0)
                EntityList.SelectedItem = EntityList.Items[0];
        }

        #region TODO Import/Export
        //void _exportListButton_Click(object sender, EventArgs e)
        //{
        //    var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

        //    if (editor.Hotspots.Count == 0)
        //        return;

        //    Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
        //    popup.Center();
        //    popup.Closed += (s, e2) =>
        //    {
        //        if (popup.DialogResult)
        //        {
        //            List<Hotspot> clonedSpots = new List<Hotspot>(editor.Hotspots.Count);

        //            foreach (var spot in editor.Hotspots)
        //            {
        //                Hotspot newSpot = new Hotspot();
        //                newSpot.Title = spot.Title;
        //                spot.DebugAppearance.CopyAppearanceTo(newSpot.DebugAppearance);
        //                newSpot.Settings = new Dictionary<string, string>(spot.Settings);
        //                clonedSpots.Add(newSpot);
        //            }

        //            popup.SelectedLoader.Save(clonedSpots, popup.SelectedFile);
        //        }
        //    };
        //    popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Hotspots() };
        //    popup.SkipFileExistCheck = true;
        //    popup.SelectButtonText = "Save";
        //    popup.Show(true);
        //}

        //private void ImportListButton_Click(object sender, EventArgs e)
        //{
        //    Windows.SelectFilePopup popup = new Windows.SelectFilePopup();
        //    popup.Center();
        //    popup.Closed += (s, e2) =>
        //    {
        //        if (popup.DialogResult)
        //        {
        //            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;
        //            Dictionary<string, Hotspot> titleKeys = new Dictionary<string, Hotspot>();
        //            List<Hotspot> loadedSpots = (List<Hotspot>)popup.SelectedLoader.Load(popup.SelectedFile);

        //            var titleCount = loadedSpots.Select(h => h.Title).Intersect(editor.Hotspots.Select(h => h.Title)).Count();

        //            if (titleCount != 0)
        //            {
        //                titleKeys = editor.Hotspots.ToDictionary((h) => h.Title, (h) => h);
        //                Window.Prompt(new ColoredString($"{titleCount} will be overwritten, continue?"), "Yes", "No", (result) =>
        //                {
        //                    if (result)
        //                        RunImportLogic(loadedSpots, titleKeys);
        //                });
        //            }
        //            else
        //                RunImportLogic(loadedSpots, titleKeys);

        //        }
        //    };
        //    popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Hotspots() };
        //    popup.Show(true);
        //}

        //void RunImportLogic(List<Hotspot> importedSpots, Dictionary<string, Hotspot> titleKeys)
        //{
        //    var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

        //    foreach (var spot in importedSpots)
        //    {
        //        if (titleKeys.ContainsKey(spot.Title))
        //        {
        //            var oldSpot = titleKeys[spot.Title];
        //            spot.DebugAppearance.CopyAppearanceTo(oldSpot.DebugAppearance);
        //            spot.Settings = oldSpot.Settings;
        //        }
        //        else
        //            editor.Hotspots.Add(spot);
        //    }

        //    RebuildListBox();
        //}
        #endregion

        void RebuildProperties(Zone zone)
        {
            if (zone.Settings.Count == 0)
            {
                propertySurface.IsVisible = false;
            }
            else
            {
                propertySurface.IsVisible = true;
            }

            
            if (propertySurface.IsVisible)
            {
                var drawing = new DrawingSurface(Consoles.ToolPane.PanelWidthControls, (zone.Settings.Count * 2) + 1);

                previousProperties = new Dictionary<string, string>();
                int y = 1;
                drawing.Print(0, 0, "Zone Settings", Settings.Green);
                foreach (var setting in zone.Settings)
                {
                    drawing.Print(0, y, setting.Key.Length > Consoles.ToolPane.PanelWidthControls ? setting.Key.Substring(0, Consoles.ToolPane.PanelWidthControls) : setting.Key, Settings.Yellow);
                    drawing.Print(1, y + 1, setting.Value.Length > Consoles.ToolPane.PanelWidthControls - 1 ? setting.Value.Substring(0, Consoles.ToolPane.PanelWidthControls - 1) : setting.Value, Settings.Grey);
                    previousProperties[setting.Key] = setting.Value;
                    y += 2;
                }

                propertySurface.TextSurface = drawing.TextSurface;
            }
        }

        void Entity_SelectedItemChanged(object sender, ListBox<EntityListBoxItem>.SelectedItemEventArgs e)
        {
            if (EntityList.SelectedItem != null)
            {
                var entity = (ResizableObject<Zone>)EntityList.SelectedItem;
                var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

                moveSelectedUp.IsEnabled = editor.Zones.IndexOf(entity) != 0;
                moveSelectedDown.IsEnabled = editor.Zones.IndexOf(entity) != editor.Zones.Count - 1;
                renameZoneButton.IsEnabled = true;
                editSettings.IsEnabled = true;

                editor.SelectedEntity = entity.Entity;
                zoneColorPresenter.IsEnabled = true;
                zoneColorPresenter.SelectedColor = entity.Entity.Animation.CurrentFrame[0].Background;

                RebuildProperties(entity.Data);

                if (!rebuilding)
                    MainScreen.Instance.ToolsPane.RedrawPanels();
            }
            else
            {
                editSettings.IsEnabled = false;
                zoneColorPresenter.IsEnabled = false;
                moveSelectedDown.IsEnabled = false;
                moveSelectedUp.IsEnabled = false;
                renameZoneButton.IsEnabled = false;
                propertySurface.IsVisible = false;
            }

            removeSelected.IsEnabled = EntityList.Items.Count != 0;
        }

        public void RebuildListBox()
        {
            EntityList.Items.Clear();

            if (MainScreen.Instance.ActiveEditor is Editors.SceneEditor)
            {
                var entities = ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).Zones;

                if (entities.Count != 0)
                {
                    foreach (var item in entities)
                        EntityList.Items.Add(item);


                    EntityList.SelectedItem = EntityList.Items[0];
                }
            }
        }

        public override void ProcessMouse(MouseConsoleState info)
        {
        }
        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            return control == EntityList ? 1 : 0;
        }

        public override void Loaded()
        {
            rebuilding = true;
            var previouslySelected = EntityList.SelectedItem;
            RebuildListBox();
            if (EntityList.Items.Count != 0)
            {
                if (previouslySelected == null || !EntityList.Items.Contains(previouslySelected))
                    EntityList.SelectedItem = EntityList.Items[0];
                else
                    EntityList.SelectedItem = previouslySelected;
            }
            rebuilding = false;
        }

        private class EntityListBoxItem : ListBoxItem
        {
            public override void Draw(SurfaceBase surface, Microsoft.Xna.Framework.Rectangle area)
            {
                string value = ((ResizableObject)Item).Name;

                if (string.IsNullOrEmpty(value))
                    value = "<no name>";

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
