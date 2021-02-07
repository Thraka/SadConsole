using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsoleEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Windows
{
    class CharacterQuickSelectPopup : SadConsole.Window
    {
        private Controls.CharacterPicker _picker;

        public Microsoft.Xna.Framework.Graphics.SpriteEffects MirrorEffect { get { return _picker.MirrorEffect; } set { _picker.MirrorEffect = value; } }

        public int SelectedCharacter { get; private set; }

        public CharacterQuickSelectPopup(int character)
            : base(18, 18)
        {
            Center();
            _picker = new Controls.CharacterPicker(Settings.Red, Settings.Color_ControlBack, Settings.Green);
            _picker.Position = new Point(1, 1);
            _picker.SelectedCharacter = character;
            _picker.UseFullClick = true;
            _picker.SelectedCharacterChanged += (sender, e) =>
            {
                SelectedCharacter = e.NewCharacter;
                this.Hide();
            };
            Add(_picker);

            this.CloseOnESC = true;
            this.Title = "Pick a character";
        }
    }
}
