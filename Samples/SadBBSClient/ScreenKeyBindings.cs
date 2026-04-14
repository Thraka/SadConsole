using System.Diagnostics.CodeAnalysis;
using SadConsole.Input;

namespace SadBBSClient;

public partial class BbsScreen
{
    private class ScreenKeyBindings
    {
        public List<KeyBinding> Bindings = [];
        public void Add(bool alt, Keys key, Action onPressed)
        {
            Bindings.Add(new KeyBinding { Alt = alt, Key = key, OnPressed = onPressed });
        }
        public bool TryMatch(Keyboard keyboard, [NotNullWhen(true)] out KeyBinding? keyBinding)
        {
            foreach (var binding in Bindings)
            {
                if (keyboard.IsKeyPressed(binding.Key) && (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt)) == binding.Alt)
                {
                    keyBinding = binding;
                    return true;
                }
            }
            keyBinding = null;
            return false;
        }
    }
}
