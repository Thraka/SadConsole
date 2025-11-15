using SadConsole.UI;

namespace SadConsole.Examples;

partial class RootScreen : ScreenObject
{
    private IDemo _activeDemo;

    private ListDemosScreen _listDemosScreen;
    private Console _demoDescriptionsScreen;
    private IScreenObject? _demoObject;

    public RootScreen()
    {
        _demoDescriptionsScreen = new(GameSettings.ScreenDescriptionBounds.Width, GameSettings.ScreenDescriptionBounds.Height);
        _demoDescriptionsScreen.Position = GameSettings.ScreenDescriptionBounds.Position;

        _listDemosScreen = new(GameSettings.ScreenListBounds.Width, GameSettings.ScreenListBounds.Height);
        _listDemosScreen.Position = GameSettings.ScreenListBounds.Position;
        _listDemosScreen.SelectedDemoChanged += (s, demo) => SetDemo(demo);

        SadConsole.UI.Border.CreateForSurface(_listDemosScreen, "Demos");
        SadConsole.UI.Border.CreateForSurface(_demoDescriptionsScreen, "Description");

        Children.Add(_listDemosScreen);
        Children.Add(_demoDescriptionsScreen);

        SetDemo(_listDemosScreen.CurrentDemo);
    }

    private void SetDemo(IDemo? demo)
    {
        if (demo == null) return;

        _activeDemo = demo;

        // Clear old demo screen
        if (_demoObject != null)
            Children.Remove(_demoObject);

        // Add the new demo screen
        IScreenSurface demoSurface = demo.CreateDemoScreen();

        SadConsole.UI.Border.CreateForSurface(demoSurface, "");

        demoSurface.Position = GameSettings.ScreenDemoBounds.Position;

        _demoObject = demoSurface;
        Children.Add(_demoObject);
        _demoObject.IsFocused = true;
        demo.PostCreateDemoScreen(demoSurface);

        // Remove old description component
        while (_demoDescriptionsScreen.HasSadComponent<LineCharacterFade>(out LineCharacterFade? component))
            _demoDescriptionsScreen.SadComponents.Remove(component);

        // Clear the description window and rewrite it
        _demoDescriptionsScreen.Clear();
        _demoDescriptionsScreen.Print(0, 0, ColoredString.Parser.Parse($"[c:r f:Yellow]Name: [c:u]{_activeDemo.Title}"));
        _demoDescriptionsScreen.Print(0, 1, ColoredString.Parser.Parse($"[c:r f:Yellow]File: [c:u]{_activeDemo.CodeFile}"));
        _demoDescriptionsScreen.DrawLine((0, 2), (_demoDescriptionsScreen.Surface.Width, 2), ICellSurface.ConnectedLineThick[(int)ICellSurface.ConnectedLineIndex.Top]);
        _demoDescriptionsScreen.Cursor.Move(0, 3).Print(ColoredString.Parser.Parse(_activeDemo.Description));
        _demoDescriptionsScreen.SadComponents.Add(new LineCharacterFade(TimeSpan.FromSeconds(0.3)));
    }

    public override void Update(TimeSpan delta)
    {
        // Update all the base objects
        base.Update(delta);

        // Even though keyboard handling happens before this, on the demo surface, which is focused
        // we want to listen for special keyboard keys that control the demo.
    }
}
