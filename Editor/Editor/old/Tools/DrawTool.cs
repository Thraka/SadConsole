namespace SadConsoleEditor.Tools
{
    using SadConsole;
    using SadConsole.Input;
    using Panels;
    using SadConsole.Surfaces;
    class PaintTool: ITool
    {
        public const string ID = "PENCIL";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Pencil"; }
        }
        public char Hotkey { get { return 'p'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        //private EntityBrush _brush;
        public SadConsole.GameHelpers.Entity Brush;

        public PaintTool()
        {
            ControlPanels = new CustomPanel[] { CharacterPickPanel.SharedInstance };
        }

        public override string ToString()
        {
            return Title;
        }

        public void OnSelected()
        {
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            Brush.Animation.CreateFrame();
            Brush.IsVisible = false;
            RefreshTool();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = true;
        }
        
        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            MainScreen.Instance.QuickSelectPane.IsVisible = false;
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            SadConsoleEditor.Settings.QuickEditor.TextSurface = Brush.Animation.Frames[0];
            SadConsoleEditor.Settings.QuickEditor.Fill(CharacterPickPanel.SharedInstance.SettingForeground,
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
            if (info.IsOnConsole)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var cell = info.Cell;
                    cell.Glyph = CharacterPickPanel.SharedInstance.SettingCharacter;
                    cell.Foreground = CharacterPickPanel.SharedInstance.SettingForeground;
                    cell.Background = CharacterPickPanel.SharedInstance.SettingBackground;
                    cell.Mirror = CharacterPickPanel.SharedInstance.SettingMirrorEffect;
                    surface.IsDirty = true;
                }

                if (info.Mouse.RightButtonDown)
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
}
