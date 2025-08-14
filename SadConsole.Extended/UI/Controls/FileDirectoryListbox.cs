using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SadConsole.UI.Controls;

/// <summary>
/// A listbox control that displays the file system.
/// </summary>
public class FileDirectoryListbox : ListBox
{
    private string? _currentFolder = null;
    private string? _originalRootFolder;
    private string _extFilter = "*.*";

    /// <summary>
    /// The current folder displayed by the listbox.
    /// </summary>
    public string? CurrentFolder
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

        [MemberNotNull(nameof(_extFilter))]
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
    /// A list of extensions of files to highlight, separated by a semicolon. The extension is just the name without any wildcards or periods, in lowercase. Example: <c>txt;json;xml</c>.
    /// </summary>
    public string? HighlightedExtentions { get; set; }

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

    /// <summary>
    /// Updates the listbox items with the contents of the specified folder.
    /// </summary>
    /// <param name="folder">The folder path to display.</param>
    /// <returns><see langword="true"/> if the folder was successfully displayed; otherwise, <see langword="false"/>.</returns>
    private bool DisplayFolder(string? folder)
    {
        if (!string.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder))
        {
            try
            {
                List<object> newItems = new(20);
                System.IO.DirectoryInfo dir = new(folder);

                if (dir.Parent != null && (!OnlyRootAndSubDirs || (OnlyRootAndSubDirs && System.IO.Path.GetFullPath(folder).ToLower() != System.IO.Path.GetFullPath(_originalRootFolder).ToLower())))
                    newItems.Add(new FauxDirectory(".."));

                foreach (string item in System.IO.Directory.GetDirectories(folder))
                    newItems.Add(new System.IO.DirectoryInfo(item));

                string[] highlightExts = HighlightedExtentions?.Trim(';').Split(';') ?? Array.Empty<string>();
                string[] filterExts = _extFilter.Trim(';').Split(';');

                foreach (string filter in filterExts)
                {
                    foreach (string item in System.IO.Directory.GetFiles(folder, filter))
                    {
                        System.IO.FileInfo fileInfo = new(item);

                        if (highlightExts.Contains(fileInfo.Extension.ToLower()))
                            newItems.Add(new HighlightedExtFile(fileInfo.Name));
                        else
                            newItems.Add(fileInfo);
                    }
                }
                

                Items.Clear();

                foreach (object item in newItems)
                    Items.Add(item);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Navigates the file system when a directory item is selected and raises the <see cref="ListBox.SelectedItemExecuted"/> event.
    /// </summary>
    /// <remarks>
    /// When a directory is selected, updates <see cref="CurrentFolder"/> to navigate into that directory.
    /// When '..' is selected, navigates to the parent directory if allowed by <see cref="OnlyRootAndSubDirs"/>.
    /// </remarks>
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
                CurrentFolder = System.IO.Directory.GetParent(_currentFolder!)!.FullName;
                if (Items.Count > 0)
                    SelectedItem = Items[0];
            }
        }
        else if (SelectedItem is System.IO.FileInfo)
        {

        }
    }

    /// <summary>
    /// A listbox item container that represents a fake directory, such as <c>..</c>.
    /// </summary>
    public class FauxDirectory
    {
        /// <summary>
        /// The name of the directory.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="name">The name to display.</param>
        public FauxDirectory(string name) =>
            Name = name;
    }

    /// <summary>
    /// A listbox item container that represents a highlighted file.
    /// </summary>
    public class HighlightedExtFile
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="name">The name to display.</param>
        public HighlightedExtFile(string name) =>
            Name = name;
    }
}
