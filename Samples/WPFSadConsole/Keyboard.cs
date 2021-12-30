using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SadConsole.Host
{
    class Keyboard : SadConsole.Input.IKeyboardState
    {
        Microsoft.Xna.Framework.Input.KeyboardState _keyboard;

        public Keyboard()
        {
            _keyboard = SadConsole.Game.Instance.MonoGameInstance.Keyboard.GetState();
        }

        public bool CapsLock => _keyboard.CapsLock;

        public bool NumLock => _keyboard.NumLock;

        public SadConsole.Input.Keys[] GetPressedKeys() =>
            _keyboard.GetPressedKeys().Cast<SadConsole.Input.Keys>().ToArray();

        public bool IsKeyDown(SadConsole.Input.Keys key) =>
            _keyboard.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key);

        public bool IsKeyUp(SadConsole.Input.Keys key) =>
            _keyboard.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)key);
    }
}
