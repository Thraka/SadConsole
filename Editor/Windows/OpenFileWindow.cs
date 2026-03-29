using System.Diagnostics.CodeAnalysis;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class OpenFileWindow
{
    protected class Instance : ImGuiObjectBase
    {
        private FileListBox _fileListBox;
        private ImGuiList<IFileHandler>? _fileLoaders = new();
        private bool _showDocumentTypes;

        private IFileHandler? _selectedLoader;
        private FileInfo? _selectedFile;
        private Action<IFileHandler, FileInfo> _onFileSelected;
        private Action? _onCancelled;
        private bool _firstShow = true;

        public Instance(string rootFolder, IEnumerable<IFileHandler> fileLoaders, Action<IFileHandler, FileInfo> onFileSelected, Action? onCancelled)
        {
            _fileListBox = new(rootFolder);
            _fileLoaders = new ImGuiList<IFileHandler>(0, fileLoaders);
            _onFileSelected = onFileSelected;
            _onCancelled = onCancelled;

            _showDocumentTypes = false;
        }

        public Instance(string rootFolder, Action<IFileHandler, FileInfo> onFileSelected, Action? onCancelled)
        {
            Core.State.DocumentBuilders.SelectedItemIndex = 0;
            _fileListBox = new(rootFolder);
            _fileLoaders = new ImGuiList<IFileHandler>(0, Core.State.DocumentBuilders.SelectedItem!.GetLoadHandlers());
            _onFileSelected = onFileSelected;
            _onCancelled = onCancelled;

            _showDocumentTypes = true;
        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Open file"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindowOnAppearing(new System.Numerics.Vector2(550, -1));
            //ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, 300));

            if (ImGui.BeginPopupModal("Open file"u8))
            {
                if (_showDocumentTypes)
                {
                    ImGui.Text("Document Type:");
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

                    if (ImGui.ListBox("##doctypes",
                                        ref Core.State.DocumentBuilders.SelectedItemIndex,
                                        Core.State.DocumentBuilders.Names,
                                        Core.State.DocumentBuilders.Count))
                    {
                        _fileLoaders = new ImGuiList<IFileHandler>(0, Core.State.DocumentBuilders.SelectedItem.GetLoadHandlers());
                    }

                    ImGui.Separator();
                }

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
                    if (_fileLoaders.IsItemSelected())
                    {
                        _fileListBox.SearchPattern = string.Join(';', _fileLoaders.SelectedItem.ExtensionsLoading.Select(ex => $"*.{ex}"));
                        _fileListBox.Begin("listbox", new(-1, 300));
                        _fileListBox.Draw();
                        _fileListBox.End();
                    }

                    ImGui.EndTable();
                }

                ImGui.Separator();

                if (ImGuiSC.WindowDrawButtons(out bool dialogResult, _fileListBox.HighlightedItem == null || _fileListBox.IsHighlightedItemDirectory, "Cancel", "Open"))
                {
                    if (dialogResult)
                        _onFileSelected(_fileLoaders.SelectedItem!, (FileInfo)_fileListBox.HighlightedItem!);
                    else
                        _onCancelled?.Invoke();

                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                }
                ImGui.EndPopup();
            }
        }
    }

    public static void Show(ImGuiRenderer renderer, IEnumerable<IFileHandler> fileHandlers, Action<IFileHandler, FileInfo> onFileSelected, Action? onCancelled)
    {
        Instance instance = new(Core.State.RootFolder, fileHandlers, onFileSelected, onCancelled);
        renderer.UIObjects.Add(instance);
    }

    public static void Show(ImGuiRenderer renderer, Action<IFileHandler, FileInfo> onFileSelected, Action? onCancelled)
    {
        Instance instance = new(Core.State.RootFolder, onFileSelected, onCancelled);
        renderer.UIObjects.Add(instance);
    }
}
