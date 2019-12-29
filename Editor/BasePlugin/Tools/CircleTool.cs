namespace SadConsoleEditor.Tools
{
    using SadRogue.Primitives;
    using SadConsole;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;
    using SadConsole.Entities;
    using Console = SadConsole.Console;

    public class CircleTool : ITool
    {
        private AnimatedScreenSurface animSinglePoint;
        private SadConsole.Effects.Fade frameEffect;
        private Point? firstPoint;
        private Point secondPoint;
        private bool cancelled;

        private BoxToolPanel _settingsPanel;

        public Entity Brush;

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
            animSinglePoint = new AnimatedScreenSurface("single", 1, 1, Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            animSinglePoint.CreateFrame();
            animSinglePoint.CurrentFrame[0].Glyph = 42;

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
            Brush = new Entity(1, 1, Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            Brush.Animations.Add(animSinglePoint.Name, animSinglePoint);
            Brush.Animation = animSinglePoint;
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
            MainConsole.Instance.Brush = Brush;

            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainConsole.Instance.QuickSelectPane.IsVisible = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainConsole.Instance.QuickSelectPane.IsVisible = false;

            firstPoint = null;
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            Brush.Animation.IsDirty = true;
        }

        public void Update()
        {
        }

        public bool ProcessKeyboard(Keyboard info, Console surface)
        {
            return false;
        }

        public void ProcessMouse(MouseScreenObjectState info, Console surface, bool isInBounds)
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
                    firstPoint = info.SurfaceCellPosition;
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

                    if (secondPoint == info.SurfaceCellPosition)
                        return;

                    secondPoint = info.SurfaceCellPosition;
                    
                    // Draw the line (erase old) to where the mouse is
                    // create the animation frame
                    var animation = new AnimatedScreenSurface("line", Math.Max(firstPoint.Value.X, secondPoint.X) - Math.Min(firstPoint.Value.X, secondPoint.X) + 1,
                                                                                    Math.Max(firstPoint.Value.Y, secondPoint.Y) - Math.Min(firstPoint.Value.Y, secondPoint.Y) + 1,
                                                                                    Config.Program.ScreenFont, Config.Program.ScreenFontSize);

                    var frame = animation.CreateFrame();

                    Point p1;

                    if (firstPoint.Value.X > secondPoint.X)
                    {
                        if (firstPoint.Value.Y > secondPoint.Y)
                            p1 = (0, 0);
                        else
                            p1 = new Point(0, frame.BufferHeight - 1);
                    }
                    else
                    {
                        if (firstPoint.Value.Y > secondPoint.Y)
                            p1 = new Point(frame.BufferWidth - 1, 0);
                        else
                            p1 = new Point(frame.BufferWidth - 1, frame.BufferHeight - 1);
                    }


                    animation.Center = p1;

                    var fillCell = _settingsPanel.UseFill ? new ColoredGlyph(_settingsPanel.FillForeColor, _settingsPanel.FillBackColor, _settingsPanel.FillGlyph) : null;
                    var borderCell = new ColoredGlyph(_settingsPanel.LineForeColor, _settingsPanel.LineBackColor, _settingsPanel.LineGlyph);
                    frame.DrawCircle(new Rectangle(0, 0, frame.BufferWidth, frame.BufferHeight), borderCell, fillCell);

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

                var fillCell = _settingsPanel.UseFill ? new ColoredGlyph(_settingsPanel.FillForeColor, _settingsPanel.FillBackColor, _settingsPanel.FillGlyph) : null;
                var borderCell = new ColoredGlyph(_settingsPanel.LineForeColor, _settingsPanel.LineBackColor, _settingsPanel.LineGlyph);

                if (info.ScreenObject.Surface.IsScrollable)
                    surface.DrawCircle(new Rectangle(Math.Min(firstPoint.Value.X, secondPoint.X) + surface.ViewPosition.X,
                                                  Math.Min(firstPoint.Value.Y, secondPoint.Y) + surface.ViewPosition.Y,
                                                  Brush.Animation.Width, Brush.Animation.Height),
                                    borderCell, fillCell);
                else
                    surface.DrawCircle(new Rectangle(Math.Min(firstPoint.Value.X, secondPoint.X),
                                                  Math.Min(firstPoint.Value.Y, secondPoint.Y),
                                                  Brush.Animation.Width, Brush.Animation.Height),
                                    borderCell, fillCell);

                firstPoint = null;

                Brush.Animation = Brush.Animations["single"];
            }
        }
    }
}
