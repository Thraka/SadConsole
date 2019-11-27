#if XNA
using Microsoft.Xna.Framework;
#endif

using SadConsole.Controls;
using System.Reflection;
using System.Linq;
using SadConsole.Themes;
using System;
using SadConsole.Input;

namespace SadConsole.Debug
{
    public static class CurrentScreen
    {
        static CurrentScreen()
        {
            SadConsole.Themes.Library.Default.SetControlTheme(typeof(ScrollingSurfaceView), new ScrollingSurfaceView.ScrollingSurfaceViewTheme());
        }

        private class DebugWindow : Window
        {
            private Console _originalScreen;
            private ContainerConsole _wrapperScreen;
            private readonly ConsoleStack _inputConsoles;

            private readonly Label _labelConsoleTitle;
            private readonly Label _labelConsoleWidth;
            private readonly Label _labelConsoleHeight;
            private readonly Label _labelConsolePosition;
            private readonly Button _buttonSetPosition;
            private readonly ListBox _listConsoles;
            private readonly CheckBox _checkIsVisible;
            private readonly ScrollingSurfaceView _surfaceView;

            private bool _isReadingConsole;

            public DebugWindow(Font font) : base(78, 22, font)
            {
                var listboxTheme = (ListBoxTheme)Library.Default.GetControlTheme(typeof(ListBox));
                listboxTheme.DrawBorder = true;

                Title = "Global.CurrentScreen Debugger";
                IsModalDefault = true;
                CloseOnEscKey = true;

                _listConsoles = new ListBox(30, 15) { Position = new Point(2, 3) };
                _listConsoles.SelectedItemChanged += Listbox_SelectedItemChanged;
                _listConsoles.Theme = listboxTheme;
                Add(_listConsoles);

                Label label = CreateLabel("Current Screen", new Point(_listConsoles.Bounds.Left, _listConsoles.Bounds.Top - 1));
                label = CreateLabel("Selected Console: ", new Point(_listConsoles.Bounds.Right + 1, label.Bounds.Top));
                {
                    _labelConsoleTitle = new Label(Width - label.Bounds.Right - 1) { Position = new Point(label.Bounds.Right, label.Bounds.Top) };
                    Add(_labelConsoleTitle);
                }

                label = CreateLabel("Width: ", new Point(label.Bounds.Left, label.Bounds.Bottom));
                {
                    _labelConsoleWidth = new Label(5) { Position = new Point(label.Bounds.Right, label.Bounds.Top) };
                    _labelConsoleWidth.Alignment = HorizontalAlignment.Right;
                    Add(_labelConsoleWidth);
                }

                label = CreateLabel("Height:", new Point(label.Bounds.Left, label.Bounds.Bottom));
                {
                    _labelConsoleHeight = new Label(5) { Position = new Point(label.Bounds.Right, label.Bounds.Top) };
                    _labelConsoleHeight.Alignment = HorizontalAlignment.Right;
                    Add(_labelConsoleHeight);
                }

                _checkIsVisible = new CheckBox(15, 1)
                {
                    Text = "Is Visible",
                    Position = new Point(_listConsoles.Bounds.Right + 1, label.Bounds.Bottom)
                };
                _checkIsVisible.IsSelectedChanged += _checkIsVisible_IsSelectedChanged;
                Add(_checkIsVisible);

                label = CreateLabel("Position: ", new Point(_labelConsoleWidth.Bounds.Right + 2, _labelConsoleWidth.Bounds.Top));
                {
                    _labelConsolePosition = new Label(7) { Position = new Point(label.Bounds.Right, label.Bounds.Top) };
                    Add(_labelConsolePosition);
                }

                _buttonSetPosition = new Button(5)
                {
                    Text = "Set",
                    Position = new Point(_labelConsolePosition.Bounds.Right + 1, _labelConsolePosition.Bounds.Top),
                };
                Add(_buttonSetPosition);
                _buttonSetPosition.Click += _buttonSetPosition_Click;

                _surfaceView = new ScrollingSurfaceView(Width - 3 - _listConsoles.Bounds.Right + 1, Height - 6 - _checkIsVisible.Bounds.Bottom + 1)
                {
                    Theme = new ScrollingSurfaceView.ScrollingSurfaceViewTheme(),
                    Position = new Point(_listConsoles.Bounds.Right + 1, _checkIsVisible.Bounds.Bottom)
                };
                Add(_surfaceView);

                // Position EDIT
                // Map View of surface



                // Cell peek of surface
                // Cell edit of surface
                // Save surface

                //label = CreateLabel("Visible: ", new Point(listbox.Bounds.Right + 1, _labelConsoleWidth.Bounds.Top));
                //{
                //    _labelConsoleHeight = new Label(5) { Position = new Point(label.Bounds.Right, label.Bounds.Top) };
                //    Add(_labelConsoleHeight);
                //}

                Label CreateLabel(string text, Point position)
                {
                    var labelTemp = new Label(text) { Position = position, TextColor = (ThemeColors?.TitleText ?? Library.Default.Colors.TitleText) };
                    Add(labelTemp);
                    return labelTemp;
                }


                // Setup the "steal the focus" system to pause the existing current screen
                _originalScreen = Global.CurrentScreen;
                _wrapperScreen = new ContainerConsole();
                _inputConsoles = Global.FocusedConsoles;
                Global.FocusedConsoles = new ConsoleStack();

                AddConsoleToList(_originalScreen);
                void AddConsoleToList(Console console, int indent = 0)
                {
                    System.Diagnostics.DebuggerDisplayAttribute debugger = console.GetType().GetTypeInfo().GetCustomAttributes<System.Diagnostics.DebuggerDisplayAttribute>().FirstOrDefault();
                    string text = debugger != null ? debugger.Value : console.ToString();

                    _listConsoles.Items.Add(new ConsoleListboxItem(new string('-', indent) + text, console));

                    foreach (Console child in console.Children)
                    {
                        AddConsoleToList(child, indent + 1);
                    }
                }

                _wrapperScreen.Children.Add(_originalScreen);
                _wrapperScreen.IsPaused = true;
                Global.CurrentScreen = new ContainerConsole();
                Global.CurrentScreen.Children.Add(_wrapperScreen);

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
                    Position = new Point(label.Bounds.Right + 3, label.Bounds.Top),
                    IsNumeric = true,
                    AllowDecimal = false,
                    Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X.ToString()
                };
                window.Add(widthBox);

