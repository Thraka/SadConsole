using System.Linq;

namespace SadConsole.Host;

class Keyboard : SadConsole.Input.IKeyboardState
{
    Microsoft.Xna.Framework.Input.KeyboardState _keyboard;

    public Keyboard() =>
        Refresh();

#if !FNA
    public bool CapsLock => _keyboard.CapsLock;
#else
    public bool CapsLock => false;
#endif

#if !FNA
    public bool NumLock => _keyboard.NumLock;
#else
    public bool NumLock => false;

#endif

    public SadConsole.Input.Keys[] GetPressedKeys() =>
        _keyboard.GetPressedKeys().Cast<SadConsole.Input.Keys>().ToArray();

    public bool IsKeyDown(SadConsole.Input.Keys key) =>
        _keyboard.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key);

    public bool IsKeyUp(SadConsole.Input.Keys key) =>
        _keyboard.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)key);

    public void Refresh() =>
#if WPF
        _keyboard = SadConsole.Game.Instance.MonoGameInstance.Keyboard.GetState();
#else
        _keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
#endif
}
