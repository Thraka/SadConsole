namespace SadConsoleEditor.Tools
{
    using SadRogue.Primitives;
    using SadConsole;
    using SadConsole.Input;
    using System;
    using SadConsoleEditor.Panels;
    using SadConsoleEditor;
    using SadConsole.Entities;
    using Console = SadConsole.Console;

    public class LayeredEntity : Entity
    {
        public bool ForceDraw = false;
        public Entity SelectedSurface;

        public bool ShowSelectedSurface { get { return SelectedSurface.IsVisible; } set { SelectedSurface.IsVisible = value; } }

        public LayeredEntity(): base(1, 1, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize)
        {
            SelectedSurface = new Entity(1, 1, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            SelectedSurface.Parent = this;
        }
    }

    public class SelectionTool : ITool
    {
        private static CellSurface stashedBrush;

        private const string AnimationSingle = "single";
        private const string AnimationSelection = "selection";

        public LayeredEntity Brush;
        public SadConsoleEditor.ResizableObject SelectionBox;

        private SadConsole.Effects.Fade _frameEffect;
        private Point? firstPoint;
        private Point secondPoint;
        private SelectionToolPanel _panel;
        private SelectionToolAltPanel _altPanel;
        private SadConsole.Effects.Fade _pulseAnimation;
        private Console _previousSurface;

        private SelectionToolPanel.CloneState _previousState;

        public const string ID = "SELECT";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Selection"; }
        }
        public char Hotkey { get { return 's'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public override string ToString()
        {
            return Title;
        }

        public SelectionTool()
        {
            Brush = new LayeredEntity();
            
            var animation = new AnimatedScreenSurface(AnimationSingle, 1, 1, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            animation.CreateFrame()[0].Glyph = 42;
            animation.Frames[0][0].Background = Color.Black;
            Brush.Animations.Add(animation.Name, animation);

            animation = new AnimatedScreenSurface(AnimationSelection, 1, 1, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            animation.CreateFrame();
            Brush.Animations.Add(animation.Name, animation);

            _frameEffect = new SadConsole.Effects.Fade()
            {
                UseCellBackground = true,
                FadeForeground = true,
                FadeDuration = 1f,
                AutoReverse = true
            };

            _panel = new SelectionToolPanel(LoadBrush, SaveBrush, StashBrush, RestoreBrush, MakeTextHandler);
            _panel.StateChangedHandler = PanelStateChanged;
            _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;

            _altPanel = new SelectionToolAltPanel();

            ControlPanels = new CustomPanel[] { _panel, _altPanel };

            _pulseAnimation = new SadConsole.Effects.Fade()
            {
                FadeBackground = true,
                UseCellBackground = false,
                DestinationBackground = Color.Transparent,
                FadeDuration = 2d,
                CloneOnApply = false,
                AutoReverse = true,
                Repeat = true,
            };
        }

        public void StashBrush()
        {
            stashedBrush = SaveBrush();
        }

        public void RestoreBrush()
        {
            if (stashedBrush != null)
                LoadBrush(stashedBrush);
        }

        public void MakeTextHandler(CellSurface textSurface)
        {
            LoadBrush(textSurface);
        }

        private void PanelStateChanged(SelectionToolPanel.CloneState state)
        {
            if (state == SelectionToolPanel.CloneState.SelectingPoint1)
            {
                Brush.ForceDraw = false;
                Brush.ShowSelectedSurface = false;
                Brush.IsVisible = false;
                Brush.Animation = Brush.Animations[AnimationSingle];
                firstPoint = null;
                MainConsole.Instance.AllowKeyboardToMoveConsole = true;

            }
            else if (state == SelectionToolPanel.CloneState.Move)
            {
                Brush.ForceDraw = false;
                var animation = Brush.Animation;
                Brush.ShowSelectedSurface = true;

                var editPosition = firstPoint.Value;

                if (_previousSurface.IsScrollable)
                    editPosition += _previousSurface.ViewPosition;

                ClearBrush(editPosition.X, editPosition.Y, _previousSurface.Surface);
                animation.Center = new Point(animation.Width / 2, animation.Height / 2);
                Brush.SelectedSurface.Animation.Center = Brush.Animation.Center;
            }
            else if (state == SelectionToolPanel.CloneState.Clear)
            {
                Brush.ForceDraw = false;
                var animation = Brush.Animation;
                Brush.ShowSelectedSurface = false;

                var editPosition = firstPoint.Value;

                if (_previousSurface.IsScrollable)
                    editPosition += _previousSurface.ViewPosition;

                ClearBrush(editPosition.X, editPosition.Y, _previousSurface);
                _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
            }
            else if (state == SelectionToolPanel.CloneState.Stamp)
            {
                Brush.ForceDraw = false;
                var animation = Brush.Animation;
                Brush.ShowSelectedSurface = true;
                animation.Center = new Point(animation.Width / 2, animation.Height / 2);
                //Brush.Position += animation.Center + new Point(1);
                Brush.SelectedSurface.Animation.Center = Brush.Animation.Center;
            }
            else
            {
                Brush.ForceDraw = true;
                MainConsole.Instance.AllowKeyboardToMoveConsole = false;
            }

        }

        void ResetSelection()
        {
            _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
        }

        private CellSurface SaveBrush()
        {
            CellSurface newSurface = new CellSurface(Brush.SelectedSurface.Animation.CurrentFrame.BufferWidth,
                                                     Brush.SelectedSurface.Animation.CurrentFrame.BufferHeight);

            Brush.SelectedSurface.Animation.CurrentFrame.Copy(newSurface);

            return newSurface;
        }

        public void LoadBrush(CellSurface surface)
        {
            _panel.State = SelectionToolPanel.CloneState.Stamp;

            // Copy data to new animation
            var cloneAnimation = new AnimatedScreenSurface("clone", surface.BufferWidth, surface.BufferHeight, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            var frame = cloneAnimation.CreateFrame();
            surface.Copy(frame);

            cloneAnimation.Center = new Point(cloneAnimation.Width / 2, cloneAnimation.Height / 2);

            Brush.SelectedSurface.Animation = cloneAnimation;
            //Brush.Animation.Tint = new Color(0f, 0f, 0f, 0f);

            Brush.IsVisible = true;

            MakeBoxAnimation(surface.BufferWidth, surface.BufferHeight, cloneAnimation.Center);
        }

        private void MakeBoxAnimation(int width, int height, Point center)
        {
            AnimatedScreenSurface animation;

            if (Brush.Animations.ContainsKey(AnimationSelection))
            {
                animation = Brush.Animations[AnimationSelection];

                if (animation.Width == width && animation.Height == height && animation.Center == center)
                {
                    Brush.Animation = animation;
                    return;
                }
            }
            
            animation = new AnimatedScreenSurface(AnimationSelection, width, height, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            var frame = animation.CreateFrame();

            frame.DrawBox(new Rectangle(0, 0, width, height), new ColoredGlyph(frame.DefaultForeground, frame.DefaultBackground), null, ICellSurface.ConnectedLineThin);

            //frame.SetEffect(frame, _pulseAnimation);
            animation.Center = center;

            Brush.Animations[animation.Name] = animation;
            Brush.Animation = animation;
        }

        public void OnSelected()
        {
            Brush.IsVisible = true;

            ResetSelection();

            MainConsole.Instance.Brush = Brush;
            

            //if (_panel.State != SelectionToolPanel.CloneState.Clone && _panel.State != SelectionToolPanel.CloneState.Move)
            //{
            //    Brush.IsVisible = true;
            //    Brush.Animation = Brush.Animations["single"];

            //    MainScreen.Instance.Brush = Brush;
            //    MainScreen.Instance.UpdateBrush();

            //    _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
            //}
            //else
            //{
            //    MainScreen.Instance.Brush = Brush;
            //    MainScreen.Instance.UpdateBrush();
            //}
        }

        public void OnDeselected()
        {
            Brush.IsVisible = false;
            MainConsole.Instance.AllowKeyboardToMoveConsole = true;
        }

        public void RefreshTool()
        {
            Brush.Animation.IsDirty = true;
        }

        public void Update()
        {
            if (_panel.State != SelectionToolPanel.CloneState.SelectingPoint1)
            { }
        }

        public bool ProcessKeyboard(Keyboard info, Console surface)
        {
            return false;
        }

        private bool cancelled;
        private Point finalPostion;

        public void ProcessMouse(MouseScreenObjectState info, Console surface, bool isInBounds)
        {
            _previousSurface = surface;

            if (cancelled)
            {
                // wait until left button is released...
                if (info.Mouse.LeftButtonDown)
                    return;
                else
                    cancelled = false;
            }

            if (!isInBounds)
                return;

            if (_panel.State == SelectionToolPanel.CloneState.SelectingPoint1)
            {
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

                        secondPoint = info.SurfaceCellPosition;

                        // Draw the line (erase old) to where the mouse is
                        // create the animation frame
                        int width = Math.Max(firstPoint.Value.X, secondPoint.X) - Math.Min(firstPoint.Value.X, secondPoint.X) + 1;
                        int height = Math.Max(firstPoint.Value.Y, secondPoint.Y) - Math.Min(firstPoint.Value.Y, secondPoint.Y) + 1;

                        Point p1;

                        if (firstPoint.Value.X > secondPoint.X)
                        {
                            if (firstPoint.Value.Y > secondPoint.Y)
                                p1 = (0, 0);
                            else
                                p1 = new Point(0, height - 1);
                        }
                        else
                        {
                            if (firstPoint.Value.Y > secondPoint.Y)
                                p1 = new Point(width - 1, 0);
                            else
                                p1 = new Point(width - 1, height - 1);
                        }

                        finalPostion = info.WorldCellPosition;
                        MakeBoxAnimation(width, height, p1);
                    }
                }
                else if (firstPoint.HasValue)
                {
                    // We let go outside of bounds
                    if (!isInBounds)
                    {
                        cancelled = true;
                        Brush.ShowSelectedSurface = false;
                        Brush.IsVisible = false;
                        Brush.Animation = Brush.Animations[AnimationSingle];
                        return;
                    }

                    if (info.ScreenObject.Surface.IsScrollable)
                    {
                        secondPoint = info.SurfaceCellPosition + info.ScreenObject.Surface.ViewPosition;
                        firstPoint = firstPoint.Value + info.ScreenObject.Surface.ViewPosition;
                    }
                    else
                    {
                        secondPoint = info.SurfaceCellPosition;
                        firstPoint = firstPoint.Value;
                    }
                    

                    // Copy data to new animation
                    AnimatedScreenSurface cloneAnimation = new AnimatedScreenSurface("clone", Brush.Animation.Width, Brush.Animation.Height, SadConsoleEditor.Config.Program.ScreenFont, Config.Program.ScreenFontSize);
                    var frame = cloneAnimation.CreateFrame();
                    Point topLeftPoint = new Point(Math.Min(firstPoint.Value.X, secondPoint.X), Math.Min(firstPoint.Value.Y, secondPoint.Y));
                    surface.Copy(topLeftPoint.X, topLeftPoint.Y, cloneAnimation.Width, cloneAnimation.Height, frame, 0, 0);

                    if (_altPanel.SkipEmptyCells && _altPanel.UseAltEmptyColor)
                    {
                        foreach (var cell in frame.Cells)
                        {
                            if (cell.Glyph == 0 && cell.Background == _altPanel.AltEmptyColor)
                                cell.Background = Color.Transparent;
                        }
                    }

                    cloneAnimation.Center = Brush.Animation.Center;

                    Brush.SelectedSurface.Animation = cloneAnimation;

                    Brush.Position = finalPostion;

                    _panel.State = SelectionToolPanel.CloneState.Selected;
                }
            }

            if (info.Mouse.RightClicked)
            {
                _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
                firstPoint = null;
            }

            if (_panel.State == SelectionToolPanel.CloneState.Selected)
            {
                Brush.Position = finalPostion;
                Brush.IsVisible = true;

                if (info.Mouse.LeftButtonDown && isInBounds)
                {
                    _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
                    firstPoint = null;
                }

            }
            if (info.Mouse.LeftClicked && isInBounds)
            {
                if (_panel.State == SelectionToolPanel.CloneState.Stamp)
                {
                    StampBrush(info.SurfaceCellPosition.X, info.SurfaceCellPosition.Y, surface);
                }
                else if (_panel.State == SelectionToolPanel.CloneState.Move)
                {
                    StampBrush(info.SurfaceCellPosition.X, info.SurfaceCellPosition.Y, surface);
                    _panel.State = SelectionToolPanel.CloneState.SelectingPoint1;
                }
            }
            
        }
        
        bool isMouseOver = false;
        
        private void StampBrush(int consoleLocationX, int consoleLocationY, ICellSurface surface)
        {
            int destinationX = consoleLocationX - Brush.Animation.Center.X;
            int destinationY = consoleLocationY - Brush.Animation.Center.Y;
            int destX = destinationX;
            int destY = destinationY;

            for (int curx = 0; curx < Brush.SelectedSurface.Animation.Width; curx++)
            {
                for (int cury = 0; cury < Brush.SelectedSurface.Animation.Height; cury++)
                {
                    if (Brush.SelectedSurface.Animation.CurrentFrame.IsValidCell(curx, cury))
                    {
                        var sourceCell = Brush.SelectedSurface.Animation.CurrentFrame[curx, cury];

                        // Not working, breakpoint here to remind me.
                        if (_altPanel.SkipEmptyCells && sourceCell.Glyph == 0 && (sourceCell.Background == Color.Transparent || (_altPanel.UseAltEmptyColor && sourceCell.Background == _altPanel.AltEmptyColor)))
                        {
                            destY++;
                            continue;
                        }

                        if (surface.IsScrollable)
                        {
                            if (surface.IsValidCell(destX + surface.ViewPosition.X, destY + surface.ViewPosition.Y))
                            {
                                var desCell = surface[destX + surface.ViewPosition.X, destY + surface.ViewPosition.Y];
                                sourceCell.CopyAppearanceTo(desCell);
                            }
                        }
                        else
                        {
                            if (surface.IsValidCell(destX, destY))
                            {
                                var desCell = surface[destX, destY];
                                sourceCell.CopyAppearanceTo(desCell);
                            }
                        }

                        
                    }
                    destY++;
                }
                destY = destinationY;
                destX++;
            }

            surface.IsDirty = true;
        }

        private void ClearBrush(int consoleLocationX, int consoleLocationY, ICellSurface surface)
        {
            int destYorg = consoleLocationY;
            int destX = consoleLocationX;
            int destY = destYorg;

            for (int curx = 0; curx < Brush.SelectedSurface.Animation.Width; curx++)
            {
                for (int cury = 0; cury < Brush.SelectedSurface.Animation.Height; cury++)
                {
                    surface.Clear(destX, destY);

                    destY++;
                }
                destY = destYorg;
                destX++;
            }

            surface.IsDirty = true;
        }
    }
}
