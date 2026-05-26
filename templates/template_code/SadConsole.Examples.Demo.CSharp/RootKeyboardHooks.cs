using SadConsole.Components;
using SadConsole.SplashScreens;
using SadConsole.UI;

namespace SadConsole.Examples;

public class RootKeyboardHooksRootComponent: RootComponent
{
    private static bool _isPromptOpen;
    private bool _waitingForEscRelease;

    public static void PromptClose()
    {
        if (_isPromptOpen) return;

        _isPromptOpen = true;
        Window.Prompt("Close the demo?", "Yes", "No", closed =>
        {
            _isPromptOpen = false;
            if (closed)
            {
#if MONOGAME
                SadConsole.Game.Instance.MonoGameInstance.Exit();
#elif SFML
                SadConsole.Host.Global.GraphicsDevice.Close();
#endif
                SadConsole.Game.Instance.Dispose();
            }
        });
    }

    public override void Run(TimeSpan delta)
    {
        // Don't interfere with splash screens
        if (Game.Instance.Screen!.Children.OfType<SplashScreenManager>().Any())
            return;

        bool modalVisible = Game.Instance.Screen!.Children.OfType<Window>().Any(w => w.IsVisible && w.IsModal);

        // If a modal was open, wait for Esc to be fully released before listening again
        if (modalVisible)
        {
            _waitingForEscRelease = true;
            return;
        }

        if (_waitingForEscRelease)
        {
            if (Game.Instance.Keyboard.IsKeyDown(Input.Keys.Escape))
                return;

            _waitingForEscRelease = false;
        }

        // If Esc is pressed, prompt to close the demo
        if (Game.Instance.Keyboard.HasKeysDown && Game.Instance.Keyboard.IsKeyDown(Input.Keys.Escape))
        {
            Game.Instance.Keyboard.Clear();
            PromptClose();
        }

        // If Alt+Enter is pressed, toggle full screen
        bool isAltDown = Game.Instance.Keyboard.IsKeyDown(Input.Keys.LeftAlt) || Game.Instance.Keyboard.IsKeyDown(Input.Keys.RightAlt);

        if (Game.Instance.Keyboard.HasKeysDown && isAltDown && Game.Instance.Keyboard.IsKeyReleased(Input.Keys.Enter))
            Game.Instance.ToggleFullScreen();
    }
}
