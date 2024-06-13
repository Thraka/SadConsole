using System.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace ImGuiNET;

public class FileListBox
{
    public event EventHandler? ItemHighlighted;
    public event EventHandler? ItemSelected;
    public event EventHandler? ChangeDirectory;

    // Settings
    public bool AllowNavigation { get; set; } = true;
    public bool ShowDirectories { get; set; } = true;
    public bool ShowFiles { get; set; } = true;
    public bool DoubleClickToSelect { get; set; } = true;
    public Color DirectoryColor { get; set; } = Color.Yellow;
    public Color FileColor { get; set; } = Color.White;
    public string SearchPattern
    {
        get => _searchPattern;
        set
        {
            if (_searchPattern == value) return;

            _searchPattern = string.IsNullOrEmpty(value) ? "*.*" : value;
            NavigateTo(CurrentDirectory.FullName);
        }
    }
    public Dictionary<string, Color> ColoredExtensions { get; set; } = new Dictionary<string, Color>();
    public bool UseEvents { get; set; } = true;

    // Selected item
    public FileSystemInfo? SelectedItem { get; set; }
    public bool IsSelectedItemDirectory => SelectedItem != null
                                            ? SelectedItem is DirectoryInfo
                                            : false;

    // Highlighted item
    public FileSystemInfo? HighlightedItem { get; set; }
    public bool IsHighlightedItemDirectory => HighlightedItem != null
                                                ? HighlightedItem is DirectoryInfo
                                                : false;
    private ItemType? _activeItem;

    // Public state
    public DirectoryInfo CurrentDirectory => _currentFolder;

    // Private state
    private ItemType[] _currentFolderItems = [];
    private bool _isRoot;

    private DirectoryInfo _rootFolder;
    private DirectoryInfo _currentFolder;
    private ItemType _parentDirectoryItem;
    private Task _directoryLoader;
    private string _searchPattern = "*.*";

    public FileListBox(string rootFolder)
    {
        _rootFolder = new DirectoryInfo(rootFolder);

        if (!_rootFolder.Exists) throw new DirectoryNotFoundException($"Folder not found: {rootFolder}");

        NavigateTo(_rootFolder.FullName);
    }

    public FileListBox(string rootFolder, string currentFolder)
    {
        _rootFolder = new DirectoryInfo(rootFolder);
        DirectoryInfo currentFolderToCheck = new(currentFolder);

        if (!_rootFolder.Exists) throw new DirectoryNotFoundException($"Folder not found: {rootFolder}");
        if (!currentFolderToCheck.Exists) throw new DirectoryNotFoundException($"Folder not found: {currentFolder}");
        if (!currentFolderToCheck.FullName.StartsWith(_rootFolder.FullName)) throw new Exception("Current folder must be a sub folder of the root folder");

        NavigateTo(currentFolderToCheck.FullName);
    }

    /// <summary>
    /// Draws the list box.
    /// </summary>
    public void Draw() =>
        Draw(out _, out _);

    public void Begin(string id) =>
        ImGui.BeginChild(id);

    public void Begin(string id, Vector2 size, bool showBorder = false, ImGuiWindowFlags flags = ImGuiWindowFlags.None) =>
        ImGui.BeginChild(id, size, showBorder, flags);

    public void End() =>
        ImGui.EndChild();

