namespace MyGame.Scenes;

class RootScene : ScreenObject
{
    private ScreenSurface _mainSurface;

    public RootScene()
    {
        // Create a surface that's the same size as the screen.
        _mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        // Fill the surface with random characters and colors
        _mainSurface.FillWithRandomGarbage(_mainSurface.Font);
        
        // Create a rectangle box that has a violet foreground and black background.
        // Characters are reset to 0 and mirroring is set to none. FillWithRandomGarbage will
        // select random characters and mirroring, so this resets it within the box.
        _mainSurface.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, Mirror.None);
        
        // Print some text at (4, 4) using the foreground and background already there (violet and black)
        _mainSurface.Print(4, 4, "Hello from SadConsole");

        // Add _mainSurface as a child object of this one. This object, RootScene, is a simple object
        // and doesn't display anything itself. Since _mainSurface is going to be a child of it, _mainSurface
        // will be displayed.
        Children.Add(_mainSurface);
    }
}
