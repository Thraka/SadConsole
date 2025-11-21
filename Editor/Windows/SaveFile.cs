using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class SaveFile : ImGuiWindowBase
{
    private FileListBox _fileListBox;
    private ImGuiList<IFileHandler> _fileLoaders = new();
    private string _selectedFileName = "";
    private Document _document;
    private bool _useCompression;

    public SaveFile(Document document)
    {
        Title = "Save file";
        _fileListBox = new(Directory.GetCurrentDirectory());
        _document = document;

        _fileLoaders = new ImGuiList<IFileHandler>(0, _document.GetSaveHandlers());
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        // TODO: This is written in the context of we only have 1 extension.
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(550, -1));
            //ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, 300));

            if (ImGui.BeginPopupModal(Title, ref IsOpen))
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
                if (ImGuiWindowBase.DrawButtons(out DialogResult, string.IsNullOrEmpty(_selectedFileName), "Cancel", "Save"))
                {
                    if (DialogResult)
                    {
                        string file = Path.Combine(_fileListBox.CurrentDirectory.FullName, _selectedFileName);

                        if (File.Exists(_fileLoaders.SelectedItem.GetFileWithValidExtensionForSave(file)))
                        {
                            PromptWindow window = new("File exists, overwrite the file?", "Save", "Yes", "No");
                            window.Closed += (s, e) =>
                            {
                                if (((PromptWindow)s).DialogResult)
                                {
                                    _fileLoaders.SelectedItem.Save(_document, file, _useCompression);
                                }
                            };
                            window.Open();
                        }
                        else
                            _fileLoaders.SelectedItem.Save(_document, file, _useCompression);
                    }

                    Close();
                }

                ImGui.EndPopup();
            }
        }
    }
}
