using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole;
using SadConsole.UI.Themes;
using SadConsole.UI;

namespace SadConsoleEditor.Controls
{
    internal class FileDirectoryListbox : ListBox
    {
        private string _currentFolder = null;
        private string _extFilter = "*.*";
        private string originalRootFolder;

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
                    _extFilter = value.ToLower();

                DisplayFolder(_currentFolder);
            }
        }

        public bool OnlyRootAndSubDirs { get; set; }

        public bool HideNonFilterFiles { get; set; }

        public string HighlightedExtentions { get; set; }

        public FileDirectoryListbox(int width, int height) : base(width, height, new FileDirectoryListboxItem())
        {
            HighlightedExtentions = "";
        }

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
                    var highlightExts = HighlightedExtentions.Trim(';').Split(';');
                    var filterExts = _extFilter.Trim(';').Split(';');

                    foreach (var filter in filterExts)
                    {
                        foreach (var item in System.IO.Directory.GetFiles(folder, filter))
                        {
                            var fileInfo = new System.IO.FileInfo(item);


                            if (highlightExts.Contains(fileInfo.Extension.ToLower()))
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

    internal class FileDirectoryListboxItem : ListBoxItemTheme
    {
        private ColoredGlyph _directoryAppNormal = new ColoredGlyph(Color.Purple, Color.Black);
        private ColoredGlyph _directoryAppMouseOver = new ColoredGlyph(Color.Purple, Color.Black);
        private ColoredGlyph _directoryAppSelected = new ColoredGlyph(new Color(255, 0, 255), Color.Black);
        private ColoredGlyph _directoryAppSelectedOver = new ColoredGlyph(new Color(255, 0, 255), Color.Black);
        private ColoredGlyph _fileAppNormal = new ColoredGlyph(Color.Gray, Color.Black);
        private ColoredGlyph _fileAppMouseOver = new ColoredGlyph(Color.Gray, Color.Black);
        private ColoredGlyph _fileAppSelected = new ColoredGlyph(Color.White, Color.Black);
        private ColoredGlyph _fileAppSelectedOver = new ColoredGlyph(Color.White, Color.Black);
        private ColoredGlyph _highExtAppNormal = new ColoredGlyph(Color.AnsiYellow, Color.Black);
        private ColoredGlyph _highExtAppMouseOver = new ColoredGlyph(Color.AnsiYellow, Color.Black);
        private ColoredGlyph _highExtAppSelected = new ColoredGlyph(Color.Yellow, Color.Black);
        private ColoredGlyph _highExtAppSelectedOver = new ColoredGlyph(Color.Yellow, Color.Black);

        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            _directoryAppNormal.Background = themeColors.ControlBackgroundNormal;
            _directoryAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _directoryAppSelected.Background = themeColors.ControlBackgroundSelected;
            _directoryAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            _fileAppNormal.Background = themeColors.ControlBackgroundNormal;
            _fileAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _fileAppSelected.Background = themeColors.ControlBackgroundSelected;
            _fileAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;

            _highExtAppNormal.Background = themeColors.ControlBackgroundNormal;
            _highExtAppMouseOver.Background = themeColors.ControlBackgroundMouseOver;
            _highExtAppSelected.Background = themeColors.ControlBackgroundSelected;
            _highExtAppSelectedOver.Background = themeColors.ControlBackgroundMouseOver;
        }


        public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            ColoredGlyph appearance;
            string displayString;

            if (item is System.IO.DirectoryInfo || item is FauxDirectory)
            {
                if (item is System.IO.DirectoryInfo info)
                    displayString = "<" + info.Name + ">";
                else
                    displayString = "<" + ((FauxDirectory)item).Name + ">";

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _directoryAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _directoryAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _directoryAppSelected;
                else
                    appearance = _directoryAppNormal;
            }
            else if (item is System.IO.FileInfo info)
            {
                displayString = info.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _fileAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _fileAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _fileAppSelected;
                else
                    appearance = _fileAppNormal;
            }
            else if (item is HighlightedExtFile extInfo)
            {
                displayString = extInfo.Name;

                if (itemState.HasFlag(ControlStates.MouseOver) && itemState.HasFlag(ControlStates.Selected))
                    appearance = _highExtAppSelectedOver;
                else if (itemState.HasFlag(ControlStates.MouseOver))
                    appearance = _highExtAppMouseOver;
                else if (itemState.HasFlag(ControlStates.Selected))
                    appearance = _highExtAppSelected;
                else
                    appearance = _highExtAppNormal;
            }
            else
            {
                appearance = GetStateAppearance(itemState);
                displayString = item.ToString();
            }

            control.Surface.Print(area.X, area.Y, displayString.Align(SadConsole.HorizontalAlignment.Left, area.Width), appearance);
        }

    }

    #endregion
}
