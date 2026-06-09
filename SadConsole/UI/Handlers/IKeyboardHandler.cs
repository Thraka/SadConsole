using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Handlers;

public interface IKeyboardHandler
{
    bool ProcessKeyboard(Keyboard state, ControlBase? origin);
}

