namespace SadConsoleEditor.Tools
{
    using SadRogue.Primitives;
    using SadConsole;
    using SadConsole.Input;
    using Console = SadConsole.Console;
    using System;
    using SadConsoleEditor.Panels;
    using SadConsole.Entities;

    class FillTool : ITool
    {
        public const string ID = "FILL";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Fill"; }
        }
        public char Hotkey { get { return 'f'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        public Entity Brush;

        public FillTool()
        {
            ControlPanels = new CustomPanel[] { Panels.CharacterPickPanel.SharedInstance };
        }

        public override string ToString()
        {
            return Title;
        }

        public void OnSelected()
        {
            Brush = new SadConsole.Entities.Entity(1, 1, Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            Brush.IsVisible = false;
            RefreshTool();
            MainConsole.Instance.Brush = Brush;

            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainConsole.Instance.QuickSelectPane.IsVisible = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainConsole.Instance.QuickSelectPane.IsVisible = false;
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            Brush.Animation.Surface.Fill(CharacterPickPanel.SharedInstance.SettingForeground,
                                 CharacterPickPanel.SharedInstance.SettingBackground,
                                 CharacterPickPanel.SharedInstance.SettingCharacter,
                                 CharacterPickPanel.SharedInstance.SettingMirrorEffect);

            Brush.Animation.IsDirty = true;
        }

        public void Update()
        {
        }

        public bool ProcessKeyboard(Keyboard info, IScreenSurface screenObject)
        {
            return false;
        }

        public void ProcessMouse(MouseScreenObjectState info, IScreenSurface screenObject, bool isInBounds)
        {
            if (info.Mouse.LeftClicked && info.IsOnScreenObject)
            {
                ColoredGlyph cellToMatch = new ColoredGlyph();
                ColoredGlyph currentFillCell = new ColoredGlyph();

                info.Cell.CopyAppearanceTo(cellToMatch);

                currentFillCell.Glyph = CharacterPickPanel.SharedInstance.SettingCharacter;
                currentFillCell.Foreground = CharacterPickPanel.SharedInstance.SettingForeground;
                currentFillCell.Background = CharacterPickPanel.SharedInstance.SettingBackground;
                currentFillCell.Mirror = CharacterPickPanel.SharedInstance.SettingMirrorEffect;
                
                Func<ColoredGlyph, bool> isTargetCell = (c) =>
                {
                    if (c.Glyph == 0 && cellToMatch.Glyph == 0)
                        return c.Background == cellToMatch.Background;

                    return c.Foreground == cellToMatch.Foreground &&
                           c.Background == cellToMatch.Background &&
                           c.Glyph == cellToMatch.Glyph &&
                           c.Mirror == cellToMatch.Mirror;
                };

                Action<ColoredGlyph> fillCell = (c) =>
                {
                    currentFillCell.CopyAppearanceTo(c);
                    //console.TextSurface.SetEffect(c, _currentFillCell.Effect);
                };

                System.Collections.Generic.List<ColoredGlyph> cells = new System.Collections.Generic.List<ColoredGlyph>(screenObject.Surface.Cells);

                Func<ColoredGlyph, SadConsole.Algorithms.NodeConnections<ColoredGlyph>> getConnectedCells = (c) =>
                {
                    Algorithms.NodeConnections<ColoredGlyph> connections = new Algorithms.NodeConnections<ColoredGlyph>();

                    Point position = Point.FromIndex(cells.IndexOf(c), screenObject.Surface.BufferWidth);

                    connections.West = screenObject.Surface.IsValidCell(position.X - 1, position.Y) ? screenObject.Surface[position.X - 1, position.Y] : null;
                    connections.East = screenObject.Surface.IsValidCell(position.X + 1, position.Y) ? screenObject.Surface[position.X + 1, position.Y] : null;
                    connections.North = screenObject.Surface.IsValidCell(position.X, position.Y - 1) ? screenObject.Surface[position.X, position.Y - 1] : null;
                    connections.South = screenObject.Surface.IsValidCell(position.X, position.Y + 1) ? screenObject.Surface[position.X, position.Y + 1] : null;

                    return connections;
                };

                if (!isTargetCell(currentFillCell))
                    SadConsole.Algorithms.FloodFill<ColoredGlyph>(info.Cell, isTargetCell, fillCell, getConnectedCells);

                ((IScreenSurface)info.ScreenObject).IsDirty = true;
            }

            if (info.Mouse.RightButtonDown && info.IsOnScreenObject)
            {
                var cell = info.Cell;

                CharacterPickPanel.SharedInstance.SettingCharacter = cell.Glyph;
                CharacterPickPanel.SharedInstance.SettingForeground = cell.Foreground;
                CharacterPickPanel.SharedInstance.SettingBackground = cell.Background;
                CharacterPickPanel.SharedInstance.SettingMirrorEffect = cell.Mirror;
            }
        }

    }
}
