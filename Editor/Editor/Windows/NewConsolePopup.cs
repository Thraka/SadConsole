using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SadConsoleEditor.Windows
{
    public class NewConsolePopup : SadConsole.UI.Window
    {
        private Button _okButton;
        private Button _cancelButton;
        private ListBox _editorsListBox;

        private TextBox _widthBox;
        private TextBox _heightBox;

        private Controls.ColorPresenter _backgroundPicker;
        private Controls.ColorPresenter _foregroundPicker;
        //private InputBox _name;

        public int SettingHeight { get; private set; }
        public int SettingWidth { get; private set; }
        public Editors.IEditorMetadata Editor { get; private set; }

        public Color SettingForeground { get { return _foregroundPicker.SelectedColor; } }
        public Color SettingBackground { get { return _backgroundPicker.SelectedColor; } }

        public bool AllowCancel { set { _cancelButton.IsEnabled = value; } }

        public NewConsolePopup() : base(40, 14)
        {
            //this.DefaultShowPosition = StartupPosition.CenterScreen;
            Title = "New Editor";

            //DefaultBackground = Settings.Color_MenuBack;
            //DefaultForeground = Settings.Color_TitleText;

            _okButton = new Button(8, 1)
            {
                Text = "Create",
                Position = new SadRogue.Primitives.Point(Width - 10, 12)
            };
            _okButton.Click += new EventHandler(_okButton_Action);

            _cancelButton = new Button(8, 1)
            {
                Text = "Cancel",
                Position = new SadRogue.Primitives.Point(2, 12)
            };
            _cancelButton.Click += new EventHandler(_cancelButton_Action);

            _editorsListBox = new ListBox(Width - 11, 4, new Controls.EditorsListBoxItem())
            {
                Position = new Point(9, 2),
            };
            _editorsListBox.SelectedItemChanged += editorsListBox_SelectedItemChanged;

            _widthBox = new TextBox(3)
            {
                Text = "0",
                MaxLength = 3,
                IsNumeric = true,
                Position = new SadRogue.Primitives.Point(Width - 5, 7)
            };

            _heightBox = new TextBox(3)
            {
                Text = "0",
                MaxLength = 3,
                IsNumeric = true,
                Position = new SadRogue.Primitives.Point(Width - 5, 8)
            };

            //_name = new InputBox(20)
            //{
            //    Text = name,
            //    Position = new SadRogue.Primitives.Point(9, 3)
            //};

            _foregroundPicker = new SadConsoleEditor.Controls.ColorPresenter("Foreground", Color.White, Width - 4);
            _foregroundPicker.Position = new Point(2, 9);
            _foregroundPicker.SelectedColor = Color.White;

            _backgroundPicker = new SadConsoleEditor.Controls.ColorPresenter("Background", Color.Transparent, Width - 4);
            _backgroundPicker.Position = new Point(2, 10);
            _backgroundPicker.SelectedColor = Color.Black;

            Controls.Add(_editorsListBox);
            Controls.Add(_widthBox);
            Controls.Add(_heightBox);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            Controls.Add(_foregroundPicker);
            Controls.Add(_backgroundPicker);
            //Add(_name);
            

            foreach (var editor in MainConsole.Instance.EditorTypes.Values)
                _editorsListBox.Items.Add(editor);

            _editorsListBox.SelectedItem = _editorsListBox.Items[0];
        }

        protected override void DrawBorder()
        {
            base.DrawBorder();

            var colors = Controls.GetThemeColors();
            var fillStyle = new ColoredGlyph(colors.ControlHostForeground, colors.ControlHostBackground);


            //_foregroundPicker.SelectedColor = fillStyle.Foreground;
            //_backgroundPicker.SelectedColor = fillStyle.Background;


            //this.Print(2, 3, "Name");
            this.Print(2, 2, "Editor");
            this.Print(2, 7, "Width");
            this.Print(2, 8, "Height");
        }

        private void editorsListBox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            Editor = (Editors.IEditorMetadata)_editorsListBox.SelectedItem;

            var settings = Config.Program.GetSettings(Editor.Id);

            _widthBox.Text = settings.DefaultWidth.ToString();
            _heightBox.Text = settings.DefaultHeight.ToString();
            _foregroundPicker.SelectedColor = settings.DefaultForeground;
            _backgroundPicker.SelectedColor = settings.DefaultBackground;
        }

        void _cancelButton_Action(object sender, EventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        void _okButton_Action(object sender, EventArgs e)
        {
            DialogResult = true;

            int width = int.Parse(_widthBox.Text);
            int height = int.Parse(_heightBox.Text);

            SettingWidth = width < 1 ? 1 : width;
            SettingHeight = height < 1 ? 1 : height;
            //Name = _name.Text;

            Hide();
        }
    }
}
