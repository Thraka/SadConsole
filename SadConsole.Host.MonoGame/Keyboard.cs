using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SadConsole.MonoGame
{
    class Keyboard : SadConsole.Input.IKeyboardState
    {
        Microsoft.Xna.Framework.Input.KeyboardState _keyboard;
        SadConsole.Input.Keys[] _keys;

        public Keyboard()
        {
            _keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public bool CapsLock => throw new NotImplementedException();

        public bool NumLock => throw new NotImplementedException();

        public SadConsole.Input.Keys[] GetPressedKeys() =>
            _keyboard.GetPressedKeys().Cast<SadConsole.Input.Keys>().ToArray();

        public bool IsKeyDown(SadConsole.Input.Keys key) =>
            _keyboard.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key);

        public bool IsKeyUp(SadConsole.Input.Keys key) =>
            _keyboard.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)key);
    }
}
