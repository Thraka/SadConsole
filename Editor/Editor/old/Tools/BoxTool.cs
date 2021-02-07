namespace SadConsoleEditor.Tools
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;
    using SadConsole.Entities;

    class BoxTool : ITool
    {
        private AnimatedSurface animSinglePoint;
        private SadConsole.Effects.Fade frameEffect;
        private Point? firstPoint;
        private Point secondPoint;
        private SadConsole.Shapes.Box boxShape;
        private bool cancelled;

        private BoxToolPanel _settingsPanel;

        public Entity Brush;

        public const string ID = "BOX";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Box"; }
        }

        public char Hotkey { get { return 'b'; } }


        public CustomPanel[] ControlPanels { get; private set; }

        public override string ToString()
        {
            return Title;
        }

        public BoxTool()
        {
            animSinglePoint = new AnimatedSurface("single", 1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            var _frameSinglePoint = animSinglePoint.CreateFrame();
            _frameSinglePoint[0].Glyph = 42;


            frameEffect = new SadConsole.Effects.Fade()
            {
                UseCellBackground = true,
                FadeForeground = true,
                FadeDuration = 1f,
                AutoReverse = true
            };

            _settingsPanel = new BoxToolPanel();

            ControlPanels = new CustomPanel[] { _settingsPanel };

            // 
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            AnimatedSurface animation = new AnimatedSurface("single", 1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            animation.CreateFrame()[0].Glyph = 42;
            Brush.Animations.Add(animation.Name, animation);
            Brush.Animation = animation;
        }

        void ResetBox()
        {
            firstPoint = null;

            Brush.Animation = Brush.Animations["single"];
        }


        public void OnSelected()
        {
            RefreshTool();
            ResetBox();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = false;

            firstPoint = null;
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
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
                    cancelled = false;
            }

            if (info.Mouse.LeftButtonDown)
            {
                if (!firstPoint.HasValue)
                {
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
                        return;
                    }


                    secondPoint = info.ConsolePosition;

                    // Draw the line (erase old) to where the mouse is
                    // create the animation frame
                    AnimatedSurface animation = new AnimatedSurface("line", Math.Max(firstPoint.Value.X, secondPoint.X) - Math.Min(firstPoint.Value.X, secondPoint.X) + 1,
                                                                                    Math.Max(firstPoint.Value.Y, secondPoint.Y) - Math.Min(firstPoint.Value.Y, secondPoint.Y) + 1,
                                                                                    SadConsoleEditor.Settings.Config.ScreenFont);

                    var frame = animation.CreateFrame();

                    Point p1;

                    if (firstPoint.Value.X > secondPoint.X)
                    {
                        if (firstPoint.Value.Y > secondPoint.Y)
                            p1 = Point.Zero;
                        else
                            p1 = new Point(0, frame.Height - 1);
                    }
                    else
                    {
                        if (firstPoint.Value.Y > secondPoint.Y)
                            p1 = new Point(frame.Width - 1, 0);
                        else
                            p1 = new Point(frame.Width - 1, frame.Height - 1);
                    }


                    animation.Center = p1;

                    SadConsoleEditor.Settings.QuickEditor.TextSurface = frame;
                    boxShape = SadConsole.Shapes.Box.GetDefaultBox();

                    if (_settingsPanel.UseCharacterBorder)
                        boxShape.LeftSideCharacter = boxShape.RightSideCharacter =
                        boxShape.TopLeftCharacter = boxShape.TopRightCharacter = boxShape.TopSideCharacter =
                        boxShape.BottomLeftCharacter = boxShape.BottomRightCharacter = boxShape.BottomSideCharacter =
                        _settingsPanel.BorderCharacter;

                    boxShape.Foreground = _settingsPanel.LineForeColor;
                    boxShape.FillColor = _settingsPanel.FillColor;
                    boxShape.Fill = _settingsPanel.UseFill;
                    boxShape.BorderBackground = _settingsPanel.LineBackColor;
                    boxShape.Position = new Point(0, 0);
                    boxShape.Width = frame.Width;
                    boxShape.Height = frame.Height;
                    boxShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                    Brush.Animation = animation;
                    
                }
            }
            else if (firstPoint.HasValue)
            {
                // We let go outside of bounds
                if (!isInBounds)
                {
                    cancelled = true;
                    return;
                }

                // Position the box shape and draw
                boxShape.Position = new Point(Math.Min(firstPoint.Value.X, secondPoint.X), Math.Min(firstPoint.Value.Y, secondPoint.Y)) 
                                    + info.Console.TextSurface.RenderArea.Location;

                SadConsoleEditor.Settings.QuickEditor.TextSurface = surface;
                boxShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                firstPoint = null;

                Brush.Animation = Brush.Animations["single"];
            }
        }
    }
}
