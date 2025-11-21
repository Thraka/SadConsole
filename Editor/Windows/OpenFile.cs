using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class OpenFile : ImGuiWindowBase
{
    private FileListBox _fileListBox;
    private ImGuiList<IFileHandler>? _fileLoaders = new();
    private bool _showDocumentTypes;

    public IFileHandler SelectedLoader;
    public FileInfo SelectedFile;

    

    public OpenFile(IEnumerable<IFileHandler> fileHandlers)
    {
        Title = "Open file";
        _fileListBox = new(Directory.GetCurrentDirectory());
        _fileLoaders = new ImGuiList<IFileHandler>(0, fileHandlers);
        _showDocumentTypes = false;
    }

    public OpenFile()
    {
        Title = "Open file";
        _fileListBox = new(Directory.GetCurrentDirectory());
        _showDocumentTypes = true;
    }
    protected override void OnOpened()
    {
        base.OnOpened();

        // Reset to no document builders selected
        Core.State.DocumentBuilders.SelectedItemIndex = 0;

        if (_showDocumentTypes)
        {
            Core.State.DocumentBuilders.SelectedItemIndex = 0;
            _fileLoaders = new ImGuiList<IFileHandler>(0, Core.State.DocumentBuilders.SelectedItem.GetLoadHandlers());
        }
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(550, -1));
            //ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, 300));

            if (ImGui.BeginPopupModal(Title, ref IsOpen))
            {
                if (_showDocumentTypes)
                {
                    ImGui.Text("Document Type:");
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

                    if (ImGui.ListBox("##doctypes",
                                      ref Core.State.DocumentBuilders.SelectedItemIndex,
                                      Core.State.DocumentBuilders.Names,
                                      Core.State.DocumentBuilders.Count, 3))
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

                if (ImGuiWindowBase.DrawButtons(out DialogResult, _fileListBox.HighlightedItem == null || _fileListBox.IsHighlightedItemDirectory, "Cancel", "Open"))
                {
                    if (DialogResult)
                    {
                        SelectedLoader = _fileLoaders.SelectedItem!;
                        SelectedFile = (FileInfo)_fileListBox.HighlightedItem!;
                    }

                    Close();
                }

                ImGui.EndPopup();
            }
        }
        else
        {
            OnClosed();
            ImGuiCore.GuiComponents.Remove(this);
        }
    }
}
