using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class SaveFileWindow
{
    protected class Instance : ImGuiObjectBase
    {
        private FileListBox _fileListBox;
        private ImGuiList<IFileHandler> _fileLoaders = new();
        private string _selectedFileName = "";
        private bool _useCompression;
        private object _saveTarget;

        private Action<IFileHandler, string>? _onFileSaved;
        private Action? _onCancelled;

        private bool _confirmOverwrite;
        private string _confirmOverwriteFile = "";
        private bool _firstShow = true;

        public Instance(string rootFolder, Document document, Action<IFileHandler, string>? onFileSaved, Action? onCancelled)
        {
            _fileListBox = new(rootFolder);
            _saveTarget = document;
            _fileLoaders = new ImGuiList<IFileHandler>(0, document.GetSaveHandlers());
            _onFileSaved = onFileSaved;
            _onCancelled = onCancelled;
        }

        public Instance(string rootFolder, object alternativeObject, IEnumerable<IFileHandler> fileHandlers, Action<IFileHandler, string>? onFileSaved, Action? onCancelled)
        {
            _fileListBox = new(rootFolder);
            _saveTarget = alternativeObject;
            _fileLoaders = new ImGuiList<IFileHandler>(0, fileHandlers);
            _onFileSaved = onFileSaved;
            _onCancelled = onCancelled;
        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Save file"u8);
                _firstShow = false;
            }

            // TODO: This is written in the context of we only have 1 extension.
            ImGuiSC.CenterNextWindowOnAppearing(new System.Numerics.Vector2(550, -1));

            if (ImGui.BeginPopupModal("Save file"u8))
            {
                if (ImGui.BeginTable("table1", 2, ImGuiTableFlags.BordersInnerV))
                {
                    ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X / 3);
                    ImGui.TableSetupColumn("two");

                    // Draw left-side file handlers
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("File Types");
                    ImGui.SetNextItemWidth(-1);
                    ImGui.ListBox("##handlers", ref _fileLoaders.SelectedItemIndex, _fileLoaders.Names, _fileLoaders.Count, 10);

                    // Draw right-side file list box;
                    ImGui.TableSetColumnIndex(1);
                    _fileListBox.SearchPattern = string.Join(';', _fileLoaders.SelectedItem.ExtensionsSaving.Select(ex => $"*.{ex}"));
                    _fileListBox.Begin("listbox", new(-1, 300));
                    if (_fileListBox.Draw(out bool fileSelected, out bool fileHighlighted))
                    {
                        if (!_fileListBox.IsSelectedItemDirectory)
                            _selectedFileName = Path.GetFileNameWithoutExtension(_fileListBox.HighlightedItem!.Name);
                    }
                    _fileListBox.End();

                    ImGui.EndTable();
                }
                ImGui.Separator();

                if (_fileLoaders.SelectedItem.DefaultSaveOptions.ShowCompressionToggle)
                    ImGui.Checkbox("Compress File", ref _useCompression);

                ImGui.AlignTextToFramePadding();
                ImGui.Text("File name: ");
                ImGui.SameLine();

                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - ImGui.GetStyle().GetWidthOfItems($".{_fileLoaders.SelectedItem.ExtensionsSaving[0]}"));
                ImGui.InputText("##filename", ref _selectedFileName, 200);
                ImGui.SameLine();
                ImGui.Text($".{_fileLoaders.SelectedItem.ExtensionsSaving[0]}");

                // Draw final row of dialog buttons
                ImGui.Separator();
                bool isClosing = false;

                if (ImGuiSC.WindowDrawButtons(out bool dialogResult, string.IsNullOrEmpty(_selectedFileName), "Cancel", "Save"))
                {
                    if (dialogResult)
                    {
                        string file = Path.Combine(_fileListBox.CurrentDirectory.FullName, _selectedFileName);

                        if (File.Exists(_fileLoaders.SelectedItem.GetFileWithValidExtensionForSave(file)))
                        {
                            ImGui.OpenPopup("confirm_overwrite"u8);
                            _confirmOverwriteFile = file;
                        }
                        else
                        {
                            _fileLoaders.SelectedItem.Save(_saveTarget, file, _useCompression);
                            _onFileSaved?.Invoke(_fileLoaders.SelectedItem, file);
                            isClosing = true;
                        }
                    }
                    else
                    {
                        _onCancelled?.Invoke();
                        isClosing = true;
                    }
                }

                if (ImGui.BeginPopup("confirm_overwrite"u8))
                {
                    ImGui.Text("File exists, overwrite the file?"u8);
                    if (ImGuiSC.WindowDrawButtons(out bool overwriteResult, false, "No", "Yes"))
                    {
                        if (overwriteResult)
                        {
                            _fileLoaders.SelectedItem.Save(_saveTarget, _confirmOverwriteFile, _useCompression);
                            _onFileSaved?.Invoke(_fileLoaders.SelectedItem, _confirmOverwriteFile);
                        }
                        else
                            _onCancelled?.Invoke();

                        isClosing = true;
                        ImGui.CloseCurrentPopup(); // confirm_overwrite
                    }
                    ImGui.EndPopup();
                }

                if (isClosing)
                {
                    ImGui.CloseCurrentPopup(); // save file
                    renderer.UIObjects.Remove(this);
                }

                ImGui.EndPopup();
            }
        }
    }

    public static void Show(ImGuiRenderer renderer, Document document, Action<IFileHandler, string>? onFileSaved = null, Action? onCancelled = null)
    {
        Instance instance = new(Core.State.RootFolder, document, onFileSaved, onCancelled);
        renderer.UIObjects.Add(instance);
    }

    public static void Show(ImGuiRenderer renderer, object alternativeObject, IEnumerable<IFileHandler> fileHandlers, Action<IFileHandler, string>? onFileSaved = null, Action? onCancelled = null)
    {
        Instance instance = new(Core.State.RootFolder, alternativeObject, fileHandlers, onFileSaved, onCancelled);
        renderer.UIObjects.Add(instance);
    }
}
