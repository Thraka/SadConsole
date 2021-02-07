using Microsoft.Xna.Framework;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SadConsoleEditor.Windows
{
    public class NewConsolePopup : SadConsole.Window
    {
        #region Fields
        private Button okButton;
        private Button cancelButton;
        private ListBox editorsListBox;

        private InputBox widthBox;
        private InputBox heightBox;

        private Controls.ColorPresenter _backgroundPicker;
        private Controls.ColorPresenter _foregroundPicker;
        //private InputBox _name;
        #endregion

        #region Properties
        public int SettingHeight { get; private set; }
        public int SettingWidth { get; private set; }
        public Editors.Editors Editor { get; private set; }

        public Color SettingForeground { get { return _foregroundPicker.SelectedColor; } }
        public Color SettingBackground { get { return _backgroundPicker.SelectedColor; } }

        public bool AllowCancel { set { cancelButton.IsEnabled = value; } }
        #endregion

        public NewConsolePopup() : base(40, 14)
        {
            //this.DefaultShowPosition = StartupPosition.CenterScreen;
            Title = "New Console";

            textSurface.DefaultBackground = Settings.Color_MenuBack;
            textSurface.DefaultForeground = Settings.Color_TitleText;
            Clear();
            Redraw();

            okButton = new Button(8, 1)
            {
                Text = "Accept",
                Position = new Microsoft.Xna.Framework.Point(base.TextSurface.Width - 10, 12)
            };
            okButton.Click += new EventHandler(_okButton_Action);

            cancelButton = new Button(8, 1)
            {
                Text = "Cancel",
                Position = new Microsoft.Xna.Framework.Point(2, 12)
            };
            cancelButton.Click += new EventHandler(_cancelButton_Action);

            //Print(2, 3, "Name");
            Print(2, 2, "Editor");
            Print(2, 7, "Width");
            Print(2, 8, "Height");

            editorsListBox = new ListBox(Width - 11, 4)
            {
                Position = new Point(9, 2),
                HideBorder = true
            };
            editorsListBox.SelectedItemChanged += editorsListBox_SelectedItemChanged;

            widthBox = new InputBox(3)
            {
                Text = "0",
                MaxLength = 3,
                IsNumeric = true,
                Position = new Microsoft.Xna.Framework.Point(base.TextSurface.Width - 5, 7)
            };

            heightBox = new InputBox(3)
            {
                Text = "0",
                MaxLength = 3,
                IsNumeric = true,
                Position = new Microsoft.Xna.Framework.Point(base.TextSurface.Width - 5, 8)
            };

            //_name = new InputBox(20)
            //{
            //    Text = name,
            //    Position = new Microsoft.Xna.Framework.Point(9, 3)
            //};

            _foregroundPicker = new SadConsoleEditor.Controls.ColorPresenter("Foreground", Theme.FillStyle.Foreground, TextSurface.Width - 4);
            _foregroundPicker.Position = new Point(2, 9);
            _foregroundPicker.SelectedColor = Color.White;

            _backgroundPicker = new SadConsoleEditor.Controls.ColorPresenter("Background", Theme.FillStyle.Foreground, TextSurface.Width - 4);
            _backgroundPicker.Position = new Point(2, 10);
            _backgroundPicker.SelectedColor = Color.Black;

            Add(editorsListBox);
            Add(widthBox);
            Add(heightBox);
            Add(cancelButton);
            Add(okButton);
            Add(_foregroundPicker);
            Add(_backgroundPicker);
            //Add(_name);

            foreach (var editor in MainScreen.Instance.Editors.Keys)
                editorsListBox.Items.Add(editor);

            editorsListBox.SelectedItem = editorsListBox.Items[0];
        }

        private void editorsListBox_SelectedItemChanged(object sender, ListBox<ListBoxItem>.SelectedItemEventArgs e)
        {
            Editor = MainScreen.Instance.Editors[(string)editorsListBox.SelectedItem];

            var settings = Settings.Config.GetSettings(Editor);

            widthBox.Text = settings.DefaultWidth.ToString();
            heightBox.Text = settings.DefaultHeight.ToString();
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

            int width = int.Parse(widthBox.Text);
            int height = int.Parse(heightBox.Text);

            SettingWidth = width < 1 ? 1 : width;
            SettingHeight = height < 1 ? 1 : height;
            //Name = _name.Text;

            Hide();
        }
    }
}
