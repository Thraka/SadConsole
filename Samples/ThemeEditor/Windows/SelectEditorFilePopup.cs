using System;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI;

namespace ThemeEditor.Windows
{
    public class SelectEditorFilePopup : Window
    {
        private string fileFilterString;
        private FileDirectoryListbox directoryListBox;
        private TextBox fileName;
        private Button selectButton;
        private Button cancelButton;

        public string CurrentFolder
        {
            get { return directoryListBox.CurrentFolder; }
            set { directoryListBox.CurrentFolder = value; }
        }

        public bool AllowCancel
        {
            set { cancelButton.IsEnabled = value; }
        }
        
        public string PreferredExtensions
        {
            get { return directoryListBox.HighlightedExtentions; }
            set { directoryListBox.HighlightedExtentions = value; }
        }

        public string SelectedFile { get; private set; }

        public bool SkipFileExistCheck { get; set; }

        public string SelectButtonText { get { return selectButton.Text; } set { selectButton.Text = value; } }

        public SelectEditorFilePopup()
            : base(55, 28)
        {
            //Border.AddToWindow(this);
            Center();
            var colors = Controls.GetThemeColors();

            this.Print(1, 1, "Files", colors.Title);

            directoryListBox = new FileDirectoryListbox(Width - 2, Height - 9)
            {
                Position = new Point(1, 3),
            };

            fileFilterString = "*.colors;";

            directoryListBox.HighlightedExtentions = ".colors";
            directoryListBox.FileFilter = "*.colors";
            directoryListBox.SelectedItemChanged += _directoryListBox_SelectedItemChanged;
            directoryListBox.SelectedItemExecuted += _directoryListBox_SelectedItemExecuted;
            directoryListBox.OnlyRootAndSubDirs = true;
            directoryListBox.HideNonFilterFiles = true;
            directoryListBox.CurrentFolder = Environment.CurrentDirectory;

            this.Print(1, 2, new string((char)196, directoryListBox.Width));
            this.Print(1, directoryListBox.Position.Y + directoryListBox.Height, new string((char)196, directoryListBox.Width));

            fileName = new TextBox(directoryListBox.Width)
            {
                Position = new Point(directoryListBox.Bounds.X, directoryListBox.Bounds.MaxExtentY + 3),
            };
            fileName.TextChanged += _fileName_TextChanged;
            this.Print(fileName.Bounds.X, fileName.Bounds.Y - 1, "Selected file", colors.Title);

            selectButton = new Button(8)
            {
                Text = "Open",
                Position = new Point(Width - 10, Height - 2),
                IsEnabled = false
            };
            selectButton.Click += new EventHandler(_selectButton_Action);

            cancelButton = new Button(8)
            {
                Text = "Cancel",
                Position = new Point(2, Height - 2)
            };
            cancelButton.Click += new EventHandler(_cancelButton_Action);

            Controls.Add(directoryListBox);
            Controls.Add(fileName);
            Controls.Add(selectButton);
            Controls.Add(cancelButton);

            Title = "Select File";
        }
        
        public override void Show(bool modal)
        {
            SelectedFile = "";
            base.Show(modal);
        }

        void _cancelButton_Action(object sender, EventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        void _selectButton_Action(object sender, EventArgs e)
        {
            if (fileName.Text != string.Empty)
            {
                SelectedFile = System.IO.Path.Combine(directoryListBox.CurrentFolder, fileName.Text);
                var extensions = fileFilterString.Replace("*", "").Trim(';').Split(';');
                bool foundExtension = false;
                foreach (var item in extensions)
                {
                    if (SelectedFile.ToLower().EndsWith(item))
                    {
                        foundExtension = true;
                        break;
                    }
                }

                if (!foundExtension)
                    SelectedFile += extensions[0];

                DialogResult = true;
                Hide();
            }
        }

        void _directoryListBox_SelectedItemExecuted(object sender, FileDirectoryListbox.SelectedItemEventArgs e)
        {

        }

        void _directoryListBox_SelectedItemChanged(object sender, FileDirectoryListbox.SelectedItemEventArgs e)
        {
            if (e.Item is System.IO.FileInfo)
                fileName.Text = ((System.IO.FileInfo)e.Item).Name;
            else if (e.Item is FileDirectoryListbox.HighlightedExtFile)
                fileName.Text = ((FileDirectoryListbox.HighlightedExtFile)e.Item).Name;
            else
                fileName.Text = "";
        }

        void _fileName_TextChanged(object sender, EventArgs e)
        {
            selectButton.IsEnabled = fileName.Text != "" && (SkipFileExistCheck || System.IO.File.Exists(System.IO.Path.Combine(directoryListBox.CurrentFolder, fileName.Text)));
        }

        //public override void Invalidate()
        //{
        //    base.Invalidate();

        //    //    Print(2, Height - 2, fileFilterString.Replace(';', ' ').Replace("*", ""));

        //    Print(editorsListBox.Bounds.Left, editorsListBox.Bounds.Top - 2, "Editor", Theme.Colors.TitleText);
        //    Print(editorsListBox.Bounds.Left, editorsListBox.Bounds.Top - 1, new string((char)196, fileLoadersList.Width));

        //    Print(fileLoadersList.Bounds.Left, fileLoadersList.Bounds.Top - 2, "Type of file", Theme.Colors.TitleText);
        //    Print(fileLoadersList.Bounds.Left, fileLoadersList.Bounds.Top - 1, new string((char)196, fileLoadersList.Width));

        //    Print(directoryListBox.Bounds.Left, directoryListBox.Bounds.Top - 2, "Files", Theme.Colors.TitleText);
        //    Print(directoryListBox.Bounds.Left, directoryListBox.Bounds.Top - 1, new string((char)196, directoryListBox.Width));

        //    Print(fileName.Bounds.Left, fileName.Bounds.Top - 1, "Selected file", Theme.Colors.TitleText);

        //    if (fileLoadersList.SelectedItem is FileLoaders.IFileLoader loader)
        //    {
        //        List<string> filters = new List<string>();
        //        foreach (var ext in loader.Extensions)
        //            filters.Add($"*.{ext};");

        //        fileFilterString = string.Concat(filters);
        //        directoryListBox.FileFilter = fileFilterString;
        //        Print(fileName.Bounds.Left, fileName.Bounds.Bottom, new string(' ', Width - fileName.Bounds.Left - 1));
        //        Print(fileName.Bounds.Left, fileName.Bounds.Bottom, fileFilterString.Replace("*", "").Replace(";", " "));
        //    }
        //}
    }
}
