namespace SadConsoleEditor.Tools
{
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Entities;
    using SadConsole.Input;
    using SadConsoleEditor.Panels;

    class RecolorTool : ITool
    {
        public const string ID = "RECOLOR";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Recolor"; }
        }
        public char Hotkey { get { return 'r'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        private RecolorToolPanel settingsPanel;

        public Entity Brush;

        public override string ToString()
        {
            return Title;
        }

        public RecolorTool()
        {
            settingsPanel = new RecolorToolPanel();

            ControlPanels = new CustomPanel[] { settingsPanel, CharacterPickPanel.SharedInstance };
        }

        public void OnSelected()
        {
            Brush = new SadConsole.GameHelpers.Entity(1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            Brush.Animation = new AnimatedSurface("default", 1, 1, SadConsoleEditor.Settings.Config.ScreenFont);
            Brush.Animation.CreateFrame();
            Brush.IsVisible = false;
            RefreshTool();
            MainScreen.Instance.Brush = Brush;

            MainScreen.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = true;
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = false;
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
                                      CharacterPickPanel.SharedInstance.SettingBackground, 42);

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

                    if (!settingsPanel.IgnoreForeground)
                        cell.Foreground = CharacterPickPanel.SharedInstance.SettingForeground;

                    if (!settingsPanel.IgnoreBackground)
                        cell.Background = CharacterPickPanel.SharedInstance.SettingBackground;

                    info.Console.TextSurface.IsDirty = true;
                }
                else if (info.Mouse.RightButtonDown)
                {
                    var cell = info.Cell;

                    if (!settingsPanel.IgnoreForeground)
                        CharacterPickPanel.SharedInstance.SettingForeground = cell.Foreground;

                    if (!settingsPanel.IgnoreBackground)
                        CharacterPickPanel.SharedInstance.SettingBackground = cell.Background;
                }
            }
        }
    }
}
