using SadConsole.UI;

namespace SadConsole.Examples;

static class GameSettings
{
    public const int GAME_WIDTH = 120;
    public const int GAME_HEIGHT = 40;
    public const int SCREEN_DEMO_WIDTH = 80;
    public const int SCREEN_DEMO_HEIGHT = 25;
    public const int SCREEN_LIST_WIDTH = 30;
    public const int SCREEN_LIST_HEIGHT = 25;
    public const int SCREEN_DESCRIPTION_WIDTH = 50;
    public const int SCREEN_DESCRIPTION_HEIGHT = 8;

    public static Rectangle ScreenListBounds = new Rectangle(2, 2, SCREEN_LIST_WIDTH, SCREEN_LIST_HEIGHT);
    public static Rectangle ScreenDescriptionBounds = new Rectangle(2, ScreenListBounds.MaxExtentY + 4, SCREEN_DESCRIPTION_WIDTH, SCREEN_DESCRIPTION_HEIGHT);
    public static Rectangle ScreenDemoBounds = new Rectangle(ScreenListBounds.MaxExtentX + 5, 2, SCREEN_DEMO_WIDTH, SCREEN_DEMO_HEIGHT);

    public static Colors ControlColorScheme = Colors.CreateAnsi();

    public static object[] Demos =
        {
            "Basics".CreateColored(Color.OrangeRed),
            new DemoAutoTyping(),
            new DemoShapes(),
            new DemoStringParsing(),
            new DemoRandomScrolling(),
            new DemoPlayground(),

            "",
            "GUI/TUI".CreateColored(Color.OrangeRed),
            new DemoControls(),
            new DemoControls2(),
            new DemoScrollableViews(),

            "",
            "Layering".CreateColored(Color.OrangeRed),
            new DemoLayeredSurface(),

            "",
            "Terminal-like".CreateColored(Color.OrangeRed),
            new DemoScrollableConsole(),
            new DemoKeyboardHandlers(),
            new DemoMultipleCursors(),

            "",
            "ASCII and Graphics".CreateColored(Color.OrangeRed),
            new DemoAsciiGraphics(),
            new DemoAnimations(),
            new DemoTheDraw(),

            "",
            "Entities".CreateColored(Color.OrangeRed),
            new DemoEntitySurface(),
            
            "",
            "Renderers".CreateColored(Color.OrangeRed),
            new DemoSurfaceOpacity(),
            new DemoSecondSurfaceRenderer(),
            new DemoRotatedSurface(),
            new DemoCustomCellsRenderer(),
            //new DemoShader(),

            //"",
            //"Advanced".CreateColored(Color.OrangeRed),
            //new DemoFontManipulation(),
        };
}
