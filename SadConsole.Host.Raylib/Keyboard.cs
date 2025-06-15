using System.Collections.Generic;
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
        return Array.Empty<Keys>();
    }

    public bool IsKeyDown(Keys key)
    {
        return false;
    }

    public bool IsKeyUp(Keys key)
    {
        return false;
    }

    public void Refresh()
    {
        
    }
}
