using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadConsole.Controls;
using SadConsole;
using Microsoft.Xna.Framework;
using SadConsole.Surfaces;

namespace SadConsoleEditor.Controls
{
    internal class FileDirectoryListbox : ListBox<FileDirectoryListboxItem>
    {
        #region Fields
        private string _currentFolder = null;
        private string _extFilter = "*.*";
        private string originalRootFolder;
        #endregion

        #region Properties
        public string CurrentFolder
        {
            get { return _currentFolder; }
            set
            {
                if (_currentFolder == null)
                    originalRootFolder = value;

                if (DisplayFolder(value))
                {
                    _currentFolder = value;
                }
            }
        }

        public string FileFilter
        {
            get { return _extFilter; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _extFilter = "*.*";
                else
                    _extFilter = value;

                DisplayFolder(_currentFolder);
            }
        }

        public bool OnlyRootAndSubDirs { get; set; }

        public bool HideNonFilterFiles { get; set; }

        public string HighlightedExtentions { get; set; }
        #endregion

        #region Constructors
        public FileDirectoryListbox(int width, int height) : base(width, height)
        {
            HighlightedExtentions = "";
        }
        #endregion

        #region Methods
        private bool DisplayFolder(string folder)
        {
            if (System.IO.Directory.Exists(folder))
            {
                try
                {
                    List<object> newItems = new List<object>(20);
                    var dir = new System.IO.DirectoryInfo(folder);

                    if (dir.Parent != null && (!OnlyRootAndSubDirs || (OnlyRootAndSubDirs && System.IO.Path.GetFullPath(folder).ToLower() != System.IO.Path.GetFullPath(originalRootFolder).ToLower())))
                        newItems.Add(new FauxDirectory { Name = ".." });

                    foreach (var item in System.IO.Directory.GetDirectories(folder))
                        newItems.Add(new System.IO.DirectoryInfo(item));
                    var highlightExts = HighlightedExtentions.Split(';');
                    var filterExts = _extFilter.Split(';');

                    foreach (var filter in filterExts)
                    {
                        foreach (var item in System.IO.Directory.GetFiles(folder, filter))
                        {
                            var fileInfo = new System.IO.FileInfo(item);


                            if (highlightExts.Contains(fileInfo.Extension))
                                newItems.Add(new HighlightedExtFile() { Name = fileInfo.Name });
                            else
                                newItems.Add(fileInfo);
                        }
                    }
                    

                    base.Items.Clear();

                    foreach (var item in newItems)
                        base.Items.Add(item);

                    return true;
                }
                catch (Exception e1)
                {
                    return false;
                }
            }
            else
                return false;
        }

        protected override void OnItemAction()
        {
            base.OnItemAction();

            if (selectedItem is System.IO.DirectoryInfo)
            {
                CurrentFolder = ((System.IO.DirectoryInfo)selectedItem).FullName;
                if (Items.Count > 0)
                    SelectedItem = Items[0];
            }
            else if (selectedItem is FauxDirectory)
            {
                if (((FauxDirectory)selectedItem).Name == "..")
                {
                    CurrentFolder = System.IO.Directory.GetParent(_currentFolder).FullName;
                    if (Items.Count > 0)
                        SelectedItem = Items[0];
                }
            }
            else if (selectedItem is System.IO.FileInfo)
            {

            }
        }

        #endregion
    }

    #region Classes

    internal class FauxDirectory
    {
        public string Name;
    }

    internal class HighlightedExtFile
    {
        public string Name;
    }

    internal class FileDirectoryListboxItem : ListBoxItem
    {
        private string _displayString;
        private Cell _directoryAppNormal = new Cell(Color.Purple, Settings.Color_ControlBack);
        private Cell _directoryAppMouseOver = new Cell(Color.Purple, Settings.Color_ControlBackDim);
        private Cell _directoryAppSelected = new Cell(new Color(255, 0, 255), Settings.Color_ControlBack);
        private Cell _directoryAppSelectedOver = new Cell(new Color(255, 0, 255), Settings.Color_ControlBackDim);
        private Cell _fileAppNormal = new Cell(Color.Gray, Settings.Color_ControlBack);
        private Cell _fileAppMouseOver = new Cell(Color.Gray, Settings.Color_ControlBackDim);
        private Cell _fileAppSelected = new Cell(Color.White, Settings.Color_ControlBack);
        private Cell _fileAppSelectedOver = new Cell(Color.White, Settings.Color_ControlBackDim);
        private Cell _highExtAppNormal = new Cell(ColorAnsi.Yellow, Settings.Color_ControlBack);
        private Cell _highExtAppMouseOver = new Cell(ColorAnsi.Yellow, Settings.Color_ControlBackDim);
        private Cell _highExtAppSelected = new Cell(Color.Yellow, Settings.Color_ControlBack);
        private Cell _highExtAppSelectedOver = new Cell(Color.Yellow, Settings.Color_ControlBackDim);

        protected override void DetermineAppearance()
        {
            var oldAppearance = base._currentAppearance;

            if (base.Item is System.IO.DirectoryInfo || base.Item is FauxDirectory)
            {
                if (_isMouseOver && _isSelected)
                    base._currentAppearance = _directoryAppSelectedOver;
                else if (_isMouseOver)
                    base._currentAppearance = _directoryAppMouseOver;
                else if (_isSelected)
                    base._currentAppearance = _directoryAppSelected;
                else
                    base._currentAppearance = _directoryAppNormal;
            }
            else if (base.Item is System.IO.FileInfo)
            {
                if (_isMouseOver && _isSelected)
                    base._currentAppearance = _fileAppSelectedOver;
                else if (_isMouseOver)
                    base._currentAppearance = _fileAppMouseOver;
                else if (_isSelected)
                    base._currentAppearance = _fileAppSelected;
                else
                    base._currentAppearance = _fileAppNormal;
            }
            else if (base.Item is HighlightedExtFile)
            {
                if (_isMouseOver && _isSelected)
                    base._currentAppearance = _highExtAppSelectedOver;
                else if (_isMouseOver)
                    base._currentAppearance = _highExtAppMouseOver;
                else if (_isSelected)
                    base._currentAppearance = _highExtAppSelected;
                else
                    base._currentAppearance = _highExtAppNormal;
            }

            if (oldAppearance != base._currentAppearance)
                IsDirty = true;
        }

        protected override void OnItemChanged(object oldItem, object newItem)
        {
            if (base.Item is System.IO.DirectoryInfo)
                _displayString = "<" + ((System.IO.DirectoryInfo)base.Item).Name + ">";
            else if (base.Item is FauxDirectory)
                _displayString = "<" + ((FauxDirectory)base.Item).Name + ">";
            else if (base.Item is System.IO.FileInfo)
                _displayString = ((System.IO.FileInfo)base.Item).Name;
            else if (base.Item is HighlightedExtFile)
                _displayString = ((HighlightedExtFile)base.Item).Name;

            DetermineAppearance();

            base.OnItemChanged(oldItem, newItem);

        }

        public override void Draw(SurfaceBase surface, Rectangle area)
        {
            SadConsoleEditor.Settings.QuickEditor.TextSurface = surface;
            SadConsoleEditor.Settings.QuickEditor.Print(area.X, area.Y, _displayString.Align(System.Windows.HorizontalAlignment.Left, area.Width), base._currentAppearance);
            IsDirty = false;
        }


    }
    #endregion
}
