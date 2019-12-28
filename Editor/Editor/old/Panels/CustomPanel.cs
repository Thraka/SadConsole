namespace SadConsoleEditor.Panels
{
    using SadConsole;
    using SadConsole.Surfaces;
    using SadConsole.Controls;
    using SadConsole.Input;

    public abstract class CustomPanel
    {
        public string Title;
        public ControlBase[] Controls;

        /// <summary>
        /// Mouse handler passed on by the tools pane.
        /// </summary>
        /// <param name="info">Mouse data</param>
        public abstract void ProcessMouse(MouseConsoleState info);

        /// <summary>
        /// Called when the tool pane redraws this pane on itself.
        /// </summary>
        /// <param name="control">The control being positioned during redraw.</param>
        /// <returns>Additional rows this redraw will use. Tools pane assumes the size of control + 1.</returns>
        public abstract int Redraw(ControlBase control);

        public abstract void Loaded();

        public bool IsCollapsed;

		public bool IsVisible = true;
    }
}
