using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsoleEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    public class CharacterPickPanel: CustomPanel
    {
        public static CharacterPickPanel SharedInstance = new CharacterPickPanel("Characters", false, false, false);

        public event EventHandler Changed;

        private Controls.ColorPresenter _foreColor;
        private Controls.ColorPresenter _backColor;
        private Controls.ColorPresenter _charPreview;
        private Controls.CharacterPicker _characterPicker;
        private Windows.CharacterQuickSelectPopup _popupCharacterWindow;
        private CheckBox _mirrorLR;
        private CheckBox _mirrorTB;

        private Microsoft.Xna.Framework.Graphics.SpriteEffects _settingMirrorEffect;
        private bool _skipChanged;
        

        public Color SettingForeground { get { return _foreColor.SelectedColor; } set { _foreColor.SelectedColor = value; } }
        public Color SettingBackground { get { return _backColor.SelectedColor; } set { _backColor.SelectedColor = value; } }
        public int SettingCharacter { get { return _characterPicker.SelectedCharacter; } set { _characterPicker.SelectedCharacter = value; } }
        public Microsoft.Xna.Framework.Graphics.SpriteEffects SettingMirrorEffect
        {
            get { return _settingMirrorEffect; }
            set
            {
                _skipChanged = true;
                _settingMirrorEffect = value;
                _mirrorLR.IsSelected = (value & Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally) == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                _mirrorTB.IsSelected = (value & Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically) == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
                _skipChanged = false;
            }
        }

        public bool HideCharacter;
        public bool HideForeground;
        public bool HideBackground;

        public bool HideMirrorLR;
        public bool HideMirrorTB;

        public Font PickerFont { get { return _characterPicker.AlternateFont; } set { _characterPicker.AlternateFont = value; } }

        public CharacterPickPanel(string title, bool hideCharacter, bool hideForeground, bool hideBackground)
        {
            Title = title;

            _foreColor = new ColorPresenter("Foreground", Settings.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _backColor = new ColorPresenter("Background", Settings.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _charPreview = new ColorPresenter("Character", Settings.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _mirrorLR = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            _mirrorTB = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            _characterPicker = new CharacterPicker(Settings.Red, Settings.Color_ControlBack, Settings.Green);
            _popupCharacterWindow = new Windows.CharacterQuickSelectPopup(0);

            _mirrorLR.Text = "Mirror Horiz.";
            _mirrorTB.Text = "Mirror Vert.";

            _foreColor.SelectedColor = Color.White;
            _backColor.SelectedColor = Color.Black;

            _foreColor.RightClickedColor += (s, e) => { var tempColor = SettingBackground; SettingBackground = SettingForeground; SettingForeground = tempColor; };
            _backColor.RightClickedColor += (s, e) => { var tempColor = SettingForeground; SettingForeground = SettingBackground; SettingBackground = tempColor; };

            _charPreview.CharacterColor = _foreColor.SelectedColor;
            _charPreview.SelectedColor = _backColor.SelectedColor;
            _charPreview.Character = 0;
            _charPreview.DisableColorPicker = true;
            _charPreview.EnableCharacterPicker = true;

            _popupCharacterWindow.TextSurface.Font = Settings.Config.ScreenFont;
            _popupCharacterWindow.Closed += (o, e) => { _characterPicker.SelectedCharacter = _popupCharacterWindow.SelectedCharacter; };

            _mirrorLR.IsSelectedChanged += Mirror_IsSelectedChanged;
            _mirrorTB.IsSelectedChanged += Mirror_IsSelectedChanged;
            _foreColor.ColorChanged += (o, e) => { _charPreview.CharacterColor = _foreColor.SelectedColor; OnChanged(); };
            _backColor.ColorChanged += (o, e) => { _charPreview.SelectedColor = _backColor.SelectedColor; OnChanged(); };
            _characterPicker.SelectedCharacterChanged += (sender, e) => { _charPreview.Character = e.NewCharacter; _charPreview.Title = "Character (" + e.NewCharacter.ToString() + ")"; OnChanged(); };
            _characterPicker.SelectedCharacter = 1;
            _charPreview.MouseButtonClicked += (o, e) => { 
                if (e.MouseState.Mouse.LeftClicked)
                {
                    _popupCharacterWindow.Center();
                    _popupCharacterWindow.Show(true);
                }
            };


            HideCharacter = hideCharacter;
            HideForeground = hideForeground;
            HideBackground = hideBackground;

            HideMirrorLR = HideMirrorTB = HideCharacter;

            Reset();
        }

        void Mirror_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_mirrorLR.IsSelected && _mirrorTB.IsSelected)
                _charPreview[_charPreview.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically | Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            else if (_mirrorLR.IsSelected)
                _charPreview[_charPreview.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            else if (_mirrorTB.IsSelected)
                _charPreview[_charPreview.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;
            else
                _charPreview[_charPreview.Width - 2, 0].Mirror = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            _popupCharacterWindow.MirrorEffect = _characterPicker.MirrorEffect = _settingMirrorEffect = _charPreview[_charPreview.Width - 2, 0].Mirror;

            if (!_skipChanged)
                OnChanged();
        }

        public void Reset()
        {
            var tempControls = new List<ControlBase>() { _foreColor, _backColor, _charPreview, _mirrorLR, _mirrorTB, _characterPicker };

            if (HideForeground) tempControls.Remove(_foreColor);
            if (HideBackground) tempControls.Remove(_backColor);
            if (HideCharacter) { tempControls.Remove(_charPreview); tempControls.Remove(_characterPicker); }

            if (HideCharacter || HideMirrorLR) { tempControls.Remove(_mirrorLR); _mirrorLR.IsSelected = false; }
            if (HideCharacter || HideMirrorTB) { tempControls.Remove(_mirrorTB); _mirrorTB.IsSelected = false; }

            Controls = tempControls.ToArray();
        }

        private void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);

            //if (MainScreen.Instance.ToolsPane != null)
                //MainScreen.Instance.ToolsPane.SelectedTool.RefreshTool();
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            if (control == _characterPicker)
                control.Position = new Point(2, control.Position.Y + 1);

            return 0;
        }

        public override void Loaded()
        {
        }
    }
}
