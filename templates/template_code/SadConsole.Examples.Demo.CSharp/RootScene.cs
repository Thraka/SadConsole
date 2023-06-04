using SadConsole.Components;
using SadConsole.UI;

namespace SadConsole.Examples;

class RootScene : ScreenObject
{
    private IDemo _activeDemo;

    private ListDemosScreen _listDemosScreen;
    private ControlsConsole _demoDescriptionsScreen;
    private IScreenObject? _demoObject;

    public RootScene()
    {
        _demoDescriptionsScreen = new ControlsConsole(GameSettings.ScreenDescriptionBounds.Width, GameSettings.ScreenDescriptionBounds.Height);
        _demoDescriptionsScreen.Position = GameSettings.ScreenDescriptionBounds.Position;

        _listDemosScreen = new ListDemosScreen(GameSettings.ScreenListBounds.Width, GameSettings.ScreenListBounds.Height);
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
        var component = _demoDescriptionsScreen.GetSadComponent<LineCharacterFade>();
        if (component != null)
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

        // Even though keyboard handling happens before this, on the focused object,
        // which is the demo surface, we want to listen for special keyboard keys
        // that control the demo.
    }

    private class LineCharacterFade : Instructions.InstructionBase
    {
        private Effects.ICellEffect _effect;

        private ColoredString[] _lines;
        private Cursor[] _lineCursors;
        private Instructions.DrawString[] _lineDrawers;
        private TimeSpan _totalTimeToPrint;

        public LineCharacterFade(TimeSpan totalTimeToPrint)
        {
            RemoveOnFinished = true;
            _totalTimeToPrint = totalTimeToPrint;

            _effect = new Effects.Fade()
            {
                FadeForeground = true,
                DestinationForeground = new Gradient(new[] { Color.Violet, Color.Black, Color.Gray, Color.Purple }, new[] { 0.0f, 0.01f, 0.5f, 1.0f }),
                UseCellDestinationReverse = true,
                UseCellForeground = true,
                CloneOnAdd = true,
                FadeDuration = TimeSpan.FromMilliseconds(300),
                RemoveOnFinished = true,
            };
        }

        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            base.Update(componentHost, delta);

            foreach(var instruction in _lineDrawers) 
                instruction?.Update(componentHost, delta);
        }

        public override void OnAdded(IScreenObject host)
        {
            if (host is not IScreenSurface obj) throw new ArgumentException($"Instruction can only be added to {nameof(IScreenSurface)}");

            _lines = new ColoredString[obj.Surface.Height];
            _lineDrawers = new Instructions.DrawString[obj.Surface.Height];
            _lineCursors = new Cursor[obj.Surface.Height];

            for (int i = 0; i < _lines.Length; i++)
            {
                int stringLength = obj.GetString(0, i, obj.Surface.Width).Trim('\0').Length;

                if (stringLength == 0) continue;

                ColoredString coloredString = obj.GetStringColored(0, i, stringLength);
                coloredString.SetEffect(_effect);
                coloredString.IgnoreEffect = false;
                _lineDrawers[i] = new Instructions.DrawString(coloredString);
                _lineDrawers[i].TotalTimeToPrint = _totalTimeToPrint;
                _lineDrawers[i].Position = (0, i);
                _lineDrawers[i].Cursor = new Cursor(obj.Surface)
                {
                    AutomaticallyShiftRowsUp = false
                };
            }

            obj.Clear();

            base.OnAdded(host);
        }
    }
}
