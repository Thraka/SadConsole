using SadConsole;
using SadConsole.Input;
using SadConsoleEditor.Panels;
using SadConsoleEditor.Tools;

namespace SadConsoleEditor.Tools
{
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
        public SadConsole.Entities.Entity Brush;

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
            Brush.Animation.CurrentFrame.Fill(CharacterPickPanel.SharedInstance.SettingForeground,
                                 CharacterPickPanel.SharedInstance.SettingBackground,
                                 CharacterPickPanel.SharedInstance.SettingCharacter,
                                 CharacterPickPanel.SharedInstance.SettingMirrorEffect);
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
            if (info.IsOnScreenObject)
            {
                if (info.Mouse.LeftButtonDown)
                {
                    var cell = info.Cell;
                    cell.Glyph = CharacterPickPanel.SharedInstance.SettingCharacter;
                    cell.Foreground = CharacterPickPanel.SharedInstance.SettingForeground;
                    cell.Background = CharacterPickPanel.SharedInstance.SettingBackground;
                    cell.Mirror = CharacterPickPanel.SharedInstance.SettingMirrorEffect;
                    screenObject.Surface.IsDirty = true;
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
