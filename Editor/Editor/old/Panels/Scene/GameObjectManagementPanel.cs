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

namespace SadConsoleEditor.Panels
{
    class EntityManagementPanel : CustomPanel
    {
        private ListBox<EntityListBoxItem> EntityList;
        private Button removeSelected;
        private Button moveSelectedUp;
        private Button moveSelectedDown;
        private Button renameLayer;
        private Button importEntity;
        private CheckBox drawEntitysCheckbox;

        private DrawingSurface animationListTitle;
        ListBox<AnimationListBoxItem> animationsListBox;
        Button playAnimationButton;

        public bool DrawObjects
        {
            get { return drawEntitysCheckbox.IsSelected; }
            set { drawEntitysCheckbox.IsSelected = value; }
        }


        public ResizableObject SelectedEntity
        {
            get { return EntityList.SelectedItem as ResizableObject; }
            set { EntityList.SelectedItem = value; }
        }

        public EntityManagementPanel()
        {
            Title = "Entities";
            EntityList = new ListBox<EntityListBoxItem>(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 4);
            EntityList.HideBorder = true;
            EntityList.SelectedItemChanged += Entity_SelectedItemChanged;
            EntityList.CompareByReference = true;

            removeSelected = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            removeSelected.Text = "Remove";
            removeSelected.Click += RemoveSelected_Click;

            moveSelectedUp = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            moveSelectedUp.Text = "Move Up";
            moveSelectedUp.Click += MoveSelectedUp_Click;

            moveSelectedDown = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            moveSelectedDown.Text = "Move Down";
            moveSelectedDown.Click += MoveSelectedDown_Click;
            
            renameLayer = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            renameLayer.Text = "Rename";
            renameLayer.Click += RenameEntity_Click;

            importEntity = new Button(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            importEntity.Text = "Import File";
            importEntity.Click += ImportEntity_Click;

            drawEntitysCheckbox = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            drawEntitysCheckbox.IsSelected = true;
            drawEntitysCheckbox.Text = "Draw Objects";

            animationListTitle = new DrawingSurface(Consoles.ToolPane.PanelWidthControls, 2);
            animationListTitle.Print(0, 0, "Animations", Settings.Green);

            animationsListBox = new ListBox<AnimationListBoxItem>(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 4);
            animationsListBox.SelectedItemChanged += AnimationList_SelectedItemChanged;
            animationsListBox.HideBorder = true;
            animationsListBox.CompareByReference = true;

            playAnimationButton = new Button(Consoles.ToolPane.PanelWidthControls);
            playAnimationButton.Text = "Play Animation";
            playAnimationButton.Click += (o, e) => { if (animationsListBox.SelectedItem != null) ((AnimatedSurface)animationsListBox.SelectedItem).Restart(); };


            Controls = new ControlBase[] { EntityList, removeSelected, moveSelectedUp, moveSelectedDown, renameLayer, importEntity, null, drawEntitysCheckbox, null, animationListTitle,animationsListBox, null, playAnimationButton };

            Entity_SelectedItemChanged(null, null);
        }

        void ImportEntity_Click(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    var entity = (Entity)popup.SelectedLoader.Load(popup.SelectedFile);
                    entity.Position = new Microsoft.Xna.Framework.Point(0, 0);
                    //entity.RenderOffset = (EditorConsoleManager.ActiveEditor as Editors.SceneEditor).Position;
                    (MainScreen.Instance.ActiveEditor as Editors.SceneEditor)?.LoadEntity(entity);
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Entity() };
            popup.Show(true);
            popup.Center();
        }

