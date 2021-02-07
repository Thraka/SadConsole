using SadRogue.Primitives;
using SadConsoleEditor.Controls;
using SadConsole;

namespace SadConsoleEditor.Windows
{
    class CharacterQuickSelectPopup : SadConsole.UI.Window
    {
        private Controls.CharacterPicker _picker;
        private int _selectedCharacter;

        public Mirror MirrorEffect { get { return _picker.MirrorEffect; } set { _picker.MirrorEffect = value; } }

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

        public CharacterQuickSelectPopup(int character)
            : base(18, 18)
        {
            Center();
            _picker = new Controls.CharacterPicker(SadConsole.UI.Themes.Library.Default.Colors.OrangeDark, SadConsole.UI.Themes.Library.Default.Colors.ControlBackgroundNormal, SadConsole.UI.Themes.Library.Default.Colors.Yellow);
            _picker.Position = new Point(1, 1);
            _picker.SelectedCharacter = character;
            _picker.UseFullClick = true;
            _picker.SelectedCharacterChanged += SelectedCharacterChangedOnControl;
            Controls.Add(_picker);

            this.CloseOnEscKey = true;
            this.Title = "Pick a character";
        }

        private void SelectedCharacterChangedOnControl(object sender, CharacterPicker.SelectedCharacterEventArgs e)
        {
            SelectedCharacter = e.NewCharacter;
            this.Hide();
        }
    }
}
