using System;
using SadConsole;
using SadConsole.UI;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    public class ScrollableConsole : SadConsole.Console
    {
        private readonly SadConsole.Console _controlsContainer;
        private readonly SadConsole.UI.Controls.ScrollBar _scrollBar;

        ///<summary>Scroll bar position.</summary>
        public int ScrollOffset { get; private set; } = 0;

        public bool ScrollbarIsVisible
        {
            get => _controlsContainer.IsVisible;
            set => _controlsContainer.IsVisible = value;
        }

        public ScrollableConsole(int width, int height, int bufferHeight) : base(width - 1, height, width -1, bufferHeight)
        {
            _controlsContainer = new SadConsole.Console(1, height);

            _scrollBar = new SadConsole.UI.Controls.ScrollBar(Orientation.Vertical, height);
            _scrollBar.IsEnabled = false;
            _scrollBar.ValueChanged += ScrollBar_ValueChanged;

            var controlHost = new SadConsole.UI.ControlHost();
            controlHost.Add(_scrollBar);
            _controlsContainer.SadComponents.Add(controlHost);
            _controlsContainer.Position = new Point(Position.X + width, Position.Y);
            _controlsContainer.IsVisible = true;

            Cursor.IsVisible = true;
            Cursor.IsEnabled = true;
            Cursor.Print("Just start typing!");

            UseMouse = true;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            //Display viewable content based on our scroll offset.
            View = new Rectangle(0, _scrollBar.Value, Width, ViewHeight);
        }

        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            base.OnPositionChanged(oldPosition, newPosition);
            //Keep the controls console (which is our scroll bar) in sync with where this console is.
            _controlsContainer.Position = new Point(Position.X + Width, Position.Y);
        }

        protected override void OnVisibleChanged()
        {
            _controlsContainer.IsVisible = this.IsVisible;
        }

        /// <inheritdoc />
        public override void Draw(TimeSpan delta)
        {
            // Draw our console and then draw the scroll bar.
            base.Draw(delta);
            _controlsContainer.Draw(delta);
        }

        public override void Update(TimeSpan delta)
        {
            // Update our console and then update the scroll bar
            base.Update(delta);
            _controlsContainer.Update(delta);

            //If cursor position exceeds our displayable content viewport, 
            //move the ScrollOffset automatically to display new content.
            if (TimesShiftedUp != 0 | Cursor.Position.Y >= ViewHeight + ScrollOffset)
            {
                //Scollbar has to be enabled to read previous content.
                _scrollBar.IsEnabled = true;

                //Cursor offset cannot exceed our viewable data end row.
                //Think about it, we would reach infinity and empty space D:
                if (ScrollOffset < Height - ViewHeight)
                {
                    //Automatically calculate our content viewport by scrolling the cursor
                    //Based on how much content is inaccessible.
                    ScrollOffset += TimesShiftedUp != 0 ? TimesShiftedUp : 1;
                }
                _scrollBar.Maximum = (Height + ScrollOffset) - Height;

                //This will follow the cursor since we move the render area in the event.
                _scrollBar.Value = ScrollOffset;

                // Reset the shift amount.
                TimesShiftedUp = 0;
            }
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            //Create a state based on our container that has the scroll bar.
            var stateForScroll = new MouseScreenObjectState(_controlsContainer, state.Mouse);

            //Check if this state based on the console holding the scroll bar.
            if (stateForScroll.IsOnScreenObject)
            {
                _controlsContainer.ProcessMouse(stateForScroll);
                return true;
            }

            //If we're here, continue the mouse processing flow ordinarily.
            return base.ProcessMouse(state);
        }
    }

}
