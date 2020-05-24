using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsoleEditor.Controls;

namespace SadConsoleEditor.Windows
{
    public class SelectEditorFilePopup : SadConsole.UI.Window
    {
        private bool _initialized;
        private string currentFolder;
        private string fileFilterString;
        private Controls.FileDirectoryListbox directoryListBox;
        private TextBox fileName;
        private Button selectButton;
        private Button cancelButton;
        private ListBox fileLoadersList;
        private ListBox editorsListBox;

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

        public FileLoaders.IFileLoader SelectedLoader { get; private set; }

        public Editors.IEditorMetadata SelectedEditor { get; private set; }

        public bool SkipFileExistCheck { get; set; }

        public string SelectButtonText { get { return selectButton.Text; } set { selectButton.Text = value; } }

        public SelectEditorFilePopup(params Editors.IEditorMetadata[] editors)
            : base(70, 30)
        {

            editorsListBox = new ListBox(15, 5, new EditorsListBoxItem());
            editorsListBox.Position = new Point(2, 4);
            editorsListBox.SelectedItemChanged += EditorsListBox_SelectedItemChanged;
            this.Print(editorsListBox.Bounds.X, editorsListBox.Bounds.Y - 1, new string((char)196, editorsListBox.Width));

            if (editors.Length != 0)
                foreach (var item in editors)
                    editorsListBox.Items.Add(item);
            else
                foreach (var item in MainConsole.Instance.EditorTypes.Values)
                    editorsListBox.Items.Add(item);
            
            fileLoadersList = new ListBox(15, Height - editorsListBox.Bounds.MaxExtentY - 6, new FileLoaderListBoxItem());
            fileLoadersList.Position = new Point(2, editorsListBox.Bounds.MaxExtentY + 3);
            fileLoadersList.SelectedItemChanged += FileLoadersList_SelectedItemChanged;
            

            directoryListBox = new SadConsoleEditor.Controls.FileDirectoryListbox(Width - editorsListBox.Bounds.MaxExtentX - 3, Height - 10)
            {
                Position = new Point(editorsListBox.Bounds.MaxExtentX + 1, editorsListBox.Bounds.Y),
            };

            directoryListBox.HighlightedExtentions = ".con;.console;.brush";
            directoryListBox.SelectedItemChanged += _directoryListBox_SelectedItemChanged;
            directoryListBox.SelectedItemExecuted += _directoryListBox_SelectedItemExecuted;
            directoryListBox.OnlyRootAndSubDirs = true;
            directoryListBox.CurrentFolder = Environment.CurrentDirectory;
            //directoryListBox.HideBorder = true;

            this.Print(directoryListBox.Bounds.X, directoryListBox.Bounds.Y - 1, new string((char)196, directoryListBox.Width));

            fileName = new TextBox(directoryListBox.Width)
            {
                Position = new Point(directoryListBox.Bounds.X, directoryListBox.Bounds.MaxExtentY + 2),
            };
            fileName.TextChanged += _fileName_TextChanged;

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

            Add(directoryListBox);
            Add(fileName);
            Add(selectButton);
            Add(cancelButton);
            Add(editorsListBox);
            Add(fileLoadersList);

            editorsListBox.SelectedItem = editorsListBox.Items[0];
            Title = "Select File";
            _initialized = true;
        }

        private void EditorsListBox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            SelectedEditor = (Editors.IEditorMetadata)e.Item;

            fileLoadersList.Items.Clear();
            foreach (var item in Config.Program.GetSettings(SelectedEditor.Id).FileLoaders)
            {
                fileLoadersList.Items.Add(MainConsole.Instance.FileLoaders[item]);
            }

            fileLoadersList.SelectedItem = fileLoadersList.Items[0];
        }

        private void FileLoadersList_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            if (e.Item != null)
            {
                IsDirty = true;

                SelectedLoader = (FileLoaders.IFileLoader)e.Item;
            }
        }
        public override void Show(bool modal)
        {
            SelectedFile = "";
            fileLoadersList.SelectedItem = null;
            fileLoadersList.SelectedItem = fileLoadersList.Items[0];
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
                var rootDir = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
                var folder = directoryListBox.CurrentFolder.Remove(0, rootDir.Length).Trim('\\');
                SelectedFile = System.IO.Path.Combine(folder, fileName.Text);
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

        void _directoryListBox_SelectedItemExecuted(object sender, SadConsoleEditor.Controls.FileDirectoryListbox.SelectedItemEventArgs e)
        {

        }

        void _directoryListBox_SelectedItemChanged(object sender, SadConsoleEditor.Controls.FileDirectoryListbox.SelectedItemEventArgs e)
        {
            if (e.Item is System.IO.FileInfo)
                fileName.Text = ((System.IO.FileInfo)e.Item).Name;
            else if (e.Item is SadConsoleEditor.Controls.HighlightedExtFile)
                fileName.Text = ((SadConsoleEditor.Controls.HighlightedExtFile)e.Item).Name;
            else
                fileName.Text = "";
        }

        void _fileName_TextChanged(object sender, EventArgs e)
        {
            selectButton.IsEnabled = fileName.Text != "" && (SkipFileExistCheck || System.IO.File.Exists(System.IO.Path.Combine(directoryListBox.CurrentFolder, fileName.Text)));
        }

        protected override void OnInvalidated()
        {
            if (!_initialized) return;

            base.OnInvalidated();

            var colors = GetThemeColors();

            //    this.Print(2, Height - 2, fileFilterString.Replace(';', ' ').Replace("*", ""));

            this.Print(editorsListBox.Bounds.X, editorsListBox.Bounds.Y - 2, "Editor", colors.TitleText);
            this.Print(editorsListBox.Bounds.X, editorsListBox.Bounds.Y - 1, new string((char)196, fileLoadersList.Width));

            this.Print(fileLoadersList.Bounds.X, fileLoadersList.Bounds.Y - 2, "Type of file", colors.TitleText);
            this.Print(fileLoadersList.Bounds.X, fileLoadersList.Bounds.Y - 1, new string((char)196, fileLoadersList.Width));

            this.Print(directoryListBox.Bounds.X, directoryListBox.Bounds.Y - 2, "Files", colors.TitleText);
            this.Print(directoryListBox.Bounds.X, directoryListBox.Bounds.Y - 1, new string((char)196, directoryListBox.Width));

            this.Print(fileName.Bounds.X, fileName.Bounds.Y - 1, "Selected file", colors.TitleText);

            if (fileLoadersList.SelectedItem is FileLoaders.IFileLoader loader)
            {
                List<string> filters = new List<string>();
                foreach (var ext in loader.Extensions)
                    filters.Add($"*.{ext};");

                fileFilterString = string.Concat(filters);
                directoryListBox.FileFilter = fileFilterString;
                this.Print(fileName.Bounds.X, fileName.Bounds.MaxExtentY, new string(' ', Width - fileName.Bounds.X - 1));
                this.Print(fileName.Bounds.X, fileName.Bounds.MaxExtentY, fileFilterString.Replace("*", "").Replace(";", " "));
            }
        }
    }
}
