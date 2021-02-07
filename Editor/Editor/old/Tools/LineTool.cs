namespace SadConsoleEditor.Tools
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Controls;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;

    class LineTool : ITool
    {
        public SadConsole.GameHelpers.Entity Brush;

        private SadConsole.Effects.Fade frameEffect;
        private Point? firstPoint;
        private Point? secondPoint;
        private SadConsole.Shapes.Line lineShape;
        private Cell lineCell;
        private Cell lineStyle;
        private LineToolPanel settingsPanel;
        private bool cancelled;
        private bool leftDown;

        public const string ID = "LINE";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Line"; }
        }
        public char Hotkey { get { return 'l'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public override string ToString()
        {
            return Title;
        }

        public LineTool()
        {
            frameEffect = new SadConsole.Effects.Fade()
            {
                UseCellBackground = true,
                FadeForeground = true,
                FadeDuration = 1f,
                AutoReverse = true
            };

            lineCell = new Cell();

            settingsPanel = new LineToolPanel();
            ControlPanels = new CustomPanel[] { settingsPanel, CharacterPickPanel.SharedInstance };

            // Configure the animations
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            AnimatedSurface animation = new AnimatedSurface("single", 1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            animation.CreateFrame()[0].Glyph = 42;
            Brush.Animations.Add(animation.Name, animation);
            ResetLine();
        }
        
        void ResetLine()
        {
            firstPoint = null;
            secondPoint = null;
            lineShape = null;

            Brush.Animation = Brush.Animations["single"];

            settingsPanel.LineLength = 0;
        }

        public void OnSelected()
        {
            RefreshTool();
            ResetLine();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = true;
		}

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = false;

            settingsPanel.LineLength = 0;
            firstPoint = null;
            secondPoint = null;
            lineShape = null;

        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            lineStyle = new Cell(CharacterPickPanel.SharedInstance.SettingForeground,
                                              CharacterPickPanel.SharedInstance.SettingBackground,
                                              CharacterPickPanel.SharedInstance.SettingCharacter,
                                              CharacterPickPanel.SharedInstance.SettingMirrorEffect);

            lineStyle.CopyAppearanceTo(lineCell);
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

            if (!isInBounds)
                return;

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
                        settingsPanel.LineLength = 0;

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
                    Point p2;

                    if (firstPoint.Value.X > secondPoint.Value.X)
                    {
                        if (firstPoint.Value.Y > secondPoint.Value.Y)
                        {
                            p1 = Point.Zero;
                            p2 = new Point(frame.Width - 1, frame.Height - 1);
                        }
                        else
                        {
                            p1 = new Point(0, frame.Height - 1);
                            p2 = new Point(frame.Width - 1, 0);
                        }
                    }
                    else
                    {
                        if (firstPoint.Value.Y > secondPoint.Value.Y)
                        {
                            p1 = new Point(frame.Width - 1, 0);
                            p2 = new Point(0, frame.Height - 1);
                        }
                        else
                        {
                            p1 = new Point(frame.Width - 1, frame.Height - 1);
                            p2 = Point.Zero;
                        }
                    }


                    animation.Center = p1;

                    SadConsoleEditor.Settings.QuickEditor.TextSurface = frame;

                    lineShape = new SadConsole.Shapes.Line();
                    lineShape.Cell = lineCell;
                    lineShape.UseEndingCell = false;
                    lineShape.UseStartingCell = false;
                    lineShape.StartingLocation = p1;
                    lineShape.EndingLocation = p2;
                    lineShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                    settingsPanel.LineLength = frame.Width > frame.Height ? frame.Width : frame.Height;

                    Brush.Animation = animation;

                }
            }
            else if (firstPoint.HasValue)
            {
                settingsPanel.LineLength = 0;

                // We let go outside of bounds
                if (!isInBounds)
                {
                    firstPoint = null;
                    Brush.Animation = Brush.Animations["single"];
                    cancelled = true;
                    return;
                }

                // Position the shape and draw
                SadConsoleEditor.Settings.QuickEditor.TextSurface = surface;

                lineShape.StartingLocation = firstPoint.Value + info.Console.TextSurface.RenderArea.Location;
                lineShape.EndingLocation = info.ConsolePosition + info.Console.TextSurface.RenderArea.Location;
                lineShape.Draw(SadConsoleEditor.Settings.QuickEditor);

                Brush.Animation = Brush.Animations["single"];
                Brush.Position = secondPoint.Value;


                firstPoint = null;
                secondPoint = null;
            }
        }



        private void SetAnimationLine(Point mousePosition)
        {
            // Draw the line (erase old) to where the mouse is
            // create the animation frame
            AnimatedSurface animation = new AnimatedSurface("line", Math.Max(firstPoint.Value.X, mousePosition.X) - Math.Min(firstPoint.Value.X, mousePosition.X) + 1,
                                                                            Math.Max(firstPoint.Value.Y, mousePosition.Y) - Math.Min(firstPoint.Value.Y, mousePosition.Y) + 1,
                                                                            SadConsoleEditor.Settings.Config.ScreenFont);


            var frame = animation.CreateFrame();

            Point p1;
            Point p2;

            if (firstPoint.Value.X > mousePosition.X)
            {
                if (firstPoint.Value.Y > mousePosition.Y)
                {
                    p1 = new Point(frame.Width - 1, frame.Height - 1);
                    p2 = new Point(0, 0);
                }
                else
                {
                    p1 = new Point(frame.Width - 1, 0);
                    p2 = new Point(0, frame.Height - 1);
                }
            }
            else
            {
                if (firstPoint.Value.Y > mousePosition.Y)
                {
                    p1 = new Point(0, frame.Height - 1);
                    p2 = new Point(frame.Width - 1, 0);
                }
                else
                {
                    p1 = new Point(0, 0);
                    p2 = new Point(frame.Width - 1, frame.Height - 1);
                }
            }

            animation.Center = p1;

            //_lineStyle = new Cell(
            //                    MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingForeground,
            //                    MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingBackground,
            //                    MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingCharacter);
            //_lineStyle.SpriteEffect = MainScreen.Instance.Instance.ToolPane.CommonCharacterPickerPanel.SettingMirrorEffect;
            //_lineStyle.CopyAppearanceTo(_lineCell);

            

            Brush.Animation = animation;
        }


    }
}
