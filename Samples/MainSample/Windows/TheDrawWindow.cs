using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole;

namespace FeatureDemo.Windows
{
    class TheDrawWindow: Window
    {
        private ListBox _childFontsListbox;
        private SadConsole.Readers.TheDrawFont[] _selectedFonts;

        public SadConsole.Readers.TheDrawFont SelectedFont { get; private set; }

        public TheDrawWindow() : base(40, 20)
        {
            Title = "Select TheDraw Font";
            Colors colors = Controls.GetThemeColors();

            Cursor.PrintAppearanceMatchesHost = false;
            Cursor.DisableWordBreak = true;
            Cursor.SetPrintAppearance(colors.Title, DefaultBackground);

            FileDirectoryListbox fileListBox = new FileDirectoryListbox(18, Height - 5);
            fileListBox.FileFilter = "*.tdf";
            fileListBox.OnlyRootAndSubDirs = true;
            fileListBox.HideNonFilterFiles = true;
            fileListBox.CurrentFolder = "./TheDraw/";
            fileListBox.Position = (1, 2);
            //((SadConsole.UI.Themes.ListBoxTheme)fileListBox.Theme).DrawBorder = true;
            Controls.Add(fileListBox);

            Surface.DrawLine((fileListBox.Bounds.MaxExtentX + 1, 1),
                             (fileListBox.Bounds.MaxExtentX + 1, Height - 2),
                             ICellSurface.ConnectedLineThin[3], colors.Lines);

            Surface.ConnectLines(ICellSurface.ConnectedLineThin);

            Cursor.Move(fileListBox.Position.X, fileListBox.Position.Y - 1)
                  .Print("Files")
                  .Move(fileListBox.Bounds.MaxExtentX + 2, fileListBox.Position.Y - 1)
                  .Print("Fonts in file");

            _childFontsListbox = new ListBox(Width - fileListBox.Bounds.MaxExtentX - 4, fileListBox.Height)
            {
                Position = (fileListBox.Bounds.MaxExtentX + 2, fileListBox.Position.Y)
            };
            _childFontsListbox.SelectedItemChanged += _childFontsListbox_SelectedItemChanged;
            Controls.Add(_childFontsListbox);

            fileListBox.SelectedItemChanged += FileListBox_SelectedItemChanged;
            fileListBox.SelectedIndex = 0;

            Button cancelButton = new Button(10, 1)
            {
                Text = "Cancel",
                Position = (1, Height - 2)
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Hide(); };
            Controls.Add(cancelButton);

            Button okButton = new Button(6, 1)
            {
                Text = "OK",
                Position = (Width - 1 - 7, Height - 2)
            };
            okButton.Click += (s, e) => { DialogResult = true; Hide(); };
            Controls.Add(okButton);
        }

        private void _childFontsListbox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            if (e.Item == null)
                SelectedFont = null;
            else
                SelectedFont = (SadConsole.Readers.TheDrawFont)((ValueTuple<string, object>)e.Item).Item2;
        }

        private void FileListBox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            _selectedFonts = SadConsole.Readers.TheDrawFont.ReadFonts(e.Item.ToString()).ToArray();
            PrintFontInformation();
        }

        private void PrintFontInformation()
        {
            _childFontsListbox.Items.Clear();

            foreach (var item in _selectedFonts)
                _childFontsListbox.Items.Add((item.Title, (object)item));

            _childFontsListbox.SelectedIndex = 0;
        }
    }
}
