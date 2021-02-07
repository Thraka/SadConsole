namespace SadConsoleEditor.Tools
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;
    using SadConsole.Entities;

    class CircleTool : ITool
    {
        public Entity Brush;
        private SadConsole.Effects.Fade frameEffect;
        private Point? firstPoint;
        private Point? secondPoint;
        private SadConsole.Shapes.Ellipse ellipseShape;
        private bool cancelled;

        private CircleToolPanel settingsPanel;
        private Cell borderAppearance;

        public const string ID = "CIRCLE";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Circle"; }
        }
        public char Hotkey { get { return 'c'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public override string ToString()
        {
            return Title;
        }

        public CircleTool()
        {
            frameEffect = new SadConsole.Effects.Fade()
            {
                UseCellBackground = true,
                FadeForeground = true,
                FadeDuration = 1f,
                AutoReverse = true
            };

            // Configure the animations
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            AnimatedSurface animation = new AnimatedSurface("single", 1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            animation.CreateFrame()[0].Glyph = 42;
            Brush.Animations.Add(animation.Name, animation);

            settingsPanel = new CircleToolPanel();
            ControlPanels = new CustomPanel[] { settingsPanel, CharacterPickPanel.SharedInstance };
            ResetCircle();
        }


        void ResetCircle()
        {
            firstPoint = null;
            secondPoint = null;

            settingsPanel.CircleWidth = 0;
            settingsPanel.CircleHeight = 0;

            Brush.Animation = Brush.Animations["single"];
        }

        public void OnSelected()
        {
            RefreshTool();
            ResetCircle();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = false;

            settingsPanel.CircleHeight = 0;
            settingsPanel.CircleWidth = 0;
            firstPoint = null;
            secondPoint = null;

        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }


        public void RefreshTool()
        {
            borderAppearance = new Cell(CharacterPickPanel.SharedInstance.SettingForeground,
                                   CharacterPickPanel.SharedInstance.SettingBackground,
                                   CharacterPickPanel.SharedInstance.SettingCharacter,
                                   CharacterPickPanel.SharedInstance.SettingMirrorEffect);

            Brush.Animation.IsDirty = true;
        }

        public void Update()
        {
        }

        public bool ProcessKeyboard(Keyboard info, SurfaceBase surface)
        {
            return false;
        }

        public void ProcessMouse(MouseConsoleState info, SurfaceBase surface, bool isInBounds)
        {
            if (cancelled)
            {
                // wait until left button is released...
                if (info.Mouse.LeftButtonDown)
                    return;
                else
                {
                    cancelled = false;
                }
            }

            if (info.Mouse.LeftButtonDown)
            {
                if (!firstPoint.HasValue)
                {
                    RefreshTool();
                    firstPoint = info.ConsolePosition;
                    return;
                }
                else
                {
                    // Check for right click cancel.
                    if (info.Mouse.RightButtonDown)
                    {
                        cancelled = true;
                        firstPoint = null;
                        secondPoint = null;
                        Brush.Animation = Brush.Animations["single"];
                        settingsPanel.CircleWidth = 0;
                        settingsPanel.CircleHeight = 0;
                        return;
                    }


                    secondPoint = info.ConsolePosition;

                    // Draw the line (erase old) to where the mouse is
                    // create the animation frame
                    AnimatedSurface animation = new AnimatedSurface("line", Math.Max(firstPoint.Value.X, secondPoint.Value.X) - Math.Min(firstPoint.Value.X, secondPoint.Value.X) + 1,
                                                                                    Math.Max(firstPoint.Value.Y, secondPoint.Value.Y) - Math.Min(firstPoint.Value.Y, secondPoint.Value.Y) + 1,
                                                                                    SadConsoleEditor.Settings.Config.ScreenFont);

                    var frame = animation.CreateFrame();

                    Point p1;

                    if (firstPoint.Value.X > secondPoint.Value.X)
                    {
                        if (firstPoint.Value.Y > secondPoint.Value.Y)
                            p1 = Point.Zero;
                        else
                            p1 = new Point(0, frame.Height - 1);
                    }
                    else
                    {
                        if (firstPoint.Value.Y > secondPoint.Value.Y)
                            p1 = new Point(frame.Width - 1, 0);
                        else
                            p1 = new Point(frame.Width - 1, frame.Height - 1);
                    }


                    animation.Center = p1;
                    settingsPanel.CircleWidth = animation.Width;
                    settingsPanel.CircleHeight = animation.Height;

                    SadConsoleEditor.Settings.QuickEditor.TextSurface = frame;

                    ellipseShape = new SadConsole.Shapes.Ellipse();
                    ellipseShape.BorderAppearance = borderAppearance;
                    ellipseShape.EndingPoint = new Point(frame.Width - 1, frame.Height - 1);
                    ellipseShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                    Brush.Animation = animation;

                }
            }
            else if (firstPoint.HasValue)
            {
                settingsPanel.CircleWidth = 0;
                settingsPanel.CircleHeight = 0;

                // We let go outside of bounds
                if (!isInBounds)
                {
                    firstPoint = null;
                    cancelled = true;
                    Brush.Animation = Brush.Animations["single"];
                    return;
                }

                // Position the shape and draw
                Point p1 = new Point(Math.Min(firstPoint.Value.X, secondPoint.Value.X), Math.Min(firstPoint.Value.Y, secondPoint.Value.Y));
                Point p2 = new Point(Math.Max(firstPoint.Value.X, secondPoint.Value.X), Math.Max(firstPoint.Value.Y, secondPoint.Value.Y));

                SadConsoleEditor.Settings.QuickEditor.TextSurface = surface;

                ellipseShape.StartingPoint = p1 + info.Console.TextSurface.RenderArea.Location;
                ellipseShape.EndingPoint = p2 + info.Console.TextSurface.RenderArea.Location;
                ellipseShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                Brush.Animation = Brush.Animations["single"];
                Brush.Position = secondPoint.Value;


                firstPoint = null;
                secondPoint = null;
            }
        }
    }
}
