using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Controls;
using SadConsoleEditor.Windows;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Surfaces;

namespace SadConsoleEditor.Panels
{
    class AnimationsPanel : CustomPanel
    {
        private ListBox animations;
        private Button removeSelected;
        private Button addNewAnimation;
        private Button renameAnimation;
        private Button addNewAnimationFromFile;
        private Button saveAnimationToFile;
        private Button changeSpeedButton;
        private Button setCenterButton;
        private Button setBoundingBoxButton;
        private Button cloneSelectedAnimationButton;
        private Button reverseAnimationButton;
        private CheckBox repeatCheck;
        private DrawingSurface animationSpeedLabel;
        private Button playPreview;

        private Entity entity;

        private Action<AnimatedSurface> animationChangeCallback;
        private Action<CustomTool> invokeCustomToolCallback;

        public enum CustomTool
        {
            Center,
            CollisionBox,
            None
        }

        public AnimationsPanel(Action<AnimatedSurface> animationChangeCallback)
        {
            Title = "Animations";
            animations = new ListBox(Consoles.ToolPane.PanelWidthControls, 4);
            animations.HideBorder = true;
            animations.SelectedItemChanged += animations_SelectedItemChanged;
            animations.CompareByReference = true;

            removeSelected = new Button(Consoles.ToolPane.PanelWidthControls);
            removeSelected.Text = "Remove";
            removeSelected.Click += removeAnimation_Click;

            addNewAnimation = new Button(Consoles.ToolPane.PanelWidthControls);
            addNewAnimation.Text = "Add New";
            addNewAnimation.Click += addNewAnimation_Click;

            renameAnimation = new Button(Consoles.ToolPane.PanelWidthControls);
            renameAnimation.Text = "Rename";
            renameAnimation.Click += renameAnimation_Click;

            addNewAnimationFromFile = new Button(Consoles.ToolPane.PanelWidthControls);
            addNewAnimationFromFile.Text = "Import Anim.";
            addNewAnimationFromFile.Click += addNewAnimationFromFile_Click;

            saveAnimationToFile = new Button(Consoles.ToolPane.PanelWidthControls);
            saveAnimationToFile.Text = "Export Anim.";
            saveAnimationToFile.Click += saveAnimationToFile_Click;

            changeSpeedButton = new Button(3);
            changeSpeedButton.ShowEnds = false;
            changeSpeedButton.Text = "Set";
            changeSpeedButton.Click += changeSpeedButton_Click;

            cloneSelectedAnimationButton = new Button(Consoles.ToolPane.PanelWidthControls);
            cloneSelectedAnimationButton.Text = "Clone Sel. Anim";
            cloneSelectedAnimationButton.Click += cloneSelectedAnimation_Click;

            reverseAnimationButton = new Button(Consoles.ToolPane.PanelWidthControls);
            reverseAnimationButton.Text = "Reverse Animation";
            reverseAnimationButton.Click += reverseAnimation_Click; ;

            setCenterButton = new Button(Consoles.ToolPane.PanelWidthControls);
            setCenterButton.Text = "Set Center";
            setCenterButton.Click += (s, e) => invokeCustomToolCallback(CustomTool.Center);

            setBoundingBoxButton = new Button(Consoles.ToolPane.PanelWidthControls);
            setBoundingBoxButton.Text = "Set Collision";
            setBoundingBoxButton.Click += (s, e) => invokeCustomToolCallback(CustomTool.CollisionBox);

            animationSpeedLabel = new DrawingSurface(Consoles.ToolPane.PanelWidthControls - changeSpeedButton.Width, 1);

            repeatCheck = new CheckBox(Consoles.ToolPane.PanelWidthControls, 1);
            repeatCheck.Text = "Repeat";
            repeatCheck.IsSelectedChanged += repeatCheck_IsSelectedChanged;

            playPreview = new Button(Consoles.ToolPane.PanelWidthControls);
            playPreview.Text = "Play Preview";
            playPreview.Click += playPreview_Click; ;

            this.animationChangeCallback = animationChangeCallback;
            //_invokeCustomToolCallback = invokeCustomToolCallback;

            Controls = new ControlBase[] { animations, null, removeSelected, addNewAnimation, renameAnimation, cloneSelectedAnimationButton, null, addNewAnimationFromFile, saveAnimationToFile, null, playPreview, null, animationSpeedLabel, changeSpeedButton, repeatCheck, null, reverseAnimationButton };
        }

