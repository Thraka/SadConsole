using System;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole
{
    public interface IScreenObjectSurface: IScreenObject
    {
        event EventHandler<MouseScreenObjectState> MouseButtonClicked;
        event EventHandler<MouseScreenObjectState> MouseEnter;
        event EventHandler<MouseScreenObjectState> MouseExit;
        event EventHandler<MouseScreenObjectState> MouseMove;

        Rectangle AbsoluteArea { get; }
        FocusBehavior FocusedMode { get; set; }
        bool FocusOnMouseClick { get; set; }
        Font Font { get; set; }
        Point FontSize { get; set; }
        bool ForceRendererRefresh { get; set; }
        int HeightPixels { get; }
        bool IsDirty { get; set; }
        bool IsExclusiveMouse { get; set; }
        bool IsFocused { get; set; }
        bool MoveToFrontOnMouseClick { get; set; }
        IRenderer Renderer { get; set; }

        /// <summary>
        /// The surface the screen object represents.
        /// </summary>
        CellSurface Surface { get; }
        Color Tint { get; set; }
        bool UsePixelPositioning { get; set; }
        int WidthPixels { get; }

        void Dispose();
        void Draw();
        void LostMouse(MouseScreenObjectState state);
        void OnFocused();
        void OnFocusLost();
        bool ProcessKeyboard(Keyboard keyboard);
        bool ProcessMouse(MouseScreenObjectState state);
        void Update();
        void UpdateAbsolutePosition();
    }
}
