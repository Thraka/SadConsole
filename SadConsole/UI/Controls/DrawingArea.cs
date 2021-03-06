using System;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// A simple surface for drawing text that can be moved and sized like a control.
    /// </summary>
    [DataContract]
    public class DrawingArea : ControlBase
    {
        /// <summary>
        /// Called when the surface is redrawn.
        /// </summary>
        public Action<DrawingArea, TimeSpan> OnDraw { get; set; }

        /// <summary>
        /// Creates a new drawing surface control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public DrawingArea(int width, int height) : base(width, height)
        {
            UseMouse = false;
            UseKeyboard = false;
            TabStop = false;
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context) => base.TabStop = false;
    }
}
