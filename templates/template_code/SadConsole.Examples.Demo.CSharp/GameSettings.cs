using SadConsole.UI;

namespace SadConsole.Examples;

static class GameSettings
{
    public const int GAME_WIDTH = 120;
    public const int GAME_HEIGHT = 44;
    public const int SCREEN_DEMO_WIDTH = 80;
    public const int SCREEN_DEMO_HEIGHT = 25;
    public const int SCREEN_LIST_WIDTH = 30;
    public const int SCREEN_LIST_HEIGHT = 25;
    public const int SCREEN_DESCRIPTION_WIDTH = 50;
    public const int SCREEN_DESCRIPTION_HEIGHT = 12;

    public static Rectangle ScreenListBounds = new(2, 2, SCREEN_LIST_WIDTH, SCREEN_LIST_HEIGHT);
    public static Rectangle ScreenDescriptionBounds = new(2, ScreenListBounds.MaxExtentY + 4, SCREEN_DESCRIPTION_WIDTH, SCREEN_DESCRIPTION_HEIGHT);
    public static Rectangle ScreenDemoBounds = new(ScreenListBounds.MaxExtentX + 5, 2, SCREEN_DEMO_WIDTH, SCREEN_DEMO_HEIGHT);

    public static Colors ControlColorScheme = Colors.CreateAnsi();

    public static object[] Demos = BuildDemos();

    private static object[] BuildDemos()
    {
        object[] builtInDemos =
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
            new DemoBinaryFont(),

            "",
            "Entities".CreateColored(Color.OrangeRed),
            new DemoEntitySurface(),

            "",
            "Renderers".CreateColored(Color.OrangeRed),
            new DemoSurfaceOpacity(),
            new DemoSecondSurfaceRenderer(),
            new DemoRotatedSurface(),
            new DemoCustomCellsRenderer(),
            new DemoShader(),

            //"",
            //"Advanced".CreateColored(Color.OrangeRed),
            //new DemoFontManipulation(),
        };

        // Discover IDemo types not already in the built-in list
        HashSet<Type> builtInTypes = [.. builtInDemos.OfType<IDemo>().Select(d => d.GetType())];

        object[] localDemos = System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IDemo).IsAssignableFrom(t)
                        && !builtInTypes.Contains(t))
            .Select(t => (object)System.Activator.CreateInstance(t)!)
            .ToArray();

        if (localDemos.Length == 0)
            return builtInDemos;

        List<object> result = ["Local".CreateColored(Color.Green), .. localDemos, "", .. builtInDemos];
        return result.ToArray();
    }
}
