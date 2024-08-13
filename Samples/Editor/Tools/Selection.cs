using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.ImGuiSystem;
using SadConsole.Editor.Model;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool
{
    private bool _isFirstPointSelected = false;
    private Rectangle _boxArea;
    private Point _firstPoint;
    private bool _boxCreated;
    private bool _isCancelled;
    private bool _isPasting;
    CellSurface? _blueprint;
    private string[] _blueprintNames = [];
    private CellSurface[] _blueprintValues = [];
    private int _blueprintSelection = -1;
    private CloneState _cloneState;

    private ShapeSettings.Settings _shapeSettings;

    public string Name => "Selection";

    public string Description => """
        ...
        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        /*
         * --- Selection ---
         * BtnReset
         * BtnCopy
         * BtnStore
         * 
         * --- Blueprints ---
         * List Item
         * List Item
         * List Item
         * List Item
         * 
         * Btn
         * 
        */
        
        ImGuiWidgets.BeginGroupPanel("Settings");

        ImGui.BeginDisabled(_cloneState != CloneState.Selected);
        if (ImGui.Button("Reset"))
        {
            OnDeselected();
        }
        ImGui.EndDisabled();

        ImGui.BeginDisabled(_cloneState != CloneState.Selected);
        if (ImGui.Button("Move"))
        {
            _cloneState = CloneState.Move;

            // Clear the spots where the selection was
            ImGuiCore.State.GetOpenDocument().VisualDocument.Surface.Clear(_boxArea.Translate(ImGuiCore.State.GetOpenDocument().VisualDocument.Surface.ViewPosition));
        }
        ImGui.EndDisabled();

        ImGui.BeginDisabled(_cloneState != CloneState.Selected);
        if (ImGui.Button("Copy"))
        {

        }
        ImGui.EndDisabled();

        ImGui.BeginDisabled(_cloneState != CloneState.Selected);
        if (ImGui.Button("Clear"))
        {

        }
        ImGui.EndDisabled();

        ImGui.BeginDisabled(_cloneState != CloneState.Selected);
        if (ImGui.Button("Store"))
        {
            ImGui.OpenPopup("Blueprint Name");
        }
        ImGui.EndDisabled();

        if (_blueprint != null)
        {
            string blueprintName = string.Empty;
            bool validateOutput(string name) =>
                string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || _blueprintNames.Contains(name);

            if (Windows.RenameWindow.Show("Blueprint Name", "Name", ref blueprintName, out bool accepted, validateOutput))
            {
                if (accepted)
                {
                    _blueprintNames = [.. _blueprintNames, blueprintName];
                    _blueprintValues = [.. _blueprintValues, _blueprint];
                }
            }
        }

        ImGui.Separator();
        ImGui.Text("Stored Blueprints");
        
        ImGui.ListBox("##listblueprints", ref _blueprintSelection, _blueprintNames, _blueprintNames.Length, 5);

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        // Cancelled but left mouse finally released, exit cancelled
        if (_isCancelled && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            _isCancelled = false;

        // Cancelled
        if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape)))
        {
            OnDeselected();
            _isCancelled = true;
        }

        if (_isCancelled)
            return;

        if (_cloneState == CloneState.SelectingPoint1)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;
                }

                Point secondPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;

                _boxArea = new(new Point(Math.Min(_firstPoint.X, secondPoint.X), Math.Min(_firstPoint.Y, secondPoint.Y)),
                                new Point(Math.Max(_firstPoint.X, secondPoint.X), Math.Max(_firstPoint.Y, secondPoint.Y)));

                document.VisualToolLayerLower.Surface.Clear();
                document.VisualToolLayerLower.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxFilled(borderStyle: ICellSurface.ConnectedLineThin,
                                                                                        borderColors: new ColoredGlyph(Color.White, Color.Black),
                                                                                        fillStyle: new ColoredGlyph(Color.White.SetAlpha(150), Color.Transparent, 176)));
            }
            else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_boxArea != Rectangle.Empty)
                {
                    _blueprint = new(_boxArea.Width, _boxArea.Height);
                    document.VisualDocument.Surface.Copy(_boxArea.Translate(document.VisualDocument.Surface.ViewPosition), _blueprint, 0, 0);
                    _boxCreated = true;
                    _isFirstPointSelected = false;
                    _cloneState = CloneState.Selected;
                }
                else
                {
                    OnDeselected();
                }
            }
        }

        // Reset by mouse or keyboard
        else if (_cloneState == CloneState.Selected)
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnDeselected();
            }
        }

        // Movement mode
        else if (_cloneState == CloneState.Move)
        {

        }

        ////////////////////////////////////////////////////////

        if (_isPasting)
        {
            _boxArea = _boxArea.WithCenter(hoveredCellPosition - document.VisualDocument.Surface.ViewPosition);
            document.VisualToolLayerLower.Surface.Clear();
            document.VisualToolLayerLower.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxThin(Color.White));
        }
        else
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;
                }

                Point secondPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;

                _boxArea = new(new Point(Math.Min(_firstPoint.X, secondPoint.X), Math.Min(_firstPoint.Y, secondPoint.Y)),
                                new Point(Math.Max(_firstPoint.X, secondPoint.X), Math.Max(_firstPoint.Y, secondPoint.Y)));

                document.VisualToolLayerLower.Surface.Clear();
                document.VisualToolLayerLower.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxFilled(borderStyle: ICellSurface.ConnectedLineThin,
                                                                                        borderColors: new ColoredGlyph(Color.White, Color.Black),
                                                                                        fillStyle: new ColoredGlyph(Color.White.SetAlpha(150), Color.Transparent, 176)));
            }
            else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_boxArea != Rectangle.Empty)
                {
                    _blueprint = new(_boxArea.Width, _boxArea.Height);
                    document.VisualDocument.Surface.Copy(_boxArea.Translate(document.VisualDocument.Surface.ViewPosition), _blueprint, 0, 0);
                    _boxCreated = true;
                    _isFirstPointSelected = false;
                }
            }
            else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                OnDeselected();
            }
        }
    }

    public void OnSelected()
    {

    }

    public void OnDeselected()
    {
        _isCancelled = false;
        _boxArea = Rectangle.Empty;
        _isFirstPointSelected = false;
        _boxCreated = false;
        _blueprint = null;
        _cloneState = CloneState.SelectingPoint1;
    }

    public void DocumentViewChanged(Document document)
    {
        OnDeselected();
    }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }


    public enum CloneState
    {
        SelectingPoint1,
        Selected,
        Stamp,
        Clear,
        Move
    }
}