        private void reverseAnimation_Click(object sender, EventArgs e)
        {
            var animation = (AnimatedSurface)animations.SelectedItem;
            animation.Frames.Reverse();
            animations_SelectedItemChanged(this, null);
        }

        private void cloneSelectedAnimation_Click(object sender, EventArgs e)
        {
            RenamePopup popup = new RenamePopup("clone");
            popup.Closed += (o, e2) =>
            {
                if (popup.DialogResult)
                {
                    var animation = (AnimatedSurface)animations.SelectedItem;
                    var newAnimation = new AnimatedSurface(popup.NewName, animation.Width, animation.Height, Settings.Config.ScreenFont);

                    foreach (var frame in animation.Frames)
                    {
                        var newFrame = newAnimation.CreateFrame();
                        frame.Copy(newFrame);
                    }

                    newAnimation.CurrentFrameIndex = 0;

                    entity.Animations[newAnimation.Name] = newAnimation;
                    RebuildListBox();
                }
            };
            popup.Show(true);
            popup.Center();
        }

        private void repeatCheck_IsSelectedChanged(object sender, EventArgs e)
        {
            ((AnimatedSurface)animations.SelectedItem).Repeat = repeatCheck.IsSelected;
        }

        private void changeSpeedButton_Click(object sender, EventArgs e)
        {
            var animation = (AnimatedSurface)animations.SelectedItem;
            AnimationSpeedPopup popup = new AnimationSpeedPopup(animation.AnimationDuration);
            popup.Closed += (s2, e2) =>
            {
                if (popup.DialogResult)
                {
                    animation.AnimationDuration = popup.NewSpeed;
                    animationSpeedLabel.Fill(Settings.Green, Settings.Color_MenuBack, 0, null);
                    animationSpeedLabel.Print(0, 0, new ColoredString("Speed: ", Settings.Green, Settings.Color_MenuBack) + new ColoredString(((AnimatedSurface)animations.SelectedItem).AnimationDuration.ToString(), Settings.Blue, Settings.Color_MenuBack));
                }
            };
            popup.Center();
            popup.Show(true);
        }

        void saveAnimationToFile_Click(object sender, EventArgs e)
        {
            var animation = (AnimatedSurface)animations.SelectedItem;

            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    popup.SelectedLoader.Save(animation, popup.SelectedFile);
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Animation() };
            popup.SelectButtonText = "Save";
            popup.SkipFileExistCheck = true;
            popup.Show(true);
            popup.Center();
        }

        void addNewAnimationFromFile_Click(object sender, EventArgs e)
        {
            SelectFilePopup popup = new SelectFilePopup();
            popup.Closed += (o2, e2) =>
            {
                if (popup.DialogResult)
                {
                    var animation = (AnimatedSurface)popup.SelectedLoader.Load(popup.SelectedFile);

                    entity.Animations[animation.Name] = animation;

                    RebuildListBox();
                }
            };
            popup.CurrentFolder = Environment.CurrentDirectory;
            popup.FileLoaderTypes = new FileLoaders.IFileLoader[] { new FileLoaders.Animation() };
            popup.Show(true);
            popup.Center();
        }

        void renameAnimation_Click(object sender, EventArgs e)
        {
            var animation = (AnimatedSurface)animations.SelectedItem;
            RenamePopup popup = new RenamePopup(animation.Name);
            popup.Closed += (o, e2) => 
            {
                if (popup.DialogResult)
                {
                    entity.Animations.Remove(animation.Name);
                    animation.Name = popup.NewName;
                    entity.Animations[animation.Name] = animation;
                    animations.IsDirty = true;
                }
            };
            popup.Show(true);
            popup.Center();
        }

