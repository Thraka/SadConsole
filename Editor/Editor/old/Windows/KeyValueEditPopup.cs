using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Windows
{
    class KeyValueEditPopup : SadConsole.Window
    {
        private class SettingKeyValue
        {
            public string Key;
            public string Value;

            public override string ToString()
            {
                return Key;
            }
        }

        private Button saveButton;
        private Button cancelButton;
        private Button addFieldButton;
        private Button removeFieldButton;
        private ListBox objectSettingsListbox;
        private InputBox settingNameInput;
        private InputBox settingValueInput;

        private SpriteEffects _settingMirrorEffect;

        public Dictionary<string, string> SettingsDictionary;
        
        public KeyValueEditPopup(Dictionary<string, string> settings)
            : base(55, 18)
        {
            Title = "Settings";
            
            // Setting controls of the game object
            objectSettingsListbox = new ListBox(25, 10);
            settingNameInput = new InputBox(25);
            settingValueInput = new InputBox(25);
            objectSettingsListbox.HideBorder = true;


            objectSettingsListbox.Position = new Point(2, 3);
            settingNameInput.Position = new Point(objectSettingsListbox.Bounds.Right + 1, objectSettingsListbox.Bounds.Top);
            settingValueInput.Position = new Point(objectSettingsListbox.Bounds.Right + 1, settingNameInput.Bounds.Bottom + 2);

            addFieldButton = new Button(14, 1);
            addFieldButton.Text = "Add/Update";
            addFieldButton.Position = new Point(settingValueInput.Bounds.Left, settingValueInput.Bounds.Bottom + 1);

            removeFieldButton = new Button(14, 1);
            removeFieldButton.Text = "Remove";
            removeFieldButton.Position = new Point(objectSettingsListbox.Bounds.Left, objectSettingsListbox.Bounds.Bottom + 1);
            removeFieldButton.IsEnabled = false;

            objectSettingsListbox.SelectedItemChanged += _objectSettingsListbox_SelectedItemChanged;
            addFieldButton.Click += _addFieldButton_Click;
            removeFieldButton.Click += _removeFieldButton_Click;

            Add(objectSettingsListbox);
            Add(settingNameInput);
            Add(settingValueInput);
            Add(addFieldButton);
            Add(removeFieldButton);

            // Save/close buttons
            saveButton = new Button(10, 1);
            cancelButton = new Button(10, 1);

            saveButton.Text = "Save";
            cancelButton.Text = "Cancel";

            saveButton.Click += _saveButton_Click;
            cancelButton.Click += (o, e) => { DialogResult = false; Hide(); };

            saveButton.Position = new Point(TextSurface.Width - 12, 16);
            cancelButton.Position = new Point(2, 16);

            Add(saveButton);
            Add(cancelButton);

            // Read the settings
            foreach (var item in settings)
                objectSettingsListbox.Items.Add(new SettingKeyValue() { Key = item.Key, Value = item.Value });

            SettingsDictionary = settings;

            Redraw();
        }

        void _saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        void _objectSettingsListbox_SelectedItemChanged(object sender, ListBox<ListBoxItem>.SelectedItemEventArgs e)
        {
            if (objectSettingsListbox.SelectedItem != null)
            {
                removeFieldButton.IsEnabled = true;

                var objectSetting = (SettingKeyValue)objectSettingsListbox.SelectedItem;

                settingNameInput.Text = objectSetting.Key;
                settingValueInput.Text = objectSetting.Value;
            }
            else
            {
                removeFieldButton.IsEnabled = false;

                settingNameInput.Text = "";
                settingValueInput.Text = "";
            }
        }

        void _updateFieldButton_Click(object sender, EventArgs e)
        {
            var objectSetting = (SettingKeyValue)objectSettingsListbox.SelectedItem;

            objectSetting.Key = settingNameInput.Text;
            objectSetting.Value = settingValueInput.Text;
        }

        void _removeFieldButton_Click(object sender, EventArgs e)
        {
            if (objectSettingsListbox.SelectedItem != null)
                objectSettingsListbox.Items.Remove(objectSettingsListbox.SelectedItem);
        }

        void _addFieldButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(settingNameInput.Text))
            {
                foreach (var item in objectSettingsListbox.Items.Cast<SettingKeyValue>())
                {
                    if (item.Key == settingNameInput.Text)
                    {
                        var updated = item;
                        updated.Value = settingValueInput.Text;
                        SettingsDictionary[item.Key] = item.Value;
                        return;
                    }
                }

                SettingKeyValue objectSetting = new SettingKeyValue();
                objectSetting.Key = settingNameInput.Text;
                objectSetting.Value = settingValueInput.Text;
                objectSettingsListbox.Items.Add(objectSetting);
            }
        }

        public override void Show(bool modal)
        {
            base.Show(modal);

            Center();
        }

        public override void Hide()
        {
            if (DialogResult)
            {
                // Fill in the game object with all the new values
                SettingsDictionary = new Dictionary<string, string>();

                foreach (var item in objectSettingsListbox.Items.Cast<SettingKeyValue>())
                    SettingsDictionary.Add(item.Key, item.Value);
            }

            base.Hide();
        }

        public override void Redraw()
        {
            base.Redraw();

            if (objectSettingsListbox != null)
            {
                Print(objectSettingsListbox.Position.X, objectSettingsListbox.Position.Y - 1, "Settings/Flags", Settings.Green);
                Print(settingNameInput.Position.X, settingNameInput.Position.Y - 1, "Setting Name", Settings.Green);
                Print(settingValueInput.Position.X, settingValueInput.Position.Y - 1, "Setting Value", Settings.Green);

                //SetGlyph(0, 9, 199);
                //for (int i = 1; i < 20; i++)
                //    SetGlyph(i, 9, 196);

                //SetGlyph(20, 9, 191);

                //for (int i = 10; i < 18; i++)
                //    SetGlyph(20, i, 179);

                //SetGlyph(20, 18, 192);

                //for (int i = 21; i < TextSurface.Width; i++)
                //    SetGlyph(i, 18, 196);

                //SetGlyph(TextSurface.Width - 1, 18, 182);

                //// Long line under field
                SetGlyph(0, cancelButton.Bounds.Top - 1, 199);
                for (int i = 1; i < TextSurface.Width; i++)
                    SetGlyph(i, cancelButton.Bounds.Top - 1, 196);
                SetGlyph(TextSurface.Width - 1, cancelButton.Bounds.Top - 1, 182);
            }
        }
            
    }
}
