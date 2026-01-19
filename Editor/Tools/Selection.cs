using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool
{
    private static CellSurface? _clipboardSurface;
    private Point _firstPoint;
    private Point _secondPoint;
    private Point _pasteOffset;
    private ShapeParameters _selectionBoxShape;
    private CellSurface _selectionSurface;
    private States _state = States.None;
    string _saveBlueprintFileName = string.Empty;
    string _saveBlueprintName = string.Empty;
    bool _pasteIgnoreEmpty = false;
    bool _systemClipboardIgnoreSpace = true;
    bool _systemClipboardUseDefaultBackground = true;
    bool _systemClipboardUseDefaultForeground = true;

    public string Title => "\uefa4 Selection\\Blueprints";

    public string Description =>
        """
        Selects an area of the document for cloning.

        To cancel the selection press the ESC key or right-click on the selection area.
        """;

    public Selection()
    {
        _selectionBoxShape = ShapeParameters.CreateStyledBoxFilled(
            ICellSurface.ConnectedLineThick,
            new ColoredGlyph(Color.White, Color.Black.SetAlpha(100)),
            new ColoredGlyph(Color.White, Color.Black.SetAlpha(50)));
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGui.Checkbox("Ignore empty cells on paste"u8, ref _pasteIgnoreEmpty);

        ImGui.BeginDisabled(Core.State.Blueprints.Count == 0);
        if (ImGui.Button("Load Blueprint"u8))
        {
            BlueprintImport? window = new();
            window.Open();
            window.Closed += (sender, e) =>
            {
                if (window.DialogResult)
                {
                    CancelPaste(document, false);
                    _selectionSurface = Core.State.Blueprints.SelectedItem!.GetSurface();
                    _state = States.Pasting;

                    window = null;
                }
            };
        }
        ImGui.EndDisabled();

        if (ImGui.Button("Import Surface"))
        {
            Windows.OpenFile fileLoader = new([new FileHandlers.SurfaceDocument(), new FileHandlers.SurfaceFile()]);
            fileLoader.Closed += FileLoader_Closed;
            fileLoader.Open();
            
        }

        if (ImGui.Button("Import Surface from Image"))
        {
            Windows.ImageToAsciiWindow imageToAscii = new(document.EditingSurfaceFont, document.EditingSurfaceFont.GetFontSize(IFont.Sizes.One));
            imageToAscii.Closed += ImageToAscii_Closed;
            imageToAscii.Open();
        }

        ImGui.BeginDisabled(_clipboardSurface == null);
        if (ImGui.Button("Paste surface from Clipboard"u8))
        {
            CancelPaste(document, false);
            _selectionSurface = _clipboardSurface!;
            _state = States.Pasting;
        }
        ImGui.EndDisabled();

        ImGui.SeparatorText("System Clipboard"u8);
        ImGui.Checkbox("Convert space to empty"u8, ref _systemClipboardIgnoreSpace);
        ImGui.Checkbox("Use Document Foreground"u8, ref _systemClipboardUseDefaultForeground);
        ImGui.Checkbox("Use Document Background"u8, ref _systemClipboardUseDefaultBackground);
        if (ImGui.Button("Create from clipboard text"u8))
        {
            CancelPaste(document, false);

            string convertedString;

            unsafe
            {
                byte* clipboardString = ImGui.GetClipboardText();
                convertedString = System.Runtime.InteropServices.Marshal.PtrToStringUTF8((nint)clipboardString) ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(convertedString))
            {
                if (_systemClipboardIgnoreSpace)
                    convertedString = convertedString.Replace(' ', '\0');

                string[] lines = convertedString.Replace("\r", string.Empty).Split('\n');
                int biggestLineWidth = lines.Select(line => line.Length).Max();
                _clipboardSurface = new CellSurface(biggestLineWidth, lines.Length);

                if (_systemClipboardUseDefaultBackground)
                    _clipboardSurface.DefaultBackground = document.EditingSurface.Surface.DefaultBackground;

                if (_systemClipboardUseDefaultForeground)
                    _clipboardSurface.DefaultForeground = document.EditingSurface.Surface.DefaultForeground;

                _clipboardSurface.Clear();

                int curY = 0;

                //_clipboardSurface.UsePrintProcessor = false;
                foreach (var line in lines)
                {
                    _clipboardSurface.Print(0, curY, line);
                    curY++;
                }
                _selectionSurface = _clipboardSurface;
                _state = States.Pasting;
            }
        }
    }

    private void ImageToAscii_Closed(object? sender, EventArgs e)
    {
        if (sender is not ImageToAsciiWindow dialog) return;
        if (!dialog.DialogResult) return;
        if (dialog.ResultSurface is CellSurface surface)
        {
            _clipboardSurface = surface;
            ClearState();
            _state = States.PastingMultiple;
            _selectionSurface = _clipboardSurface;
        }
    }

    private void FileLoader_Closed(object? sender, EventArgs e)
    {
        OpenFile dialog = (OpenFile)sender!;

        if (!dialog.DialogResult) return;

        object? result = dialog.SelectedLoader.Load(dialog.SelectedFile.FullName);

        if (result != null)
        {
            CellSurface? surface = null;

            if (result is DocumentSurface doc)
                surface = (CellSurface)doc.EditingSurface.Surface;

            else if (result is CellSurface resultSurface)
                surface = resultSurface;

            if (surface is not null)
            {
                _clipboardSurface = surface;
                ClearState();
                _state = States.PastingMultiple;
                _selectionSurface = _clipboardSurface;
            }
        }
        dialog.Closed -= FileLoader_Closed;
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (_state == States.SelectionDone && ImGui.BeginPopupContextItem("selection_rightmenu"u8) || ImGuiP.IsPopupOpen("selection_rightmenu"u8))
        {
            bool _createBlueprint = false;
            if (ImGui.Selectable("Copy to Clipboard"u8))
            {
                CopySurface(document);
                _clipboardSurface = _selectionSurface;
                ClearState();
                ImGui.CloseCurrentPopup();
            }

            if (ImGui.Selectable("Move"u8))
            {
                CopySurface(document);
                ClearSurface(document);
                _state = States.Pasting;

                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Clone"u8))
            {
                CopySurface(document);
                _state = States.Pasting;

                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Clone + Stamp"u8))
            {
                CopySurface(document);
                _state = States.PastingMultiple;

                ImGui.CloseCurrentPopup();
            }

            ImGui.Separator();
            ImGui.Text("Create Document"u8);
            ImGui.Indent();
            if (ImGui.Selectable("Surface Document"u8))
            {
                ImGui.Indent();
                CopyToSurfaceDocument(document);
                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Layered Document"u8))
            {
                ImGui.Indent();
                CopyToLayeredDocument(document);
                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Animated Document"u8))
            {
                ImGui.Indent();
                CopyToAnimatedDocument(document);
                ImGui.CloseCurrentPopup();
            }
            ImGui.Unindent();

            if (ImGui.Selectable("Create Blueprint"u8))
            {
                _saveBlueprintFileName = string.Empty;
                _saveBlueprintName = string.Empty;
                CopySurface(document);
                ImGui.CloseCurrentPopup();
                _createBlueprint = true;
            }

            ImGui.Separator();
            if (ImGui.Selectable("Erase"u8))
            {
                ClearSurface(document);
                document.VisualLayerToolMiddle.Surface.Clear();
                ClearState();

                ImGui.CloseCurrentPopup();
            }

            if (ImGui.Selectable("Cancel"u8))
            {
                CancelPaste(document, false);
                
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();

            if (_createBlueprint)
                ImGui.OpenPopup("create_blueprint"u8);

            return;
        }

        if (ImGui.BeginPopupModal("create_blueprint"u8))
        {
            ImGui.SetNextItemWidth(400);
            
            ImGui.InputText("Name"u8, ref _saveBlueprintName, 50);
            ImGui.InputText("Filename"u8, ref _saveBlueprintFileName, 50);

            bool fileExists = File.Exists(System.IO.Path.Combine(Core.Settings.BlueprintFolder, _saveBlueprintFileName));

            if (fileExists)
            {
                ImGui.TextColored(Color.Red.ToVector4(), "File already exists. Delete?"u8);
                if (ImGui.Button("Delete"u8))
                {
                    File.Delete(System.IO.Path.Combine(Core.Settings.BlueprintFolder, _saveBlueprintFileName));
                }
            }

            bool disableAccept = string.IsNullOrEmpty(_saveBlueprintFileName.Trim()) || string.IsNullOrEmpty(_saveBlueprintName.Trim()) || fileExists;

            if (ImGuiWindowBase.DrawButtons(out bool savedClicked, disableAccept))
            {
                if (savedClicked)
                {
                    Blueprint.CreateAndSave(_saveBlueprintName, Path.Combine(Core.Settings.BlueprintFolder, _saveBlueprintFileName), _selectionSurface);
                }

                _saveBlueprintFileName = string.Empty;
                _saveBlueprintName = string.Empty;

                Core.State.LoadBlueprints();

                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        // Cancelled but left mouse finally released, exit cancelled
        if (_state == States.Cancel && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            _state = States.None;

        // Cancelled
        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape)))
        {
            CancelPaste(document);
        }

        if (_state == States.Cancel)
            return;

        else if (_state is States.Pasting or States.PastingMultiple)
        {
            if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            {
                Point pos = hoveredCellPosition - new Point(_selectionSurface.Width / 2, _selectionSurface.Height / 2);

                // Do copy
                for (int curX = 0; curX < _selectionSurface.Surface.Width; curX++)
                {
                    for (int curY = 0; curY < _selectionSurface.Surface.Height; curY++)
                    {
                        ColoredGlyphBase cell = _selectionSurface.Surface[curX, curY];
                        bool cancelCopy = _pasteIgnoreEmpty &&
                                          cell.Foreground == _selectionSurface.DefaultForeground &&
                                          cell.Background == _selectionSurface.DefaultBackground &&
                                          cell.Glyph == _selectionSurface.DefaultGlyph &&
                                          cell.Decorators != null && cell.Decorators.Count == 0;

                        if (_selectionSurface.IsValidCell(curX, curY, out int sourceIndex) &&
                            document.EditingSurface.Surface.IsValidCell(pos.X + curX, pos.Y + curY, out int destIndex))
                        {
                            _selectionSurface.Surface[sourceIndex].CopyAppearanceTo(document.EditingSurface.Surface[destIndex]);
                        }
                    }
                }

                

                document.EditingSurface.Surface.IsDirty = true;

                if (_state != States.PastingMultiple)
                {
                    CancelPaste(document, false);
                }
            }

            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape))
            {
                CancelPaste(document, false);
            }

            // Draw the floating surface
            else
            {
                if (ImGuiP.IsKeyPressed(ImGuiKey.LeftArrow))
                    _pasteOffset += Direction.Left;
                else if (ImGuiP.IsKeyPressed(ImGuiKey.RightArrow))
                    _pasteOffset += Direction.Right;

                if (ImGuiP.IsKeyPressed(ImGuiKey.UpArrow))
                    _pasteOffset += Direction.Up;
                else if (ImGuiP.IsKeyPressed(ImGuiKey.DownArrow))
                    _pasteOffset += Direction.Down;

                document.VisualLayerToolMiddle.Surface.Clear();
                document.VisualLayerToolUpper.Surface.Clear();

                Point pos = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition - new Point(_selectionSurface.Width / 2, _selectionSurface.Height / 2) + _pasteOffset;
                _selectionSurface.Copy(document.VisualLayerToolMiddle.Surface, pos.X, pos.Y);
                document.VisualLayerToolUpper.Surface.DrawBox(new Rectangle(pos.X, pos.Y, _selectionSurface.Width, _selectionSurface.Height),
                                                                            _selectionBoxShape);
                //document.VisualLayerToolUpper.Surface.Print(0, 0, pos.ToString());
            }

            return;
        }
        else if (_state is States.SelectionDone && ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            ClearState();

        // Preview
        if (_state is States.MakingSelection or States.None && ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
        {
            if (_state == States.None)
            {
                _state = States.MakingSelection;

                _firstPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
            }

            _secondPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;

            document.VisualLayerToolMiddle.Surface.Clear();
            document.VisualLayerToolMiddle.Surface.DrawBox(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                                        new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                                                                        _selectionBoxShape);
        }

        // Commit selection
        else if (_state == States.MakingSelection && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
        {
            _state = States.SelectionDone;
            ImGui.OpenPopup("selection_rightmenu"u8);
        }
    }

    private void CopyToSurfaceDocument(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);
        
        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);

        DocumentSurface doc = new(_selectionSurface);
        doc.EditingSurfaceFont = document.EditingSurfaceFont;
        doc.EditingSurfaceFontSize = document.EditingSurfaceFontSize;
        doc.EditorFontSize = document.EditorFontSize;
        doc.EditingSurface.Font = document.EditingSurface.Font;
        doc.EditingSurface.FontSize = document.EditingSurface.FontSize;

        doc.Title = Document.GenerateName("Surface");

        Core.State.Documents.Add(doc);
    }

    private void CopyToLayeredDocument(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);

        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);

        DocumentLayeredSurface doc = new(new (_selectionSurface, document.EditingSurface.Font, document.EditingSurface.FontSize));
        doc.EditingSurfaceFont = document.EditingSurfaceFont;
        doc.EditingSurfaceFontSize = document.EditingSurfaceFontSize;
        doc.EditorFontSize = document.EditorFontSize;

        doc.Title = Document.GenerateName("Layered");

        Core.State.Documents.Add(doc);
    }

    private void CopyToAnimatedDocument(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);

        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);

        DocumentAnimated doc = new(new AnimatedScreenObject("Animation1", document.EditingSurface.Font, document.EditingSurface.FontSize, [ _selectionSurface ]));
        doc.EditingSurfaceFont = document.EditingSurfaceFont;
        doc.EditingSurfaceFontSize = document.EditingSurfaceFontSize;
        doc.EditorFontSize = document.EditorFontSize;
        doc.EditingSurface.Font = doc.EditingSurfaceFont;
        doc.EditingSurface.FontSize = doc.EditingSurfaceFontSize;
        doc.Resync();

        doc.Title = Document.GenerateName("Animation");

        Core.State.Documents.Add(doc);
    }

    private void CancelPaste(Document document, bool setCancel = true)
    {
        document.VisualLayerToolMiddle.Surface.Clear();
        document.VisualLayerToolUpper.Surface.Clear();
        ClearState();
        _pasteOffset = Point.Zero;

        if (setCancel)
            _state = States.Cancel;
    }

    private void CopySurface(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);
        _selectionSurface.DefaultForeground = document.EditingSurface.Surface.DefaultForeground;
        _selectionSurface.DefaultBackground = document.EditingSurface.Surface.DefaultBackground;
        _selectionSurface.DefaultGlyph = document.EditingSurface.Surface.DefaultGlyph;

        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);
    }

    private void ClearSurface(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        document.EditingSurface.Surface.Clear(selectionArea);
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) =>
        CancelPaste(document, false);

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public void ClearState()
    {
        _firstPoint = Point.None;
        _secondPoint = Point.None;
        _state = States.None;
    }

    public override string ToString() =>
        Title;

    private enum States
    {
        None,
        MakingSelection,
        SelectionDone,
        Pasting,
        PastingMultiple,
        Cancel
    }
}
