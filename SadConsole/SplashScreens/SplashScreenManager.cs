using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.SplashScreens;

/// <summary>
/// GameHost use only. Use the <see cref="CheckRun"/> method to show any splash screens after <see cref="GameHost.OnStart"/> was called.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Splashscreen Manager")]
public class SplashScreenManager : ScreenObject
{
    /// <summary>
    /// Checks if any splash screens have been added with <see cref="GameHost.SetSplashScreens(ScreenSurface[])"/>, if so, starts them.
    /// </summary>
    public static void CheckRun()
    {
        if (GameHost.Instance._splashScreens.Count != 0)
        {
            GameHost.Instance.SaveGlobalState();
            GameHost.Instance.FocusedScreenObjects = new FocusedScreenObjectStack();
            GameHost.Instance.Screen = new ScreenObject();
            GameHost.Instance.Screen.Children.Add(new SplashScreenManager());
        }
    }

    /// <summary>
    /// Creates a new instance of this type.
    /// </summary>
    public SplashScreenManager()
    {
        _activeScreen = GameHost.Instance._splashScreens.Dequeue();
        Children.Add(_activeScreen);
        GameHost.Instance.FocusedScreenObjects.Set(_activeScreen);
    }

    private ScreenSurface _activeScreen;

    /// <inheritdoc/>
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (_activeScreen.IsVisible == false)
        {
            Children.Remove(_activeScreen);

            if (GameHost.Instance._splashScreens.Count != 0)
            {
                _activeScreen = GameHost.Instance._splashScreens.Dequeue();
                Children.Add(_activeScreen);
                GameHost.Instance.FocusedScreenObjects.Set(_activeScreen);
            }
            else
            {
                GameHost.Instance.RestoreGlobalState();
            }
        }
    }
}