    /// <summary>
    /// Draws the list box.
    /// </summary>
    /// <param name="itemHighlighted">True when an item is single-clicked and <see cref="DoubleClickToSelect"/> is true.</param>
    /// <returns>True when an item is selected or highlighted.</returns>
    public bool Draw(out bool itemSelected, out bool itemHighlighted)
    {
        itemSelected = false;
        itemHighlighted = false;

        // Loading directory
        if (!_directoryLoader.IsCompleted)
        {
            ImGui.Text("Loading...");
        }
        else
        {
            // Draw the ..\ folder
            if (_currentFolder.Parent != null && !_currentFolder.FullName.Equals(_rootFolder.FullName))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, _parentDirectoryItem.Color.PackedValue);
                if (ImGui.Selectable(_parentDirectoryItem.Name, false, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (DoubleClickToSelect && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        NavigateTo(_currentFolder.Parent.FullName);

                    else if (!DoubleClickToSelect)
                        NavigateTo(_currentFolder.Parent.FullName);

                }
                ImGui.PopStyleColor();
            }

            // If we're at the root, but there are no dirs nor files, alert the user
            if (_currentFolder.FullName.Equals(_rootFolder.FullName) && _currentFolderItems.Length == 0)
            {
                ImGui.Text("No files match the specified criteria.");
            }

            // Draw items
            foreach (ItemType item in _currentFolderItems)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, item.Color.PackedValue);

                ImGuiSelectableFlags flags = ImGuiSelectableFlags.DontClosePopups;
                if (DoubleClickToSelect) flags |= ImGuiSelectableFlags.AllowDoubleClick;

                // Draw item. If selected...
                if (ImGui.Selectable(item.Name, item == _activeItem, flags))
                {
                    // When doubleclick is enabled, you can highlight (single click).
                    if (DoubleClickToSelect)
                    {
                        // Item double clicked, navigate to directory or select it item/dir
                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        {
                            if (item.IsDirectory && AllowNavigation)
                            {
                                NavigateTo(item.FullName);
                            }
                            else
                            {
                                itemSelected = true;
                                OnSelected(item);
                            }
                        }

                        // Single clicked, item is highlighted
                        else
                        {
                            itemHighlighted = true;
                            OnHighlighted(item);
                        }
                    }

                    // Single click to select means no highlighting. You click, it selects.
                    else
                    {
                        if (item.IsDirectory && AllowNavigation)
                        {
                            NavigateTo(item.FullName);
                        }
                        else
                        {
                            itemSelected = true;
                            OnSelected(item);
                        }
                    }
                }
                ImGui.PopStyleColor();
            }
        }

        return itemSelected || itemHighlighted;
    }

    [MemberNotNull(nameof(_directoryLoader), nameof(_currentFolder), nameof(_parentDirectoryItem))]
    private void NavigateTo(string path)
    {
        // Clear highlighted
        HighlightedItem = null;
        SelectedItem = null;
        _activeItem = null;

        // Load with the latest settings
        _parentDirectoryItem = ItemType.AsDirectory($"..{Path.DirectorySeparatorChar}", "", DirectoryColor);

        // Move directories
        _currentFolder = new(path);

        // Check if it exists
        if (!_currentFolder.Exists)
            throw new DirectoryNotFoundException($"Folder not found: {_currentFolder}");

        _directoryLoader = Task.Run(LoadDirectory).ContinueWith((t) => OnDirectoryChanged(), TaskContinuationOptions.ExecuteSynchronously);
    }

    private void LoadDirectory()
    {
        List<ItemType> items = new();
        _activeItem = null;
        HighlightedItem = null;
        SelectedItem = null;

        if (ShowDirectories)
        {
            foreach (DirectoryInfo item in _currentFolder.EnumerateDirectories())
                items.Add(ItemType.AsDirectory(item.Name, item.FullName, DirectoryColor));
        }

        if (ShowFiles)
        {
            string[] fileSearches = SearchPattern.Split(';');

            if (fileSearches.Length == 1 && fileSearches[0] == "*.*")
                fileSearches[0] = "*";

            List<ItemType> fileItemsOnly = new();

            foreach (string pattern in fileSearches)
            {
                foreach (FileInfo item in _currentFolder.EnumerateFiles(pattern, SearchOption.TopDirectoryOnly))
                {
                    Color itemColor = FileColor;
                    string extension = item.Extension.ToLower();

                    if (ColoredExtensions.TryGetValue(extension, out Color value))
                        itemColor = value;

                    fileItemsOnly.Add(ItemType.AsFile(item.Name, item.FullName, itemColor));
                }
            }

            items.AddRange(fileItemsOnly.OrderBy(item => item.Name));
        }

        _currentFolderItems = items.Count != 0 ? items.ToArray() : Array.Empty<ItemType>();
    }

    private void OnDirectoryChanged()
    {
        if (UseEvents)
            ChangeDirectory?.Invoke(this, EventArgs.Empty);
    }

    private void OnHighlighted(ItemType item)
    {
        _activeItem = item;

        if (item.IsDirectory)
            HighlightedItem = new DirectoryInfo(item.FullName);
        else
            HighlightedItem = new FileInfo(item.FullName);

        if (UseEvents)
            ItemHighlighted?.Invoke(this, EventArgs.Empty);
    }

    private void OnSelected(ItemType item)
    {
        _activeItem = item;

        if (item.IsDirectory)
            SelectedItem = new DirectoryInfo(item.FullName);
        else
            SelectedItem = new FileInfo(item.FullName);

        if (UseEvents)
            ItemSelected?.Invoke(this, EventArgs.Empty);
    }

    private class ItemType
    {
        public bool IsDirectory { get; }
        public string Name { get; }
        public string FullName { get; }
        public Color Color { get; }

        private ItemType(string name, string fullName, bool isDirectory, Color color) =>
            (Name, FullName, IsDirectory, Color) = (name, fullName, isDirectory, color);

        public static ItemType AsFile(string name, string fullName, Color color) =>
            new(name, fullName, false, color);

        public static ItemType AsDirectory(string name, string fullName, Color color) =>
            new(name, fullName, true, color);
    }
}
