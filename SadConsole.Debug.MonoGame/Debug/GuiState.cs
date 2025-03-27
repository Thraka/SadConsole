using System.Collections.Generic;

namespace SadConsole.Debug;

static class GuiState
{
    public static IScreenObject? _selectedScreenObject;
    public static ScreenObjectState? _selectedScreenObjectState;
    public static ScreenObjectState? _hoveredScreenObjectState;
    public static Dictionary<IScreenObject, ScreenObjectState> ScreenObjectUniques = new Dictionary<IScreenObject, ScreenObjectState>();

    //public static void Update()
    //{
    //    if (_oldShowSadConsoleRendering != ShowSadConsoleRendering)
    //    {
    //        _oldShowSadConsoleRendering = ShowSadConsoleRendering;
    //        ShowSadConsoleRenderingChanged?.Invoke(null, EventArgs.Empty);
    //    }
    //}
    public static void RefreshScreenObject()
    {
        _selectedScreenObject = null;
        _selectedScreenObjectState = null;
        _hoveredScreenObjectState = null;

        ScreenObjectUniques.Clear();
        ScreenObjectState._identifierCounter = 0;
        ScreenObjectState.Create(Game.Instance.Screen);
    }
}
