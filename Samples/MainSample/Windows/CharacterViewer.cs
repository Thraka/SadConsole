using System;
using SadConsole;
using SadConsole.Effects;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace FeatureDemo.Windows
{
    public class CharacterViewer : Window
    {
        private CharacterPicker _picker;
        private int _selectedCharacter;

        public Mirror MirrorEffect { get { return _picker.MirrorSetting; } set { _picker.MirrorSetting = value; } }

        public int SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                _picker.SelectedCharacterChanged -= SelectedCharacterChangedOnControl;
                _selectedCharacter = _picker.SelectedCharacter = value;
                _picker.SelectedCharacterChanged += SelectedCharacterChangedOnControl;
            }
        }

        public CharacterViewer(int character)
            : base(18, 18)
        {
            Center();
            _picker = new CharacterPicker(SadConsole.UI.Themes.Library.Default.Colors.OrangeDark, SadConsole.UI.Themes.Library.Default.Colors.ControlBackgroundNormal, SadConsole.UI.Themes.Library.Default.Colors.Yellow);
            _picker.Position = new Point(1, 1);
            _picker.SelectedCharacter = character;
            _picker.UseFullClick = true;
            _picker.SelectedCharacterChanged += SelectedCharacterChangedOnControl;
            Controls.Add(_picker);

            this.CloseOnEscKey = true;
            this.Title = "Pick a character";
        }

        private void SelectedCharacterChangedOnControl(object sender, ValueChangedEventArgs<int> e)
        {
            SelectedCharacter = e.NewValue;
            this.Hide();
        }
    }

}
