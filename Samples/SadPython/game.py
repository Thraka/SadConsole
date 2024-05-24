import clr

clr.AddReference("SadConsole")
clr.AddReference("SadConsole.Host.MonoGame")

from SadConsole import Game, CellSurfaceEditor as Edit, Settings as SadConsoleSettings
from SadConsole import ScreenSurface, IScreenObject
from SadConsole import Configuration;

SadConsoleSettings.WindowTitle = "Python Game"

def getGameConfig():
    config = Configuration.Builder()
    Configuration.Extensions.SetScreenSize(config, 20, 10)
    #config.OnStart(Init)
    Configuration.Extensions.SetStartingScreen(config, lambda host: MyConsole())
    return config

def Init():
    Edit.FillWithRandomGarbage(Game.Instance.StartingConsole, Game.Instance.DefaultFont)


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
        Edit.Clear(self, 2, 2, 15)
        Edit.Print(self, 2, 2, delta.ToString("G"))
