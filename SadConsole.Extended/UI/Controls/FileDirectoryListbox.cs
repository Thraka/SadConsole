using System;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.UI.Controls;

/// <summary>
/// A listbox control that displays the file system.
/// </summary>
public class FileDirectoryListbox : ListBox
{
    private string _currentFolder = null;
    private string _extFilter = "*.*";
    private string _originalRootFolder;

    /// <summary>
    /// The current folder displayed by the listbox.
    /// </summary>
    public string CurrentFolder
    {
        get { return _currentFolder; }
        set
        {
            if (_currentFolder == null)
                _originalRootFolder = value;

            if (DisplayFolder(value))
            {
                _currentFolder = value;
            }
        }
    }

    /// <summary>
    /// A *.* wildcard filesystem filter. Use <code>;</code> to split multiple filters.
    /// </summary>
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

    /// <summary>
    /// When <see langword="true"/>, only allows navigation from the root folder of the original value provided to <see cref="CurrentFolder"/> and below; otherwise <see langword="false"/>.
    /// </summary>
    public bool OnlyRootAndSubDirs { get; set; }

    /// <summary>
    /// When <see langword="true"/>, only displays files that match <see cref="FileFilter"/>; otherwise <see langword="false"/> to display all files.
    /// </summary>
    public bool HideNonFilterFiles { get; set; }

    /// <summary>
    /// When <see langword="true"/>, only displays files that match <see cref="FileFilter"/>; otherwise <see langword="false"/> to display all files.
    /// </summary>
    public string HighlightedExtentions { get; set; }

    /// <summary>
    /// Creates a new instance of the control and uses <see cref="FileDirectoryListboxItem"/> as the item theme.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="height">The height of the control.</param>
    public FileDirectoryListbox(int width, int height) : this(width, height, new FileDirectoryListboxItem()) { }

    /// <summary>
    /// Creates a new instance of the control with the specified item theme.
    /// </summary>
    /// <param name="width">The width of the control.</param>
    /// <param name="height">The height of the control.</param>
    /// <param name="itemTheme">The theme to use for the items.</param>
    public FileDirectoryListbox(int width, int height, ListBoxItemTheme itemTheme) : base(width, height, itemTheme) { }


    private bool DisplayFolder(string folder)
    {
        if (System.IO.Directory.Exists(folder))
        {
            try
            {
                List<object> newItems = new List<object>(20);
                var dir = new System.IO.DirectoryInfo(folder);

                if (dir.Parent != null && (!OnlyRootAndSubDirs || (OnlyRootAndSubDirs && System.IO.Path.GetFullPath(folder).ToLower() != System.IO.Path.GetFullPath(_originalRootFolder).ToLower())))
                    newItems.Add(new FauxDirectory { Name = ".." });

                foreach (var item in System.IO.Directory.GetDirectories(folder))
                    newItems.Add(new System.IO.DirectoryInfo(item));
                var highlightExts = HighlightedExtentions?.Trim(';').Split(';') ?? Array.Empty<string>();
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
            catch (Exception)
            {
                return false;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// Navigates a directory if a directory is selected. Raises the <see cref="ListBox.SelectedItemExecuted"/> event.
    /// </summary>
    protected override void OnItemAction()
    {
        base.OnItemAction();

        if (SelectedItem is System.IO.DirectoryInfo)
        {
            CurrentFolder = ((System.IO.DirectoryInfo)SelectedItem).FullName;
            if (Items.Count > 0)
                SelectedItem = Items[0];
        }
        else if (SelectedItem is FauxDirectory)
        {
            if (((FauxDirectory)SelectedItem).Name == "..")
            {
                CurrentFolder = System.IO.Directory.GetParent(_currentFolder).FullName;
                if (Items.Count > 0)
                    SelectedItem = Items[0];
            }
        }
        else if (SelectedItem is System.IO.FileInfo)
        {

        }
    }

    /// <summary>
    /// A listbox item container that represents a fake directory, such as <code>..</code>.
    /// </summary>
    public class FauxDirectory
    {
        /// <summary>
        /// The name of the directory.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// A listbox item container that represents a highlighted file.
    /// </summary>
    public class HighlightedExtFile
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; set; }
    }
}
