using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class OpenFile : ImGuiWindow
{
    private int _documentSelectedIndex = 0;
    private string _documentName = "";
    private FileListBox _fileListBox;
    private bool _hideDocumentTypes;
    public Model.Document? Document;
    private IFileHandler[] _fileLoaders;
    private string[] _fileLoadersNames;
    private int _fileLoaderSelectedIndex;

    public IFileHandler SelectedHandler;
    public FileInfo SelectedFile;

    public OpenFile()
    {
        Title = "Open file";
        _fileListBox = new(Directory.GetCurrentDirectory());

        //_fileListBox.ItemSelected += (s, e) => selectedEvent = true;
        //_fileListBox.ItemHighlighted += (s, e) => highlightedEvent = true;
        //_fileListBox.ChangeDirectory += (s, e) => { changedDirEvent = true; counter++; };
    }

    public void Show()
    {
        IsOpen = true;
        _hideDocumentTypes = false;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }

    public void Show(Model.Document document)
    {
        IsOpen = true;
        _hideDocumentTypes = true;
        Document = document;

        _fileLoaders = Document.GetLoadHandlers().ToArray();
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
                if (!_hideDocumentTypes)
                {
                    ImGui.Text("Document Type:");
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    if (DocumentTypeListControl.DrawListBox("##Document Type", 3, ref _documentSelectedIndex, ref Document))
                    {
                        _fileLoaders = Document.GetLoadHandlers().ToArray();
                        _fileLoadersNames = _fileLoaders.Select(f => f.FriendlyName).ToArray();
                        _fileLoaderSelectedIndex = 0;
                    }
                    ImGui.Separator();
                }

                if (Document != null)
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
                        _fileListBox.SearchPattern = string.Join(';', _fileLoaders[_fileLoaderSelectedIndex].ExtensionsLoading.Select(ex => $"*.{ex}"));
                        _fileListBox.Begin("listbox", new(-1, 300));
                        _fileListBox.Draw();
                        _fileListBox.End();

                        ImGui.EndTable();
                    }
                    
                    //ImGui.GetWindowDrawList().AddRectFilled(boxStart, boxEnd, Color.Blue.PackedValue);
                    //ImGui.SameLine();
                }

                // Draw final row of dialog buttons
                ImGui.Separator();
                if (ImGui.Button("Cancel")) { DialogResult = false; IsOpen = false; }

                // Right-align button
                float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
                ImGui.SameLine(ImGui.GetWindowWidth() - pos);

                if (_fileListBox.SelectedItem != null && _fileListBox.IsSelectedItemDirectory == false)
                {
                    ImGui.BeginDisabled();
                    ImGui.Button("Open");
                    ImGui.EndDisabled();
                }
                else
                {
                    if (ImGui.Button("Open"))
                    {
                        SelectedHandler = _fileLoaders[_fileLoaderSelectedIndex];
                        SelectedFile = (FileInfo)_fileListBox.HighlightedItem!;
                        DialogResult = Document!.HydrateFromFileHandler(SelectedHandler, SelectedFile.FullName);
                        if (!DialogResult)
                            ImGuiCore.Alert("Unable to load file.");
                        IsOpen = false;
                    }
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