        void removeAnimation_Click(object sender, EventArgs e)
        {
            var animation = (AnimatedSurface)animations.SelectedItem;

            if (animation.Name == "default")
            {
                SadConsole.Window.Message(new ColoredString("You cannot delete the default animation"), "Close");
            }
            else
            {
                entity.Animations.Remove(animation.Name);
                RebuildListBox();
                animations.SelectedItem = animations.Items[0];
            }
        }

        void addNewAnimation_Click(object sender, EventArgs e)
        {
            RenamePopup popup = new RenamePopup("", "Animation Name");
            popup.Closed += (o, e2) =>
            {

                if (popup.DialogResult)
                {
                    string newName = popup.NewName.Trim();
                    var keys = entity.Animations.Keys.Select(k => k.ToLower()).ToList();

                    if (keys.Contains(newName.ToLower()))
                    {
                        Window.Message("Name must be unique", "Close");
                    }
                    else if (string.IsNullOrEmpty(newName))
                    {
                        Window.Message("Name cannot be blank", "Close");
                    }
                    else
                    {
                        var previouslySelected = (AnimatedSurface)animations.SelectedItem;
                        var animation = new AnimatedSurface(newName, previouslySelected.Width, previouslySelected.Height, Settings.Config.ScreenFont);
                        animation.CreateFrame();
                        animation.AnimationDuration = 1;
                        entity.Animations[animation.Name] = animation;
                        RebuildListBox();
                        animations.SelectedItem = animation;
                    }
                }
            };
            popup.Show(true);
            popup.Center();
        }

        void animations_SelectedItemChanged(object sender, ListBox<ListBoxItem>.SelectedItemEventArgs e)
        {
            removeSelected.IsEnabled = animations.Items.Count != 1;

            renameAnimation.IsEnabled = true;

            if (animations.SelectedItem != null)
            {
                var animation = (AnimatedSurface)animations.SelectedItem;

                removeSelected.IsEnabled = animations.Items.Count != 1;

                repeatCheck.IsSelected = animation.Repeat;
                animationSpeedLabel.Fill(Settings.Green, Settings.Color_MenuBack, 0, null);
                animationSpeedLabel.Print(0, 0, new ColoredString("Speed: ", Settings.Green, Settings.Color_MenuBack) + new ColoredString(((AnimatedSurface)animations.SelectedItem).AnimationDuration.ToString(), Settings.Blue, Settings.Color_MenuBack));

                animationChangeCallback(animation);
            }
        }

        private void playPreview_Click(object sender, EventArgs e)
        {
            PreviewAnimationPopup popup = new PreviewAnimationPopup((AnimatedSurface)animations.SelectedItem);
            popup.Center();
            popup.Show(true);
        }

        public void RebuildListBox()
        {
            animations.Items.Clear();

            foreach (var item in entity.Animations)
            {
                animations.Items.Add(item.Value);

                if (item.Value == entity.Animation)
                    animations.SelectedItem = item.Value;
            }

            if (animations.SelectedItem == null)
                animations.SelectedItem = animations.Items[0];
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            if (control == changeSpeedButton)
            {
                animationSpeedLabel.Fill(Settings.Green, Settings.Color_MenuBack, 0, null);
                animationSpeedLabel.Print(0, 0, new ColoredString("Speed: ", Settings.Green, Settings.Color_MenuBack) + new ColoredString(((AnimatedSurface)animations.SelectedItem).AnimationDuration.ToString(), Settings.Blue, Settings.Color_MenuBack));
                changeSpeedButton.Position = new Microsoft.Xna.Framework.Point(Consoles.ToolPane.PanelWidth - changeSpeedButton.Width - 1, animationSpeedLabel.Position.Y);
            }

            return 0;
        }

        public override void Loaded()
        {
            //var previouslySelected = _animations.SelectedItem;
            //RebuildListBox();
            //if (previouslySelected == null || !_animations.Items.Contains(previouslySelected))
            //    _animations.SelectedItem = _animations.Items[0];
            //else
            //    _animations.SelectedItem = previouslySelected;
        }

        public void SetEntity(Entity entity)
        {
            this.entity = entity;
            RebuildListBox();
        }
    }
}
