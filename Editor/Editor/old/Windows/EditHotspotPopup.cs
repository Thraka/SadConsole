using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsoleEditor.Windows
{
    class EditHotspotPopup : SadConsole.Window
    {
        private Button saveButton;
        private Button cancelButton;
        private Button addFieldButton;
        private Button removeFieldButton;
        private ListBox objectSettingsListbox;
        private InputBox nameInput;
        private InputBox settingNameInput;
        private InputBox settingValueInput;
        private Controls.ColorPresenter foregroundPresenter;
        private Controls.ColorPresenter backgroundPresenter;
        private Controls.ColorPresenter characterPresenter;
        private CheckBox mirrorHorizCheck;
        private CheckBox mirrorVertCheck;
        private Controls.CharacterPicker characterPicker;

        private SpriteEffects settingMirrorEffect;

        public Hotspot CreatedHotspot;

        public Cell SettingAppearance
        {
            get { return new Cell(foregroundPresenter.SelectedColor, backgroundPresenter.SelectedColor, characterPresenter.Character); }
            set
            {
                foregroundPresenter.SelectedColor = value.Foreground;
                backgroundPresenter.SelectedColor = value.Background;
                characterPresenter.SelectedColor = value.Background;
                characterPresenter.CharacterColor = value.Foreground;
                characterPresenter.Character = value.Glyph;
            }
        }
        
        public EditHotspotPopup(Hotspot oldHotspot)
            : base(39, 29)
        {
            textSurface.Font = Settings.Config.ScreenFont;
            Title = "Hotspot Editor";

            CreatedHotspot = new Hotspot();
            CreatedHotspot.Title = oldHotspot.Title;
            CreatedHotspot.Positions = new List<Point>(oldHotspot.Positions);
            oldHotspot.DebugAppearance.CopyAppearanceTo(CreatedHotspot.DebugAppearance);

            foreach (var key in oldHotspot.Settings.Keys)
                CreatedHotspot.Settings[key] = oldHotspot.Settings[key];


            // Settings of the appearance fields
            nameInput = new InputBox(13);
            characterPicker = new SadConsoleEditor.Controls.CharacterPicker(Settings.Red, Settings.Color_ControlBack, Settings.Green);
            foregroundPresenter = new SadConsoleEditor.Controls.ColorPresenter("Foreground", Settings.Green, 18);
            backgroundPresenter = new SadConsoleEditor.Controls.ColorPresenter("Background", Settings.Green, 18);
            characterPresenter = new SadConsoleEditor.Controls.ColorPresenter("Preview", Settings.Green, 18);
            mirrorHorizCheck = new CheckBox(18, 1);
            mirrorVertCheck = new CheckBox(18, 1);

            characterPicker.SelectedCharacterChanged += (o, e) => characterPresenter.Character = characterPicker.SelectedCharacter;
            foregroundPresenter.ColorChanged += (o, e) => characterPresenter.CharacterColor = foregroundPresenter.SelectedColor;
            backgroundPresenter.ColorChanged += (o, e) => characterPresenter.SelectedColor = backgroundPresenter.SelectedColor;

            Print(2, 2, "Name", Settings.Green);
            nameInput.Position = new Point(7, 2);
            foregroundPresenter.Position = new Point(2, 4);
            backgroundPresenter.Position = new Point(2, 5);
            characterPresenter.Position = new Point(2, 6);
            characterPicker.Position = new Point(21, 2);
            mirrorHorizCheck.Position = new Point(2, 7);
            mirrorVertCheck.Position = new Point(2, 8);

            nameInput.Text = "New";

            mirrorHorizCheck.IsSelectedChanged += Mirror_IsSelectedChanged;
            mirrorVertCheck.IsSelectedChanged += Mirror_IsSelectedChanged;
            mirrorHorizCheck.Text = "Mirror Horiz.";
            mirrorVertCheck.Text = "Mirror Vert.";

            foregroundPresenter.SelectedColor = Color.White;
            backgroundPresenter.SelectedColor = Color.DarkRed;

            characterPresenter.CharacterColor = foregroundPresenter.SelectedColor;
            characterPresenter.SelectedColor = backgroundPresenter.SelectedColor;
            characterPicker.SelectedCharacter = 1;

            Add(characterPicker);
            Add(nameInput);
            Add(foregroundPresenter);
			Add(backgroundPresenter);
            Add(characterPresenter);
            Add(mirrorHorizCheck);
            Add(mirrorVertCheck);

            // Setting controls of the game object
            objectSettingsListbox = new ListBox(18, 7);
            settingNameInput = new InputBox(18);
            settingValueInput = new InputBox(18);
            objectSettingsListbox.HideBorder = true;


            Print(2, 10, "Settings/Flags", Settings.Green);
            objectSettingsListbox.Position = new Point(2, 11);

            Print(2, 19, "Setting Name", Settings.Green);
            Print(2, 22, "Setting Value", Settings.Green);
            settingNameInput.Position = new Point(2, 20);
            settingValueInput.Position = new Point(2, 23);

            addFieldButton = new Button(16, 1);
            addFieldButton.Text = "Save/Update";
            addFieldButton.Position = new Point(TextSurface.Width - 18, 20);

            removeFieldButton = new Button(16, 1);
            removeFieldButton.Text = "Remove";
            removeFieldButton.Position = new Point(TextSurface.Width - 18, 21);
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

            saveButton.Position = new Point(TextSurface.Width - 12, 26);
            cancelButton.Position = new Point(2, 26);

            Add(saveButton);
            Add(cancelButton);

            // Read the Entity
            nameInput.Text = CreatedHotspot.Title;
            mirrorHorizCheck.IsSelected = (CreatedHotspot.DebugAppearance.Mirror & Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally) == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            mirrorVertCheck.IsSelected = (CreatedHotspot.DebugAppearance.Mirror & Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically) == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
            characterPicker.SelectedCharacter = CreatedHotspot.DebugAppearance.Glyph;
            foregroundPresenter.SelectedColor = CreatedHotspot.DebugAppearance.Foreground;
            backgroundPresenter.SelectedColor = CreatedHotspot.DebugAppearance.Background;

            foreach (var item in CreatedHotspot.Settings)
            {
                var newSetting = new SettingKeyValue() { Key = item.Key, Value = item.Value };
                objectSettingsListbox.Items.Add(newSetting);
            }

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

        void Mirror_IsSelectedChanged(object sender, EventArgs e)
        {
            if (mirrorHorizCheck.IsSelected && mirrorVertCheck.IsSelected)
                characterPresenter[characterPresenter.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically | Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            else if (mirrorHorizCheck.IsSelected)
                characterPresenter[characterPresenter.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            else if (mirrorVertCheck.IsSelected)
                characterPresenter[characterPresenter.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
            else
                characterPresenter[characterPresenter.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            characterPicker.MirrorEffect = characterPicker.MirrorEffect = settingMirrorEffect = characterPresenter[characterPresenter.Width - 2, 0].Mirror;
        }

        public override void Hide()
        {
            if (DialogResult)
            {
                // Fill in the game object with all the new values
                CreatedHotspot.Title = nameInput.Text;
                CreatedHotspot.DebugAppearance = new Cell(foregroundPresenter.SelectedColor, backgroundPresenter.SelectedColor, characterPicker.SelectedCharacter, settingMirrorEffect);
                CreatedHotspot.Settings.Clear();

                foreach (var item in objectSettingsListbox.Items.Cast<SettingKeyValue>())
                    CreatedHotspot.Settings[item.Key] = item.Value;
            }

            base.Hide();
        }

        public override void Redraw()
        {
            base.Redraw();

            Print(2, 2, "Name", Settings.Green);
            Print(2, 10, "Settings/Flags", Settings.Green);
            Print(2, 19, "Setting Name", Settings.Green);
            Print(2, 22, "Setting Value", Settings.Green);

            SetGlyph(0, 9, 199);
            for (int i = 1; i < 20; i++)
                SetGlyph(i, 9, 196);

            SetGlyph(20, 9, 191);

            for (int i = 10; i < 18; i++)
                SetGlyph(20, i, 179);

            SetGlyph(20, 18, 192);

            for (int i = 21; i < TextSurface.Width; i++)
                SetGlyph(i, 18, 196);

            SetGlyph(TextSurface.Width - 1, 18, 182);

            // Long line under field
            SetGlyph(0, 24, 199);
            for (int i = 1; i < TextSurface.Width; i++)
                SetGlyph(i, 24, 196);
            SetGlyph(TextSurface.Width - 1, 24, 182);
        }

        private class SettingKeyValue
        {
            public string Key;
            public string Value;

            public override string ToString()
            {
                return Key;
            }
        }
    }
}
