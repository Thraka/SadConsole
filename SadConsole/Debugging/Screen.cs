#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadConsole.Debug
{
    /// <summary>
    /// A debugging screen that takes the place of the active <see cref="GameHost.Screen"/> and displays information about SadConsole.
    /// </summary>
    public static class Screen
    {
        static Screen()
        {
            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(ScrollingSurfaceView), new ScrollingSurfaceView.ScrollingSurfaceViewTheme());
        }

        /// <summary>
        /// Displays the debugger.
        /// </summary>
        /// <param name="font">The font to use the debugging screen.</param>
        /// <param name="fontSize">The size of the font.</param>
        public static void Show(Font font, Point fontSize)
        {
            DebugWindow window = new DebugWindow(font);
            window.Show();
            window.Center();
        }

        public static void Show() =>
            Show(GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(Font.Sizes.One));

        private class DebugWindow : Window
        {
            private IScreenObject _originalScreen;
            private IScreenObject _wrapperScreen;
            private readonly FocusedScreenObjectStack _inputConsoles;

            private readonly Label _labelConsoleTitle;
            private readonly Label _labelConsoleWidth;
            private readonly Label _labelConsoleHeight;
            private readonly Label _labelConsolePosition;
            private readonly Button _buttonSetPosition;
            private readonly ListBox _listConsoles;
            private readonly CheckBox _checkIsVisible;
            private readonly ScrollingSurfaceView _surfaceView;

            private bool _isReadingConsole;

            public DebugWindow(Font font) : base(78, 22)
            {
                Font = font;
                FontSize = Font.GetFontSize(Font.Sizes.One);

                var listboxTheme = (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
                listboxTheme.DrawBorder = true;

                Title = "Global.CurrentScreen Debugger";
                IsModalDefault = true;
                CloseOnEscKey = true;

                _listConsoles = new ListBox(30, 15) { Position = new Point(2, 3) };
                _listConsoles.SelectedItemChanged += Listbox_SelectedItemChanged;
                _listConsoles.Theme = listboxTheme;
                Controls.Add(_listConsoles);

                Label label = CreateLabel("Current Screen", new Point(_listConsoles.Bounds.X, _listConsoles.Bounds.Y - 1));
                label = CreateLabel("Selected Console: ", new Point(_listConsoles.Bounds.MaxExtentX + 2, label.Bounds.Y));
                {
                    _labelConsoleTitle = new Label(Width - label.Bounds.MaxExtentX) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
                    Controls.Add(_labelConsoleTitle);
                }

                label = CreateLabel("Width: ", new Point(label.Bounds.X, label.Bounds.MaxExtentY + 1));
                {
                    _labelConsoleWidth = new Label(5) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
                    _labelConsoleWidth.Alignment = HorizontalAlignment.Right;
                    Controls.Add(_labelConsoleWidth);
                }

                label = CreateLabel("Height:", new Point(label.Bounds.X, label.Bounds.MaxExtentY + 1));
                {
                    _labelConsoleHeight = new Label(5) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
                    _labelConsoleHeight.Alignment = HorizontalAlignment.Right;
                    Controls.Add(_labelConsoleHeight);
                }

                _checkIsVisible = new CheckBox(15, 1)
                {
                    Text = "Is Visible",
                    Position = new Point(_listConsoles.Bounds.MaxExtentX + 2, label.Bounds.MaxExtentY + 1)
                };
                _checkIsVisible.IsSelectedChanged += _checkIsVisible_IsSelectedChanged;
                Controls.Add(_checkIsVisible);

                label = CreateLabel("Position: ", new Point(_labelConsoleWidth.Bounds.MaxExtentX + 3, _labelConsoleWidth.Bounds.Y));
                {
                    _labelConsolePosition = new Label(7) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
                    Controls.Add(_labelConsolePosition);
                }

                _buttonSetPosition = new Button(5)
                {
                    Text = "Set",
                    Position = new Point(_labelConsolePosition.Bounds.MaxExtentX + 2, _labelConsolePosition.Bounds.Y),
                };
                Controls.Add(_buttonSetPosition);
                _buttonSetPosition.Click += _buttonSetPosition_Click;

                _surfaceView = new ScrollingSurfaceView(Width - 3 - _listConsoles.Bounds.MaxExtentX + 2, Height - 6 - _checkIsVisible.Bounds.MaxExtentY + 2)
                {
                    Theme = new ScrollingSurfaceView.ScrollingSurfaceViewTheme(),
                    Position = new Point(_listConsoles.Bounds.MaxExtentX + 2, _checkIsVisible.Bounds.MaxExtentY + 1)
                };
                Controls.Add(_surfaceView);

                // Position EDIT
                // Map View of surface



                // Cell peek of surface
                // Cell edit of surface
                // Save surface

                //label = CreateLabel("Visible: ", new Point(listbox.Bounds.MaxExtentX + 2, _labelConsoleWidth.Bounds.Y));
                //{
                //    _labelConsoleHeight = new Label(5) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
                //    Add(_labelConsoleHeight);
                //}

                Label CreateLabel(string text, Point position)
                {
                    var labelTemp = new Label(text) { Position = position, TextColor = Controls.GetThemeColors().Title };
                    Controls.Add(labelTemp);
                    return labelTemp;
                }


                // Setup the "steal the focus" system to pause the existing current screen
                _originalScreen = SadConsole.GameHost.Instance.Screen;
                _wrapperScreen = new ScreenObject();
                _inputConsoles = SadConsole.GameHost.Instance.FocusedScreenObjects;
                SadConsole.GameHost.Instance.FocusedScreenObjects = new FocusedScreenObjectStack();

                AddConsoleToList(_originalScreen);
                void AddConsoleToList(IScreenObject console, int indent = 0)
                {
                    System.Diagnostics.DebuggerDisplayAttribute debugger = console.GetType().GetTypeInfo().GetCustomAttributes<System.Diagnostics.DebuggerDisplayAttribute>().FirstOrDefault();
                    string text = debugger != null ? debugger.Value : console.ToString();

                    _listConsoles.Items.Add(new ConsoleListboxItem(new string('-', indent) + text, console));

                    foreach (IScreenObject child in console.Children)
                    {
                        AddConsoleToList(child, indent + 1);
                    }
                }

                _wrapperScreen.Children.Add(_originalScreen);
                _wrapperScreen.IsEnabled = false;
                GameHost.Instance.Screen = new ScreenObject();
                GameHost.Instance.Screen.Children.Add(_wrapperScreen);

                _listConsoles.SelectedItem = _listConsoles.Items[0];
            }

            private void _buttonSetPosition_Click(object sender, EventArgs e)
            {
                var window = new Window(16, 7)
                {
                    Title = "Set Position",
                    CloseOnEscKey = true
                };
                TextBox widthBox;
                TextBox heightBox;

                Label label = CreateLabel("X: ", new Point(1, 2));
                widthBox = new TextBox(4)
                {
                    Position = new Point(label.Bounds.MaxExtentX + 4, label.Bounds.Y),
                    IsNumeric = true,
                    AllowDecimal = false,
                    Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X.ToString()
                };
                window.Controls.Add(widthBox);

                label = CreateLabel("Y: ", new Point(1, 3));
                heightBox = new TextBox(4)
                {
                    Position = new Point(label.Bounds.MaxExtentX + 4, label.Bounds.Y),
                    IsNumeric = true,
                    AllowDecimal = false,
                    Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y.ToString()
                };
                window.Controls.Add(heightBox);

                var buttonSave = new Button(4, 1)
                {
                    Text = "Save",
                    Position = new Point(1, window.Height - 2),
                    Theme = new UI.Themes.ButtonTheme() { ShowEnds = false }
                };
                buttonSave.Click += (s, e2) =>
                {
                    ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position = new Point(int.Parse(widthBox.Text), int.Parse(heightBox.Text));
                    _labelConsolePosition.DisplayText = $"{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X},{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y}";
                    window.Hide();
                };
                window.Controls.Add(buttonSave);

                var buttonCancel = new Button(6, 1)
                {
                    Text = "Cancel",
                    Position = new Point(window.Width - 1 - 6, window.Height - 2),
                    Theme = new ButtonTheme() { ShowEnds = false }
                };
                buttonCancel.Click += (s, e2) =>
                {
                    window.Hide();
                };
                window.Controls.Add(buttonCancel);

                window.Center();
                window.Show(true);

                Label CreateLabel(string text, Point position)
                {
                    var labelTemp = new Label(text) { Position = position, TextColor = Controls.GetThemeColors().Title};
                    window.Controls.Add(labelTemp);
                    return labelTemp;
                }
            }

            private void SetEditCell(ColoredGlyph cell)
            {
                if (cell == null)
                {
                    return;
                }

                cell.CopyAppearanceTo(this[Width - 3, Height - 3]);

                IsDirty = true;
            }

            private void _checkIsVisible_IsSelectedChanged(object sender, System.EventArgs e)
            {
                if (_isReadingConsole)
                {
                    return;
                } ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.IsVisible = ((CheckBox)sender).IsSelected;
            }

            private void Listbox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
            {
                var item = (ConsoleListboxItem)e.Item;
                _isReadingConsole = true;
                _labelConsoleTitle.DisplayText = item.ToString().Trim('-');
                _labelConsoleWidth.DisplayText = "";
                _labelConsoleHeight.DisplayText = "";
                _labelConsolePosition.DisplayText = $"{item.Console.Position.X},{item.Console.Position.Y}";
                _checkIsVisible.IsSelected = item.Console.IsVisible;
                _isReadingConsole = false;

                if (item.Console is IScreenSurface surface)
                {
                    _labelConsoleWidth.DisplayText = surface.Surface.Width.ToString();
                    _labelConsoleHeight.DisplayText = surface.Surface.Height.ToString();
                    _surfaceView.SetTargetSurface(surface);
                }
            }

            private bool _cellTrackOnControl;

            public override bool ProcessMouse(MouseScreenObjectState state)
            {
                if (state.IsOnScreenObject)
                {
                    Rectangle mouseArea = _surfaceView.MouseArea;
                    mouseArea  = mouseArea.Translate(_surfaceView.Position);
                    if (mouseArea.Contains(state.SurfaceCellPosition))
                    {
                        _cellTrackOnControl = true;
                        Point pos = state.SurfaceCellPosition - _surfaceView.Position;
                        if (_surfaceView.MouseArea.Contains(pos))
                        {
                            SetEditCell(_surfaceView.Surface[pos.X, pos.Y]);
                        }
                    }
                    else if (_cellTrackOnControl)
                    {
                        _cellTrackOnControl = false;
                        SetEditCell(null);
                    }
                }

                return base.ProcessMouse(state);
            }

            public override string ToString() => "Debug Window";

            public override void Hide()
            {
                base.Hide();
                _originalScreen.Parent = null;
                GameHost.Instance.Screen = _originalScreen;
                _originalScreen = null;
                _wrapperScreen = null;
                GameHost.Instance.FocusedScreenObjects = _inputConsoles;
            }
        }

        private class ConsoleListboxItem
        {
            private readonly string Title;
            public IScreenObject Console;

            public ConsoleListboxItem(string title, IScreenObject console)
            {
                Console = console;
                Title = title;
            }

            public override string ToString() => Title;
        }

        private class ScrollingSurfaceView : ControlBase
        {
            protected ScrollBar HorizontalBar;
            protected ScrollBar VerticalBar;

            protected ICellSurface SurfaceReference;
            protected ScreenSurface SurfaceView;

            protected int HorizontalBarY;
            protected int VerticalBarX;

            public Rectangle MouseArea;

            public ScrollingSurfaceView(int width, int height) : base(width, height)
            {
                HorizontalBar = new ScrollBar(Orientation.Horizontal, width - 1);
                VerticalBar = new ScrollBar(Orientation.Vertical, height - 1);

                HorizontalBar.Position = new Point(0, height - 1);
                VerticalBar.Position = new Point(width - 1, 0);

                HorizontalBar.ValueChanged += HorizontalBar_ValueChanged;
                VerticalBar.ValueChanged += VerticalBar_ValueChanged;

                HorizontalBar.IsEnabled = false;
                VerticalBar.IsEnabled = false;
                HorizontalBar.IsVisible = false;
                VerticalBar.IsVisible = false;

                MouseArea = new Rectangle(1, 1, width - 2, height - 2);
            }

            private void VerticalBar_ValueChanged(object sender, EventArgs e)
            {
                if (!((ScrollBar)sender).IsEnabled)
                {
                    return;
                }

                SurfaceView.Surface.View = SurfaceView.Surface.View.WithY(((ScrollBar)sender).Value);
                IsDirty = true;
            }

            private void HorizontalBar_ValueChanged(object sender, EventArgs e)
            {
                if (!((ScrollBar)sender).IsEnabled)
                {
                    return;
                }
                SurfaceView.Surface.View = SurfaceView.Surface.View.WithX(((ScrollBar)sender).Value);
                IsDirty = true;
            }

            protected override void OnParentChanged()
            {
                VerticalBar.Parent = Parent;
                HorizontalBar.Parent = Parent;
            }

            protected override void OnPositionChanged()
            {
                VerticalBarX = Width - 1;
                HorizontalBarY = Height - 1;
                VerticalBar.Position = Position + new Point(VerticalBarX, 0);
                HorizontalBar.Position = Position + new Point(0, HorizontalBarY);
            }

            public void SetTargetSurface(IScreenSurface surface)
            {
                SurfaceReference = null;
                SurfaceView = null;

                SurfaceReference = surface.Surface;
                SurfaceView = new ScreenSurface(surface.Surface, Width - 2, Height - 2);
                SurfaceView.Surface.DefaultBackground = surface.Surface.DefaultBackground;

                if (SurfaceView.Surface.ViewWidth != SurfaceView.Surface.Width)
                {
                    HorizontalBar.IsEnabled = true;
                    HorizontalBar.Maximum = SurfaceView.Surface.Width - SurfaceView.Surface.ViewWidth;
                }
                else
                    HorizontalBar.IsEnabled = false;

                if (SurfaceView.Surface.Height != SurfaceView.Surface.Height)
                {
                    VerticalBar.IsEnabled = true;
                    VerticalBar.Maximum = SurfaceView.Surface.Height - SurfaceView.Surface.ViewHeight;
                }
                else
                    VerticalBar.IsEnabled = false;

                VerticalBar.Value = 0;
                HorizontalBar.Value = 0;

                IsDirty = true;
            }

            public override bool ProcessMouse(MouseScreenObjectState state)
            {
                if (IsEnabled)
                {
                    if (MouseState_IsMouseOver)
                    {
                        var newState = new ControlMouseState(this, state);
                        Point mouseControlPosition = newState.MousePosition;

                        if (mouseControlPosition.X == VerticalBarX)
                            VerticalBar.ProcessMouse(state);

                        if (mouseControlPosition.Y == HorizontalBarY)
                            HorizontalBar.ProcessMouse(state);
                    }
                    else
                    {
                        base.ProcessMouse(state);
                    }
                }

                return false;
            }

            public class ScrollingSurfaceViewTheme : SadConsole.UI.Themes.ThemeBase
            {
                public override void Attached(ControlBase control)
                {
                    control.Surface = new CellSurface(control.Width, control.Height);

                    base.Attached(control);
                }

                public override void UpdateAndDraw(ControlBase control, TimeSpan time)
                {
                    if (!(control is ScrollingSurfaceView scroller))
                    {
                        return;
                    }

                    if (!scroller.IsDirty)
                    {
                        return;
                    }

                    if (scroller.SurfaceView == null)
                    {
                        return;
                    }

                    RefreshTheme(control.FindThemeColors(), control);

                    ColoredGlyph appearance = ControlThemeState.GetStateAppearance(scroller.State);

                    scroller.Surface.DefaultBackground = scroller.SurfaceView.Surface.DefaultBackground;
                    scroller.Surface.Clear();
                    scroller.Surface.DrawBox(new Rectangle(0, 0, scroller.Surface.Width, scroller.Surface.Height), appearance, null, ICellSurface.ConnectedLineThin);
                    scroller.SurfaceView.Surface.Copy(scroller.SurfaceView.Surface.View, scroller.Surface, 1, 1);

                    //if (scroller.SurfaceReference is ControlsConsole controlsConsole)
                    //{
                    //    foreach (ControlBase childControl in controlsConsole.Controls)
                    //    {
                    //        for (int i = 0; i < childControl.Surface.Cells.Length; i++)
                    //        {
                    //            ref Cell cell = ref childControl.Surface.Cells[i];

                    //            if (!cell.IsVisible)
                    //            {
                    //                continue;
                    //            }

                    //            Point cellRenderPosition = i.ToPoint(childControl.Surface.Width) + childControl.Position;

                    //            if (!scroller.SurfaceView.ViewPort.Contains(cellRenderPosition - new Point(1)))
                    //            {
                    //                continue;
                    //            }

                    //            cell.CopyAppearanceTo(scroller.Surface[(cellRenderPosition - scroller.SurfaceView.ViewPort.Location).ToIndex(scroller.Surface.Width)]);
                    //        }
                    //    }
                    //}

                    scroller.VerticalBar.IsDirty = true;
                    scroller.VerticalBar.Update(time);

                    scroller.HorizontalBar.IsDirty = true;
                    scroller.HorizontalBar.Update(time);

                    for (int y = 0; y < scroller.VerticalBar.Height; y++)
                    {
                        scroller.Surface.SetGlyph(scroller.VerticalBarX, y, scroller.VerticalBar.Surface[0, y].Glyph);
                        scroller.Surface.SetCellAppearance(scroller.VerticalBarX, y, scroller.VerticalBar.Surface[0, y]);
                    }

                    for (int x = 0; x < scroller.HorizontalBar.Width; x++)
                    {
                        scroller.Surface.SetGlyph(x, scroller.HorizontalBarY, scroller.HorizontalBar.Surface[x, 0].Glyph);
                        scroller.Surface.SetCellAppearance(x, scroller.HorizontalBarY, scroller.HorizontalBar.Surface[x, 0]);
                    }

                    scroller.IsDirty = false;
                }

                public override UI.Themes.ThemeBase Clone() =>
                    new ScrollingSurfaceViewTheme()
                    {
                        ControlThemeState = ControlThemeState
                    };

                public override void RefreshTheme(Colors themeColors, ControlBase control)
                {
                    if (themeColors == null) themeColors = UI.Themes.Library.Default.Colors;

                    base.RefreshTheme(themeColors, control);

                    ControlThemeState.SetForeground(ControlThemeState.Normal.Foreground);
                    ControlThemeState.SetBackground(ControlThemeState.Normal.Background);
                    var scroller = (ScrollingSurfaceView)control;

                    scroller.VerticalBar.Theme = new ScrollBarTheme();
                    scroller.HorizontalBar.Theme = new ScrollBarTheme();
                    scroller.VerticalBar.Theme.RefreshTheme(themeColors, control);
                    scroller.HorizontalBar.Theme.RefreshTheme(themeColors, control);
                }
            }
        }
    }
}
#endif
