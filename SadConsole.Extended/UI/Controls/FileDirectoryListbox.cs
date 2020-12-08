using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.UI.Controls;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.UI.Themes;
using SadConsole.UI;

namespace SadConsole.UI.Controls
{
    public class FileDirectoryListbox : ListBox
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

        public class FauxDirectory
        {
            public string Name;
        }

        public class HighlightedExtFile
        {
            public string Name;
        }
    }
}
