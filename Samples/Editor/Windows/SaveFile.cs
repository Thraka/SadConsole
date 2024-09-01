using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class SaveFile : ImGuiWindow
{
    private FileListBox _fileListBox;
    public Model.Document? Document;
    private IFileHandler[] _fileLoaders;
    private string[] _fileLoadersNames;
    private int _fileLoaderSelectedIndex;
    private string _selectedFileName = "";

    
    public SaveFile()
    {
        Title = "Save file";
        _fileListBox = new(Directory.GetCurrentDirectory());

        //_fileListBox.ItemSelected += (s, e) => selectedEvent = true;
        //_fileListBox.ItemHighlighted += (s, e) => highlightedEvent = true;
        //_fileListBox.ChangeDirectory += (s, e) => { changedDirEvent = true; counter++; };
    }

    public void Show(Model.Document document)
    {
        IsOpen = true;
        Document = document;

        _fileLoaders = Document.GetSaveHandlers().ToArray();
        _fileLoadersNames = _fileLoaders.Select(f => f.FriendlyName).ToArray();
        _fileLoaderSelectedIndex = 0;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiExt.CenterNextWindow();
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
                    ImGui.ListBox("##handlers", ref _fileLoaderSelectedIndex, _fileLoadersNames, _fileLoadersNames.Length, 10);

                    // Draw right-side file list box;
                    ImGui.TableSetColumnIndex(1);
                    _fileListBox.SearchPattern = string.Join(';', _fileLoaders[_fileLoaderSelectedIndex].ExtensionsSaving.Select(ex => $"*.{ex}"));
                    _fileListBox.Begin("listbox", new(-1, 300));
                    if (_fileListBox.Draw(out bool fileSelected, out bool fileHighlighted))
                    {
                        if (!_fileListBox.IsSelectedItemDirectory)
                            _selectedFileName = _fileListBox.HighlightedItem!.Name;
                    }
                    _fileListBox.End();

                    ImGui.EndTable();
                }
                ImGui.Separator();

                ImGui.Text("File name: ");
                ImGui.SameLine();
                ImGui.InputText("##filename", ref _selectedFileName, 200);

                //ImGui.GetWindowDrawList().AddRectFilled(boxStart, boxEnd, Color.Blue.PackedValue);
                //ImGui.SameLine();


                // Draw final row of dialog buttons
                ImGui.Separator();
                if (ImGui.Button("Cancel")) { DialogResult = false; IsOpen = false; }

                // Right-align button
                float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
                ImGui.SameLine(ImGui.GetWindowWidth() - pos);

                ImGui.BeginDisabled(string.IsNullOrEmpty(_selectedFileName));

                if (ImGui.Button("Save"))
                {
                    string file = Path.Combine(_fileListBox.CurrentDirectory.FullName, _selectedFileName);
                    object savableInstance = Document!.DehydrateToFileHandler(_fileLoaders[_fileLoaderSelectedIndex], file);
                    _fileLoaders[_fileLoaderSelectedIndex].Save(savableInstance, file);
                    // TODO: Support editing palettes and saving them
                    //if (Document.HasPalette)
                    //    Document.Palette.Save(file + ".pal");
                    DialogResult = true;
                    IsOpen = false;
                }

                ImGui.EndDisabled();

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
