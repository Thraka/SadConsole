using System.Numerics;
using Hexa.NET.ImGui;

namespace SadConsole.Editor.Windows;

public class FilePicker
{
    static readonly Dictionary<object, FilePicker> _filePickers = new Dictionary<object, FilePicker>();

    public string RootFolder;
    public string CurrentFolder;
    public string SelectedFile;
    public List<string> AllowedExtensions;
    public bool OnlyAllowFolders;

    public static FilePicker GetFolderPicker(object o, string startingPath)
        => GetFilePicker(o, startingPath, null, true);

    public static FilePicker GetFilePicker(object o, string startingPath, string searchFilter = null, bool onlyAllowFolders = false)
    {
        if (File.Exists(startingPath))
        {
            startingPath = new FileInfo(startingPath).DirectoryName;
        }
        else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
        {
            startingPath = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(startingPath))
                startingPath = AppContext.BaseDirectory;
        }

        if (!_filePickers.TryGetValue(o, out FilePicker fp))
        {
            fp = new FilePicker();
            fp.RootFolder = startingPath;
            fp.CurrentFolder = startingPath;
            fp.OnlyAllowFolders = onlyAllowFolders;

            if (searchFilter != null)
            {
                if (fp.AllowedExtensions != null)
                    fp.AllowedExtensions.Clear();
                else
                    fp.AllowedExtensions = new List<string>();

                fp.AllowedExtensions.AddRange(searchFilter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }

            _filePickers.Add(o, fp);
        }

        return fp;
    }

    public static void RemoveFilePicker(object o) => _filePickers.Remove(o);

    public bool Draw()
    {
        ImGui.Text("Current Folder: " + Path.GetFileName(RootFolder) + CurrentFolder.Replace(RootFolder, ""));
        bool result = false;

        if (ImGui.BeginChild(1, new Vector2(400, 400)))
        {
            var di = new DirectoryInfo(CurrentFolder);
            if (di.Exists)
            {
                if (di.Parent != null && CurrentFolder != RootFolder)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.PackedValue);
                    if (ImGui.Selectable("../", false, ImGuiSelectableFlags.NoAutoClosePopups))
                        CurrentFolder = di.Parent.FullName;

                    ImGui.PopStyleColor();
                }

                var fileSystemEntries = GetFileSystemEntries(di.FullName);
                foreach (var fse in fileSystemEntries)
                {
                    if (Directory.Exists(fse))
                    {
                        var name = Path.GetFileName(fse);
                        ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.PackedValue);
                        if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.NoAutoClosePopups))
                            CurrentFolder = fse;
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        var name = Path.GetFileName(fse);
                        bool isSelected = SelectedFile == fse;
                        if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.NoAutoClosePopups))
                            SelectedFile = fse;

                        if (ImGuiP.IsMouseDoubleClicked(0))
                        {
                            result = true;
                            ImGui.CloseCurrentPopup();
                        }
                    }
                }
            }
        }
        ImGui.EndChild();


        if (ImGui.Button("Cancel"))
        {
            result = false;
            ImGui.CloseCurrentPopup();
        }

        if (OnlyAllowFolders)
        {
            ImGui.SameLine();
            if (ImGui.Button("Open"))
            {
                result = true;
                SelectedFile = CurrentFolder;
                ImGui.CloseCurrentPopup();
            }
        }
        else if (SelectedFile != null)
        {
            ImGui.SameLine();
            if (ImGui.Button("Open"))
            {
                result = true;
                ImGui.CloseCurrentPopup();
            }
        }

        return result;
    }

    bool TryGetFileInfo(string fileName, out FileInfo realFile)
    {
        try
        {
            realFile = new FileInfo(fileName);
            return true;
        }
        catch
        {
            realFile = null;
            return false;
        }
    }

    List<string> GetFileSystemEntries(string fullName)
    {
        var files = new List<string>();
        var dirs = new List<string>();

        foreach (var fse in Directory.GetFileSystemEntries(fullName, ""))
        {
            if (Directory.Exists(fse))
            {
                dirs.Add(fse);
            }
            else if (!OnlyAllowFolders)
            {
                if (AllowedExtensions != null)
                {
                    var ext = Path.GetExtension(fse);
                    if (AllowedExtensions.Contains(ext))
                        files.Add(fse);
                }
                else
                {
                    files.Add(fse);
                }
            }
        }

        var ret = new List<string>(dirs);
        ret.AddRange(files);

        return ret;
    }

}
