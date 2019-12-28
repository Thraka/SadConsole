using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
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
        private CheckBox _mirrorLR;
        private CheckBox _mirrorTB;

        private Mirror _settingMirrorEffect;
        private bool _skipChanged;
        

        public Color SettingForeground { get { return _foreColor.SelectedColor; } set { _foreColor.SelectedColor = value; } }
        public Color SettingBackground { get { return _backColor.SelectedColor; } set { _backColor.SelectedColor = value; } }
        public int SettingCharacter { get { return _characterPicker.SelectedCharacter; } set { _characterPicker.SelectedCharacter = value; } }
        public Mirror SettingMirrorEffect
        {
            get { return _settingMirrorEffect; }
            set
            {
                _skipChanged = true;
                _settingMirrorEffect = value;
                _mirrorLR.IsSelected = (value & Mirror.Horizontal) == Mirror.Horizontal;
                _mirrorTB.IsSelected = (value & Mirror.Vertical) == Mirror.Vertical;
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

            _foreColor = new ColorPresenter("Foreground", SadConsole.UI.Themes.Library.Default.Colors.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _backColor = new ColorPresenter("Background", SadConsole.UI.Themes.Library.Default.Colors.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _charPreview = new ColorPresenter("Character", SadConsole.UI.Themes.Library.Default.Colors.Green, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _mirrorLR = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            _mirrorTB = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            _characterPicker = new CharacterPicker(SadConsole.UI.Themes.Library.Default.Colors.OrangeDark, SadConsole.UI.Themes.Library.Default.Colors.ControlBack, SadConsole.UI.Themes.Library.Default.Colors.Yellow);

            _mirrorLR.Text = "Mirror Horiz.";
            _mirrorTB.Text = "Mirror Vert.";

            _foreColor.SelectedColor = Color.White;
            _backColor.SelectedColor = Color.Black;

            _foreColor.RightClickedColor += (s, e) => { var tempColor = SettingBackground; SettingBackground = SettingForeground; SettingForeground = tempColor; };
            _backColor.RightClickedColor += (s, e) => { var tempColor = SettingForeground; SettingForeground = SettingBackground; SettingBackground = tempColor; };

            _charPreview.GlyphColor = _foreColor.SelectedColor;
            _charPreview.SelectedColor = _backColor.SelectedColor;
            _charPreview.SelectedGlyph = 0;
            _charPreview.DisableColorPicker = true;
            _charPreview.EnableCharacterPicker = true;
            _charPreview.GlyphChanged += (o, e) => { _characterPicker.SelectedCharacter = _charPreview.SelectedGlyph; };

            _mirrorLR.IsSelectedChanged += Mirror_IsSelectedChanged;
            _mirrorTB.IsSelectedChanged += Mirror_IsSelectedChanged;
            _foreColor.ColorChanged += (o, e) => { _charPreview.GlyphColor = _foreColor.SelectedColor; OnChanged(); };
            _backColor.ColorChanged += (o, e) => { _charPreview.SelectedColor = _backColor.SelectedColor; OnChanged(); };
            _characterPicker.SelectedCharacterChanged += (sender, e) => { _charPreview.SelectedGlyph = e.NewCharacter; _charPreview.Title = "Character (" + e.NewCharacter.ToString() + ")"; OnChanged(); };
            _characterPicker.SelectedCharacter = 1;

            HideCharacter = hideCharacter;
            HideForeground = hideForeground;
            HideBackground = hideBackground;

            HideMirrorLR = HideMirrorTB = HideCharacter;

            Reset();
        }

        void Mirror_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_mirrorLR.IsSelected && _mirrorTB.IsSelected)
                _charPreview.Surface[_charPreview.Width - 2, 0].Mirror = Mirror.Vertical | Mirror.Horizontal;
            else if (_mirrorLR.IsSelected)
                _charPreview.Surface[_charPreview.Width - 2, 0].Mirror = Mirror.Horizontal;
            else if (_mirrorTB.IsSelected)
                _charPreview.Surface[_charPreview.Width - 2, 0].Mirror = Mirror.Vertical;
            else
                _charPreview.Surface[_charPreview.Width - 2, 0].Mirror = Mirror.None;

            _charPreview.SelectedGlyphMirror = _characterPicker.MirrorEffect = _settingMirrorEffect = _charPreview.Surface[_charPreview.Width - 2, 0].Mirror;

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
            Changed?.Invoke(this, EventArgs.Empty);
            MainConsole.Instance.ActiveEditor?.SelectedTool?.RefreshTool();
        }

        public override void ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
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