                label = CreateLabel("Y: ", new Point(1, 3));
                heightBox = new TextBox(4)
                {
                    Position = new Point(label.Bounds.Right + 3, label.Bounds.Top),
                    IsNumeric = true,
                    AllowDecimal = false,
                    Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y.ToString()
                };
                window.Add(heightBox);

                var buttonSave = new Button(4, 1)
                {
                    Text = "Save",
                    Position = new Point(1, window.Height - 2),
                    Theme = new Themes.ButtonTheme() { ShowEnds = false }
                };
                buttonSave.Click += (s, e2) =>
                {
                    ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position = new Point(int.Parse(widthBox.Text), int.Parse(heightBox.Text));
                    _labelConsolePosition.DisplayText = $"{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X},{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y}";
                    window.Hide();
                };
                window.Add(buttonSave);

                var buttonCancel = new Button(6, 1)
                {
                    Text = "Cancel",
                    Position = new Point(window.Width - 1 - 6, window.Height - 2),
                    Theme = new Themes.ButtonTheme() { ShowEnds = false }
                };
                buttonCancel.Click += (s, e2) =>
                {
                    window.Hide();
                };
                window.Add(buttonCancel);

                window.Center();
                window.Show(true);

                Label CreateLabel(string text, Point position)
                {
                    var labelTemp = new Label(text) { Position = position, TextColor = ThemeColors.TitleText };
                    window.Add(labelTemp);
                    return labelTemp;
                }
            }

            private void SetEditCell(Cell cell)
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
                _labelConsoleWidth.DisplayText = item.Console.Width.ToString();
                _labelConsoleHeight.DisplayText = item.Console.Height.ToString();
                _labelConsolePosition.DisplayText = $"{item.Console.Position.X},{item.Console.Position.Y}";
                _checkIsVisible.IsSelected = item.Console.IsVisible;
                _isReadingConsole = false;
                _surfaceView.SetTargetSurface(item.Console);
            }

            private bool _cellTrackOnControl;

