﻿using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool
{
    private Point _firstPoint;
    private Point _secondPoint;
    private ShapeParameters _selectionBoxShape;
    private CellSurface _selectionSurface;
    private States _state = States.None;
    string _saveBlueprintFileName = string.Empty;
    string _saveBlueprintName = string.Empty;
    bool _pasteIgnoreEmpty = false;

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
        ImGui.SeparatorText(Title);

        ImGui.Checkbox("Ignore empty cells on paste"u8, ref _pasteIgnoreEmpty);

        ImGui.BeginDisabled(Core.State.Blueprints.Count == 0);
        if (ImGui.Button("Import"u8))
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
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        

        if (_state == States.SelectionDone && ImGui.BeginPopupContextItem("selection_rightmenu"u8) || ImGuiP.IsPopupOpen("selection_rightmenu"u8))
        {
            bool _createBlueprint = false;
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
            if (ImGui.Selectable("Create Document"u8))
            {
                CopyToDocument(document);
                ImGui.CloseCurrentPopup();
            }

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
                document.VisualLayerToolLower.Surface.Clear();
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

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

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

                if (_state == States.PastingMultiple)
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
                document.VisualLayerToolLower.Surface.Clear();
                document.VisualLayerToolUpper.Surface.Clear();

                Point pos = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition - new Point(_selectionSurface.Width / 2, _selectionSurface.Height / 2);
                _selectionSurface.Copy(document.VisualLayerToolLower.Surface, pos.X, pos.Y);
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

            document.VisualLayerToolLower.Surface.Clear();
            document.VisualLayerToolLower.Surface.DrawBox(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
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

    private void CopyToDocument(Document document)
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
        doc.Title = Document.GenerateName("Surface");

        Core.State.Documents.Objects.Add(doc);
    }

    private void CancelPaste(Document document, bool setCancel = true)
    {
        document.VisualLayerToolLower.Surface.Clear();
        document.VisualLayerToolUpper.Surface.Clear();
        ClearState();

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
