using SadConsole.Input;

namespace SadBBSClient;

public partial class BbsScreen
{
    private struct KeyBinding
    {
        public bool Alt;
        public Keys Key;
        public Action OnPressed;
    }
}