            public override bool ProcessMouse(MouseConsoleState state)
            {
                if (state.IsOnConsole)
                {
                    Rectangle mouseArea = _surfaceView.MouseArea;
                    mouseArea.Offset(_surfaceView.Position);
                    if (mouseArea.Contains(state.ConsoleCellPosition))
                    {
                        _cellTrackOnControl = true;
                        Point pos = state.ConsoleCellPosition - _surfaceView.Position;
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
                Global.CurrentScreen = _originalScreen;
                _originalScreen = null;
                _wrapperScreen = null;
                Global.FocusedConsoles = _inputConsoles;
            }
        }

        public static void Show() => Show(Global.FontDefault);

        public static void Show(Font font)
        {
            DebugWindow window = new DebugWindow(font);
            window.Show();
            window.Center();
        }

        private class ConsoleListboxItem
        {
            private readonly string Title;
            public Console Console;

            public ConsoleListboxItem(string title, Console console)
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

            protected CellSurface SurfaceReference;
            protected ScrollingConsole SurfaceView;

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

                SurfaceView.ViewPort = new Rectangle(SurfaceView.ViewPort.X, ((ScrollBar)sender).Value, SurfaceView.ViewPort.Width, SurfaceView.ViewPort.Height);
                IsDirty = true;
            }

            private void HorizontalBar_ValueChanged(object sender, EventArgs e)
            {
                if (!((ScrollBar)sender).IsEnabled)
                {
                    return;
                }

                SurfaceView.ViewPort = new Rectangle(((ScrollBar)sender).Value, SurfaceView.ViewPort.Y, SurfaceView.ViewPort.Width, SurfaceView.ViewPort.Height);
                IsDirty = true;
            }

            protected override void OnParentChanged()
            {
                VerticalBar.Parent = parent;
                HorizontalBar.Parent = parent;
            }

            protected override void OnPositionChanged()
            {
                VerticalBarX = Width - 1;
                HorizontalBarY = Height - 1;
                VerticalBar.Position = Position + new Point(VerticalBarX, 0);
                HorizontalBar.Position = Position + new Point(0, HorizontalBarY);
            }

            public void SetTargetSurface(Console surface)
            {
                SurfaceReference = null;
                SurfaceView = null;

                SurfaceReference = surface;
                SurfaceView = new ScrollingConsole(surface.Width, surface.Height, surface.Font, new Rectangle(0, 0, Width - 2 > surface.Width ? surface.Width : Width - 2,
                                                                                                                    Height - 2 > surface.Height ? surface.Height : Height - 2), surface.Cells)
                {
                    DefaultBackground = surface.DefaultBackground
                };

                if (SurfaceView.ViewPort.Width != SurfaceView.Width)
                {
                    HorizontalBar.IsEnabled = true;
                    HorizontalBar.Maximum = SurfaceView.Width - SurfaceView.ViewPort.Width;
                }
                else
                {
                    HorizontalBar.IsEnabled = false;
                }

                if (SurfaceView.ViewPort.Height != SurfaceView.Height)
                {
                    VerticalBar.IsEnabled = true;
                    VerticalBar.Maximum = SurfaceView.Height - SurfaceView.ViewPort.Height;
                }
                else
                {
                    VerticalBar.IsEnabled = false;
                }

                VerticalBar.Value = 0;
                HorizontalBar.Value = 0;

                IsDirty = true;
            }

            public override bool ProcessMouse(MouseConsoleState state)
            {
                if (isEnabled)
                {
                    if (isMouseOver)
                    {
                        Point mouseControlPosition = TransformConsolePositionByControlPosition(state.CellPosition);

                        if (mouseControlPosition.X == VerticalBarX)
                        {
                            VerticalBar.ProcessMouse(state);
                        }

                        if (mouseControlPosition.Y == HorizontalBarY)
                        {
                            HorizontalBar.ProcessMouse(state);
                        }
                    }
                    else
                    {
                        base.ProcessMouse(state);
                    }
                }

                return false;
            }

            public class ScrollingSurfaceViewTheme : Themes.ThemeBase
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

                    RefreshTheme(control.ThemeColors, control);

                    Cell appearance = GetStateAppearance(scroller.State);

                    scroller.Surface.DefaultBackground = scroller.SurfaceView.DefaultBackground;
                    scroller.Surface.Clear();
                    scroller.Surface.DrawBox(new Rectangle(0, 0, scroller.Surface.Width, scroller.Surface.Height), appearance, null, CellSurface.ConnectedLineThin);
                    scroller.SurfaceView.Copy(scroller.SurfaceView.ViewPort, scroller.Surface, 1, 1);

                    if (scroller.SurfaceReference is ControlsConsole controlsConsole)
                    {
                        foreach (ControlBase childControl in controlsConsole.Controls)
                        {
                            for (int i = 0; i < childControl.Surface.Cells.Length; i++)
                            {
                                ref Cell cell = ref childControl.Surface.Cells[i];

                                if (!cell.IsVisible)
                                {
                                    continue;
                                }

                                Point cellRenderPosition = i.ToPoint(childControl.Surface.Width) + childControl.Position;

                                if (!scroller.SurfaceView.ViewPort.Contains(cellRenderPosition - new Point(1)))
                                {
                                    continue;
                                }

                                cell.CopyAppearanceTo(scroller.Surface[(cellRenderPosition - scroller.SurfaceView.ViewPort.Location).ToIndex(scroller.Surface.Width)]);
                            }
                        }
                    }

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

                public override ThemeBase Clone() => new ScrollingSurfaceViewTheme()
                {
                    Normal = Normal.Clone(),
                    Disabled = Disabled.Clone(),
                    MouseOver = MouseOver.Clone(),
                    MouseDown = MouseDown.Clone(),
                    Selected = Selected.Clone(),
                    Focused = Focused.Clone(),
                };

                public override void RefreshTheme(Colors themeColors, ControlBase control)
                {
                    if (themeColors == null) themeColors = Library.Default.Colors;

                    base.RefreshTheme(themeColors, control);

                    SetForeground(Normal.Foreground);
                    SetBackground(Normal.Background);

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
