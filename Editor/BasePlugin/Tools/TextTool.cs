namespace SadConsoleEditor.Tools
{
    using SadConsole;
    using SadConsole.Entities;
    using SadConsole.Input;
    using SadConsoleEditor.Panels;
    using SadRogue.Primitives;

    class TextTool : ITool
    {
        public const string ID = "TEXT";

        private bool writing;
        private Console tempConsole;
        private SadConsole.Effects.EffectsManager blinkManager;

        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Text"; }
        }
        public char Hotkey { get { return 't'; } }

        public CustomPanel[] ControlPanels { get; private set; }

        private RecolorToolPanel settingsPanel;

        public Entity Brush;

        public override string ToString()
        {
            return Title;
        }

        public TextTool()
        {
            settingsPanel = new RecolorToolPanel();

            ControlPanels = new CustomPanel[] { settingsPanel, CharacterPickPanel.SharedInstance };
            tempConsole = new Console(1, 1);
        }

        public void OnSelected()
        {
            Brush = new SadConsole.Entities.Entity(1, 1, Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            Brush.IsVisible = false;
            
            RefreshTool();
            MainConsole.Instance.Brush = Brush;

            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(CharacterPickPanel.SharedInstance, System.EventArgs.Empty);
            CharacterPickPanel.SharedInstance.Changed += CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = true;

            //blinkManager = new SadConsole.Effects.EffectsManager();
        }

        public void OnDeselected()
        {
            CharacterPickPanel.SharedInstance.Changed -= CharPanelChanged;
            CharacterPickPanel.SharedInstance.HideCharacter = false;
            if (writing)
            {
                MainConsole.Instance.DisableBrush = false;
                writing = false;
                tempConsole.Cursor.IsVisible = false;
                Brush.IsVisible = true;
            }
        }

        private void CharPanelChanged(object sender, System.EventArgs e)
        {
            MainConsole.Instance.QuickSelectPane.CommonCharacterPickerPanel_ChangedHandler(sender, e);
            RefreshTool();
        }

        public void RefreshTool()
        {
            Brush.Animation.Frames[0].Fill(CharacterPickPanel.SharedInstance.SettingForeground,
                                           CharacterPickPanel.SharedInstance.SettingBackground, 
                                           95);

            SadConsole.Effects.Blink blinkEffect = new SadConsole.Effects.Blink();
            blinkEffect.BlinkSpeed = 0.35f;
            //blinkManager.SetEffect(Brush.Animation.Frames[0][0], blinkEffect);
            Brush.Animation.Frames[0].SetEffect(Brush.Animation.Frames[0][0], blinkEffect);
            Brush.Animation.IsDirty = true;
        }

        public void Update()
        {
            //blinkManager.UpdateEffects(SadConsole.Global.GameTimeElapsedUpdate);
        }

        public bool ProcessKeyboard(Keyboard info, Console surface)
        {
            if (writing)
            {
                if (info.IsKeyDown(Keys.Escape))
                {
                    writing = false;
                    MainConsole.Instance.AllowKeyboardToMoveConsole = true;
                    MainConsole.Instance.DisableBrush = false;
                    tempConsole.Cursor.IsVisible = false;
                    tempConsole.IsDirty = true;
                    Brush.IsVisible = true;
                }
                else
                {
                    //tempConsole.SadConsole.Surfaces.Basic = (SurfaceBaseRendered)surface;
                    tempConsole.Cursor.PrintAppearance = new ColoredGlyph(CharacterPickPanel.SharedInstance.SettingForeground, CharacterPickPanel.SharedInstance.SettingBackground);
                    tempConsole.ProcessKeyboard(info);
                }

                return true;
            }

            return false;
        }

        public void ProcessMouse(MouseScreenObjectState info, Console surface, bool isInBounds)
        {
            if (info.IsOnScreenObject && info.Mouse.LeftClicked)
            {
                MainConsole.Instance.AllowKeyboardToMoveConsole = false;
                MainConsole.Instance.DisableBrush = true;
                writing = true;
                Brush.IsVisible = false;
                tempConsole = surface;

                if (surface.IsScrollable)
                {
                    tempConsole.Cursor.IsVisible = true;
                    tempConsole.Cursor.AutomaticallyShiftRowsUp = false;
                    tempConsole.Cursor.Position = info.SurfaceCellPosition + surface.ViewPosition;
                }
                else
                {
                    tempConsole = surface;
                    tempConsole.Cursor.IsVisible = true;
                    Brush.Position = info.SurfaceCellPosition;
                    tempConsole.Cursor.Position = info.SurfaceCellPosition;
                }
            }
        }
       
    }
}

