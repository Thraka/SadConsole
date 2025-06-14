using System.Collections.Generic;
using SFML.Graphics;
using SadConsole.Input;
using System;
using System.Linq;

namespace SadConsole.Host;

public class Keyboard : IKeyboardState
{
    public bool CapsLock => throw new NotImplementedException();

    public bool NumLock => throw new NotImplementedException();

    public Keys[] GetPressedKeys()
    {
        throw new NotImplementedException();
    }

    public bool IsKeyDown(Keys key)
    {
        throw new NotImplementedException();
    }

    public bool IsKeyUp(Keys key)
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }
}