        void RenameEntity_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject)EntityList.SelectedItem;
            RenamePopup popup = new RenamePopup(entity.Name);
            popup.Closed += (o, e2) =>
            {
                if (popup.DialogResult)
                {
                    var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;
                    editor.RenameEntity(entity, popup.NewName);
                }

                EntityList.IsDirty = true;
            };
            popup.Show(true);
            popup.Center();
        }

        void MoveSelectedDown_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            int index = editor.Objects.IndexOf(entity);
            editor.Objects.Remove(entity);
            editor.Objects.Insert(index + 1, entity);
            RebuildListBox();
            EntityList.SelectedItem = entity;
        }

        void MoveSelectedUp_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            int index = editor.Objects.IndexOf(entity);
            editor.Objects.Remove(entity);
            editor.Objects.Insert(index - 1, entity);
            RebuildListBox();
            EntityList.SelectedItem = entity;
        }

        void RemoveSelected_Click(object sender, EventArgs e)
        {
            var entity = (ResizableObject)EntityList.SelectedItem;
            var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

            editor.RemoveEntity(entity);

            RebuildListBox();

            if (EntityList.Items.Count != 0)
                EntityList.SelectedItem = EntityList.Items[0];
        }

        void Entity_SelectedItemChanged(object sender, ListBox<EntityListBoxItem>.SelectedItemEventArgs e)
        {
            if (EntityList.SelectedItem != null)
            {
                var entity = (ResizableObject)EntityList.SelectedItem;
                var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;

                moveSelectedUp.IsEnabled = editor.Objects.IndexOf(entity) != 0;
                moveSelectedDown.IsEnabled = editor.Objects.IndexOf(entity) != editor.Objects.Count - 1;
                renameLayer.IsEnabled = true;
             
                editor.SelectedEntity = entity.Entity;
            }
            else
            {
                moveSelectedDown.IsEnabled = false;
                moveSelectedUp.IsEnabled = false;
                renameLayer.IsEnabled = false;
            }

            removeSelected.IsEnabled = EntityList.Items.Count != 0;
            RebuildAnimationListBox();
        }

        private void AnimationList_SelectedItemChanged(object sender, ListBox<AnimationListBoxItem>.SelectedItemEventArgs e)
        {
            if (animationsListBox.SelectedItem != null)
            {
                var animation = (AnimatedSurface)animationsListBox.SelectedItem;
                var editor = (Editors.SceneEditor)MainScreen.Instance.ActiveEditor;
                animation.CurrentFrameIndex = 0;
                editor.SelectedEntity.Animation = animation;

            }
        }

        public void RebuildListBox()
        {
            EntityList.Items.Clear();

            if (MainScreen.Instance.ActiveEditor is Editors.SceneEditor)
            {
                var entities = ((Editors.SceneEditor)MainScreen.Instance.ActiveEditor).Objects;

                if (entities.Count != 0)
                {
                    foreach (var item in entities)
                        EntityList.Items.Add(item);


                    EntityList.SelectedItem = EntityList.Items[0];
                }
            }
        }

        public void RebuildAnimationListBox()
        {
            animationsListBox.Items.Clear();

            if (EntityList.SelectedItem != null)
            {
                var animations = ((ResizableObject)EntityList.SelectedItem).Entity.Animations;

                if (animations.Count != 0)
                {
                    foreach (var item in animations.Values)
                        animationsListBox.Items.Add(item);

                    animationsListBox.SelectedItem = ((ResizableObject)EntityList.SelectedItem).Entity.Animation;
                }
            }
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            return control == EntityList ? 1 : 0;
        }

        public override void Loaded()
        {
            var previouslySelected = EntityList.SelectedItem;
            RebuildListBox();
            if (EntityList.Items.Count != 0)
            {
                if (previouslySelected == null || !EntityList.Items.Contains(previouslySelected))
                    EntityList.SelectedItem = EntityList.Items[0];
                else
                    EntityList.SelectedItem = previouslySelected;
            }
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

        private class AnimationListBoxItem : ListBoxItem
        {
            public override void Draw(SurfaceBase surface, Microsoft.Xna.Framework.Rectangle area)
            {
                string value = ((AnimatedSurface)Item).Name;

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
