#nullable disable
#if DEBUG
using System;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

using SadRogue.Primitives;

namespace SadConsole.Debug;

/// <summary>
/// A debugging screen that takes the place of the active <see cref="GameHost.Screen"/> and displays information about SadConsole.
/// </summary>
public static class Screen
{
    /// <summary>
    /// Displays the debugger.
    /// </summary>
    /// <param name="font">The font to use the debugging screen.</param>
    /// <param name="fontSize">The size of the font.</param>
    public static void Show(IFont font, Point fontSize)
    {
        DebugWindow window = new DebugWindow(font, fontSize);
        window.Show();
        window.Center();
    }

    /// <summary>
    /// Shows the debug screen with the default font and size.
    /// </summary>
    public static void Show() =>
        Show(GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(IFont.Sizes.One));

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
        private readonly SurfaceViewer _surfaceView;

        private bool _isReadingConsole;

        public DebugWindow(IFont font, Point fontSize) : base(78, 22)
        {
            Font = font;
            FontSize = fontSize;

            Title = "Global.CurrentScreen Debugger";
            IsModalDefault = true;
            CloseOnEscKey = true;

            _listConsoles = new ListBox(30, 15) { Position = new Point(2, 3) };
            _listConsoles.SelectedItemChanged += Listbox_SelectedItemChanged;
            _listConsoles.DrawBorder = true;
            Controls.Add(_listConsoles);

            Label label = CreateLabel("Current Screen", new Point(_listConsoles.Bounds.X, _listConsoles.Bounds.Y - 1));
            label = CreateLabel("Selected Console: ", new Point(_listConsoles.Bounds.MaxExtentX + 2, label.Bounds.Y));
            {
                _labelConsoleTitle = new Label(Width - label.Bounds.MaxExtentX - 2) { Position = new Point(label.Bounds.MaxExtentX + 1, label.Bounds.Y) };
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

            _surfaceView = new SurfaceViewer(Width - 3 - _listConsoles.Bounds.MaxExtentX + 1, Height - 6 - _checkIsVisible.Bounds.MaxExtentY + 2)
            {
                Position = new Point(_listConsoles.Bounds.MaxExtentX + 2, _checkIsVisible.Bounds.MaxExtentY + 1),

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
                _listConsoles.Items.Add(new ConsoleListboxItem(new string('-', indent) + console.ToString(), console));

                for (int i = 0; i < console.Children.Count; i++)
                    AddConsoleToList(console.Children[i], indent + 1);
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
            NumberBox widthBox;
            NumberBox heightBox;

            Label label = CreateLabel("X: ", new Point(1, 2));
            widthBox = new NumberBox(4)
            {
                Position = new Point(label.Bounds.MaxExtentX + 4, label.Bounds.Y),
                Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X.ToString()
            };
            window.Controls.Add(widthBox);

            label = CreateLabel("Y: ", new Point(1, 3));
            heightBox = new NumberBox(4)
            {
                Position = new Point(label.Bounds.MaxExtentX + 4, label.Bounds.Y),
                Text = ((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y.ToString()
            };
            window.Controls.Add(heightBox);

            var buttonSave = new Button(4, 1)
            {
                Text = "Save",
                Position = new Point(1, window.Height - 2),
                ShowEnds = false
            };
            buttonSave.Click += (s, e2) =>
            {
                //((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position = new Point(int.Parse(widthBox.Text), int.Parse(heightBox.Text));
                _labelConsolePosition.DisplayText = $"{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.X},{((ConsoleListboxItem)_listConsoles.SelectedItem).Console.Position.Y}";
                window.Hide();
            };
            window.Controls.Add(buttonSave);

            var buttonCancel = new Button(6, 1)
            {
                Text = "Cancel",
                Position = new Point(window.Width - 1 - 6, window.Height - 2),
                ShowEnds = false
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
                var labelTemp = new Label(text) { Position = position, TextColor = Controls.GetThemeColors().Title };
                window.Controls.Add(labelTemp);
                return labelTemp;
            }
        }

        private void SetEditCell(ColoredGlyphBase cell)
        {
            if (cell == null)
            {
                return;
            }

            cell.CopyAppearanceTo(Surface[Width - 3, Height - 3]);

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
                _surfaceView.Surface = surface.Surface;
            }
            else
                _surfaceView.ResetSurface();
        }

        private bool _cellTrackOnControl;

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (state.IsOnScreenObject)
            {
                Rectangle mouseArea = _surfaceView.MouseArea;
                mouseArea = mouseArea.Translate(_surfaceView.Position);
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
}
#endif
