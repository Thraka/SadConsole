using System.Numerics;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool, IOverlay
{
    private bool _isFirstPointSelected = false;
    private Rectangle _boxArea;
    private Point _firstPoint;
    private bool _boxCreated;
    private Overlay _toolOverlay = new();
    private bool _isCancelled;
    private bool _isPasting;
    CellSurface? _blueprint;
    private string[] _blueprintNames = [];
    private CellSurface[] _blueprintValues = [];
    private int _blueprintSelection = -1;

    private ShapeSettings.Settings _shapeSettings;

    public string Name => "Selection";

    public string Description => """
        ...
        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Overlay Overlay => _toolOverlay;

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

        ImGui.BeginDisabled(!_boxCreated);

        if (ImGui.Button("Reset"))
        {
            OnDeselected();
        }
        else if (ImGui.Button("Copy"))
        {

        }
        else if (ImGui.Button("Store"))
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

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        GuiParts.Tools.ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

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

        if (_isPasting)
        {
            _boxArea = _boxArea.WithCenter(hoveredCellPosition - surface.Surface.ViewPosition);
            Overlay.Surface.Clear();
            Overlay.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxThin(Color.White));
        }
        else
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - surface.Surface.ViewPosition;
                }

                Point secondPoint = hoveredCellPosition - surface.Surface.ViewPosition;

                _boxArea = new(new Point(Math.Min(_firstPoint.X, secondPoint.X), Math.Min(_firstPoint.Y, secondPoint.Y)),
                                new Point(Math.Max(_firstPoint.X, secondPoint.X), Math.Max(_firstPoint.Y, secondPoint.Y)));

                Overlay.Surface.Clear();
                Overlay.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxThin(Color.White));
            }
            else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_boxArea != Rectangle.Empty)
                {
                    _blueprint = new(_boxArea.Width, _boxArea.Height);
                    surface.Surface.Copy(_boxArea.Translate(surface.Surface.ViewPosition), _blueprint, 0, 0);
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
        Overlay.Surface.Clear();
        _isCancelled = false;
        _boxArea = Rectangle.Empty;
        _isFirstPointSelected = false;
        _boxCreated = false;
        _blueprint = null;
    }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
