using System.Collections.Generic;
using SFML.Graphics;
using SadConsole.Input;
using System;

namespace SadConsole.Host;

class Keyboard : SadConsole.Input.IKeyboardState
{
    SadConsole.Input.Keys[] _keys;
    //List<SadConsole.Input.Keys> _keys = new List<Input.Keys>(5);
    private RenderWindow _window;
    private bool _capslock;

    public Keyboard(RenderWindow window)
    {
        _window = window;
        //window.KeyPressed += Window_KeyPressed;
        //window.KeyReleased += Window_KeyReleased;
    }

    ~Keyboard()
    {
        //_window.KeyPressed -= Window_KeyPressed;
        //window.KeyReleased -= Window_KeyReleased;
    }

    private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        //var sadKey = e.Code.ToSadConsole();

        //if (sadKey != Input.Keys.None && !_keys.Contains(sadKey))
        //    _keys.Add(sadKey);
    }

    private void Window_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
    {
        //var sadKey = e.Code.ToSadConsole();

        //if (sadKey != Input.Keys.None && _keys.Contains(sadKey))
        //    _keys.Remove(sadKey);
    }

    // TODO: Figure out capslock!
    public bool CapsLock => _capslock;

    // Forcing numlock on works as the getpressedkeys method will automatically return the keypad key vs keyboard lock key.
    public bool NumLock => true;

    public SadConsole.Input.Keys[] GetPressedKeys()
    {
        //return _keys.ToArray();
        List<Input.Keys> keysPressed = new List<Input.Keys>(5);

        for (int i = 0; i < (int)SFML.Window.Keyboard.Key.KeyCount; i++)
        {
            if (SFML.Window.Keyboard.IsKeyPressed((SFML.Window.Keyboard.Key)i))
                keysPressed.Add(((SFML.Window.Keyboard.Key)i).ToSadConsole());
        }

        return keysPressed.ToArray();
    }

    public bool IsKeyDown(SadConsole.Input.Keys key) =>
        SFML.Window.Keyboard.IsKeyPressed(key.ToSFML());

    public bool IsKeyUp(SadConsole.Input.Keys key) =>
        !SFML.Window.Keyboard.IsKeyPressed(key.ToSFML());

    public void Refresh() =>
        throw new NotImplementedException("This method is not used by the host.");
}
