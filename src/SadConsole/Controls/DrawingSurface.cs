namespace SadConsole.Controls
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A simple surface for drawing text that can be moved and sized like a control.
    /// </summary>
    [DataContract]
    public class DrawingSurface : ControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Action<DrawingSurface> OnDraw { get; set; }

        /// <summary>
        /// Creates a new drawing surface control with the specified width and height.
        /// </summary>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        public DrawingSurface(int width, int height) : base(width, height) => base.TabStop = false;

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context) => base.TabStop = false;
    }
}
