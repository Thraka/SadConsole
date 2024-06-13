import clr

clr.AddReference("SadConsole")
clr.AddReference("SadConsole.Host.MonoGame")
clr.AddReference("TheSadRogue.Primitives")

from SadConsole import Game, CellSurfaceEditor as Edit, Settings as SadConsoleSettings
from SadConsole import ScreenSurface, IScreenObject
from SadConsole import Configuration
from SadRogue.Primitives import Color as Color

SadConsoleSettings.WindowTitle = "Python Game"

def getGameConfig():
    config = Configuration.Builder()
    Configuration.Extensions.SetScreenSize(config, 20, 10)
    Configuration.Extensions.SetStartingScreen(config, lambda host: MyConsole())
    return config

class MyConsole(ScreenSurface):
    def __new__(cls, *args):
        if len(args) == 2:
            instance = ScreenSurface.__new__(cls, args[0], args[1])

        if len(args) == 0:
            instance = ScreenSurface.__new__(cls, 20, 10)
        else:
            raise ValueError('I created this class to only support constructors of (Width, Height)')

        Edit.FillWithRandomGarbage(instance, Game.Instance.DefaultFont)

        return instance

    def Update(self, delta):
        Edit.Print(self, 1, 2, "Frame update time:", Color.White, Color.Black, 0)
        Edit.Clear(self, 1, 3, 15)
        Edit.Print(self, 3, 3, delta.ToString("G"))
        Edit.Print(self, 1, 4, "Frame counter: ", Color.White, Color.Black, 0)
        Edit.Clear(self, 1, 5, 15)
        Edit.Print(self, 3, 5, Game.Instance.FrameNumber.ToString())
